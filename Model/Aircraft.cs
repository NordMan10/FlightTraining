using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FlightTraining.Model
{
    public class Aircraft : IAircraft
    {
        public Aircraft(AircraftType type, int id, string name, int velocity, string entryTime,
            Image image, int imageSize, int trackId_, List<IThreeDPoint> path, int pixelsInCell_)
        {
            Type = type;
            Id = id;
            Name = name.ToUpper() + "_" + entryTime.ToString();
            X = path[0].X;
            Y = path[0].Y;
            Z = path[0].Z;
            Velocity = velocity;
            HeightToGain = 0;
            EntryTime = entryTime;
            Image = image;
            ImageSize = imageSize;
            Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            InitInfoForm();
            Shifts = new Shifts();
            Tracker = 0;
            NeedToRemove = false;
            TrackId = trackId_;
            Path = path;
            FlightStageChanged += Aircraft_FlightStageChanged;
            ChangeFlightStage(FlightStage.Ordinary);
        }

        private Action<int, Shifts> CalcAndSetShifts = (tracker, shifts) => throw new NotImplementedException();

        private event Action<FlightStage> FlightStageChanged;

        public int TrackId { get; }

        private readonly AircraftFutureLocationExt futureLocation = new AircraftFutureLocationExt();

        public AircraftType Type { get; }

        public int Id { get; }

        public string Name { get; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Z { get; private set; }

        public int Velocity { get; }

        public int HeightToGain { get; private set; }

        public string EntryTime { get; }

        public Image Image { get; }

        public int ImageSize { get; }

        public Shifts Shifts { get; private set; }

        public Label InfoForm { get; private set; }

        public List<IThreeDPoint> Path { get; }

        public Tuple<Point, Point> OldPointsCoords { get; private set; }

        public int Tracker { get; private set; }

        public bool NeedToRemove { get; private set; }

        public FlightStage FlightStage { get; private set; }


        private void Aircraft_FlightStageChanged(FlightStage stage)
        {
            switch (stage)
            {
                case FlightStage.Ordinary:
                    ChangeAndInvoke_Shifts(CalcAndSetShifts_Ordinary, Tracker);
                    break;
                case FlightStage.Maneuver:
                    ChangeAndInvoke_Shifts(CalcAndSetShifts_Maneuver, Tracker);
                    break;
                case FlightStage.HeightHold:
                    ChangeAndInvoke_Shifts(CalcAndSetShifts_HeightHold, Tracker);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Изменяет метод для расчёта смещений в делегате. Относится к обработчику события изменения стадии полёта
        /// </summary>
        /// <param name="action">Метод для расчёта смещений</param>
        private void ChangeAndInvoke_Shifts(Action<int, Shifts> action, int tracker_, Shifts shifts_ = null)
        {
            CalcAndSetShifts = (tracker, shifts) => action(tracker, shifts);
            Invoke_CalculationShifts(tracker_, shifts_);
        }

        private void Invoke_CalculationShifts(int tracker, Shifts shifts = null) { CalcAndSetShifts(tracker, shifts); }

        private void InitInfoForm()
        {
            InfoForm = new Label();
            InfoForm.Width = 90;
            InfoForm.Height = 45;
            InfoForm.Location = new Point((int)X + ImageSize, (int)Y - InfoForm.Height - ImageSize / 2);
            InfoForm.Text = Name + "\n" + ((int)Z).ToString() + " | " + Velocity.ToString();
            InfoForm.TextAlign = ContentAlignment.MiddleCenter;
            InfoForm.Font = new Font("Roboto", 10F, FontStyle.Bold, GraphicsUnit.Point, 204);
            //InfoForm.ForeColor = Color.White;
            InfoForm.BorderStyle = BorderStyle.FixedSingle;
            InfoForm.FlatStyle = FlatStyle.Flat;
            InfoForm.BackColor = Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF);
        }

        public void Move()
        {
            CheckToHeightHold();

            X += Shifts.Dx;
            Y += Shifts.Dy;
            Z += Shifts.Dz;

            UpdateInfoForm();
        }

        private void CheckToHeightHold()
        {
            if (Z >= ProgramOptions.UmvTrackGainHeight[TrackId])
                ChangeFlightStage(FlightStage.HeightHold);
        }

        private void UpdateInfoForm()
        {
            InfoForm.Location = new Point((int)X + ImageSize, (int)Y - InfoForm.Height - ImageSize / 2);
            InfoForm.Text = Name + "\n" + ((int)Z).ToString() + " | " + Velocity.ToString();
        }

        public void CheckToChangePath()
        {
            if (IsPointNear()) ChangeShifts();
        }

        private void ChangeShifts()
        {
            if (Tracker + 1 < Path.Count - 1)
            {
                Tracker++;
                Invoke_CalculationShifts(Tracker);
            }
            else
            {
                Shifts.Dx = Shifts.Dy = Shifts.Dz = 0;
                NeedToRemove = true;
            }
            
        }

        private bool IsPointNear()
        {
            var dx = Math.Abs(Path[Tracker + 1].X - X);
            var dy = Math.Abs(Path[Tracker + 1].Y - Y);
            if (dx + dy <= ProgramOptions.CloseDistance) return true;
            else return false;
        }

        private void CalcAndSetShifts_Ordinary(int tracker, Shifts shifts = null)
        {
            var directShift = GetDirectShift();
            var hypotenuseV = directShift / Math.Cos(Path[tracker].ShiftsData[TrackId][1]);

            SetShifts(shifts, directShift * Math.Cos(Path[tracker].ShiftsData[TrackId][0]),
                directShift * Math.Sin(Path[tracker].ShiftsData[TrackId][0]),
                hypotenuseV * Math.Sin(Path[tracker].ShiftsData[TrackId][1]));
        }

        private void CalcAndSetShifts_Maneuver(int tracker, Shifts shifts = null)
        {
            var directShift = GetDirectShift();

            SetShifts(shifts, directShift * Math.Cos(Path[tracker].ShiftsData[TrackId][0]),
                directShift * Math.Sin(Path[tracker].ShiftsData[TrackId][0]),
                GetHeightManeuverShift(HeightToGain, ProgramOptions.TimeToGainHeight));
        }

        private void CalcAndSetShifts_HeightHold(int tracker, Shifts shifts = null)
        {
            var directShift = GetDirectShift();

            SetShifts(shifts, directShift * Math.Cos(Path[tracker].ShiftsData[TrackId][0]),
                 directShift * Math.Sin(Path[tracker].ShiftsData[TrackId][0]), 0);
        }

        private void SetShifts(Shifts shifts, double dx, double dy, double dz)
        {
            if (shifts != null)
            {
                shifts.Dx = dx;
                shifts.Dy = dy;
                shifts.Dz = dz;
            }
            else
            {
                Shifts.Dx = dx;
                Shifts.Dy = dy;
                Shifts.Dz = dz;
            }
        }

        public void SetHeightToGain(int height) { HeightToGain = height; }

        private double GetHeightManeuverShift(int height, int time)
        {
            var dh = height - Z;
            return dh / time / ProgramOptions.TimeCoafficient;
        }

        /// <summary>
        /// Возвращает расстояние в пикселях, проходимое ВС в любую сторону за 1 с
        /// </summary>
        /// <returns></returns>
        private double GetDirectShift()
        {
            return Velocity / ProgramOptions.TimeCoafficient / ProgramOptions.MetersInCell * ProgramOptions.PixelsInCell;
        }

        public void UpdateLocationAndShifts() // РЕФАКТОРИНГ
        {
            var ratioX = (double)Math.Abs(Path[Tracker + 1].X - Path[Tracker].X) / (double)Math.Abs(OldPointsCoords.Item2.X - OldPointsCoords.Item1.X);
            if (Math.Abs(OldPointsCoords.Item2.X - OldPointsCoords.Item1.X) == 0)
            {
                ratioX = Path[Tracker].X - OldPointsCoords.Item1.X;
                X += ratioX;
            }
            else X *= ratioX;

            var ratioY = (double)Math.Abs(Path[Tracker + 1].Y - Path[Tracker].Y) / (double)Math.Abs(OldPointsCoords.Item2.Y - OldPointsCoords.Item1.Y);
            if (Math.Abs(OldPointsCoords.Item2.Y - OldPointsCoords.Item1.Y) == 0)
            {
                ratioY = Path[Tracker].Y - OldPointsCoords.Item1.Y;
                Y += ratioY;
            }
            else Y *= ratioY;


            //var startHeight = 800;

            //var similarityRatio = Z / startHeight;

            //var dx = (double)Math.Abs(Path[tracker + 1].X - Path[tracker].X);
            //var dy = (double)Math.Abs(Path[tracker + 1].Y - Path[tracker].Y);

            //X = Path[tracker].X + (dx * (1 - similarityRatio));
            //Y = Path[tracker].Y + (dy * (1 - similarityRatio));

            Invoke_CalculationShifts(Tracker);
        }

        public void SaveOldPath()
        {
            OldPointsCoords = Tuple.Create(new Point(Path[Tracker].X, Path[Tracker].Y), new Point(Path[Tracker + 1].X, Path[Tracker + 1].Y));
        }

        /// <summary>
        /// Изменяет значение сдвига по вертикали для набора указанной высоты за указанное время
        /// </summary>
        /// <param name="height">Высота в метрах, которую нужно набрать</param>
        /// <param name="time">Время в секундах, за которое нужно набрать высоту</param>

        public void ChangeFlightStage(FlightStage stage)
        {
            FlightStage = stage;
            FlightStageChanged.Invoke(stage);
        }


        /// <summary>
        /// Вызывает метод, возвращающий точку, в которой окажется ВС через указанный промежуток времени
        /// </summary>
        /// <returns></returns>
        public IThreeDPoint GetFutureLocation()
        {
            return futureLocation.GetFutureLocation(X, Y, Z, Shifts.Dx, Shifts.Dy, Shifts.Dz, 
                Tracker, Path, CalcAndSetShifts, ProgramOptions.PredictInterval);
        }
    }
}
