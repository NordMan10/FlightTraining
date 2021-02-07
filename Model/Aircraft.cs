using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FlightTraining.Model
{
    public class Aircraft : IAircraft
    {
        public Aircraft(AircraftType type, int id, string name, int velocity, string entryTime,
            Image image, int imageSize, int trackId, List<Point3D> path)
        {
            Type = type;
            Id = id;
            Name = name.ToUpper() + "_" + entryTime;
            //Location = new Point3D(path[0].X, path[0].Y, path[0].Z);
            X = path[0].X;
            Y = path[0].Y;
            Z = path[0].Z;
            Velocity = velocity;
            HeightToGain = 0;
            EntryTime = entryTime;
            InitImage(image, imageSize);
            InitInfoForm();
            Tracker = 0;
            TrackId = trackId;
            Path = path;
            FlightStageChanged += Aircraft_FlightStageChanged;
            ChangeFlightStage(FlightStage.Ordinary);
        }

        private Action<int, Shifts> CalcAndSetShifts = (tracker, shifts) => 
            throw new NotImplementedException();

        private event Action<FlightStage> FlightStageChanged;

        public int TrackId { get; }

        private AircraftFutureLocation FutureLocation { get; } = new AircraftFutureLocation();

        public AircraftType Type { get; }

        public int Id { get; }

        public string Name { get; }
        //public Point3D Location { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Z { get; private set; }

        public int Velocity { get; }

        public int HeightToGain { get; private set; }

        public string EntryTime { get; }

        public Image Image { get; private set; }

        public int ImageSize { get; private set; }

        public Shifts Shifts { get; } = new Shifts();

        public Label InfoForm { get; private set; }

        public List<Point3D> Path { get; }

        public Tuple<Point, Point> OldPointsCoords { get; private set; }

        /// <summary>
        /// Точка, которую прошло данное ВС
        /// </summary>
        public int Tracker { get; private set; }

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

        private void InitImage(Image image, int imageSize)
        {
            Image = image;
            ImageSize = imageSize;
            Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        /// <summary>
        /// Изменяет метод в делегате для расчёта смещений. Относится к обработчику 
        /// события изменения стадии полёта
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
            InfoForm.BorderStyle = BorderStyle.FixedSingle;
            InfoForm.FlatStyle = FlatStyle.Flat;
            InfoForm.BackColor = Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF);
        }

        public void Move()
        {
            CheckToHeightHold();

            //Location.UpdateCoords(Tuple.Create(Location.X + Shifts.Dx, Location.Y + Shifts.Dy, Location.Z + Shifts.Dz));

            X += Shifts.Dx;
            Y += Shifts.Dy;
            Z += Shifts.Dz;

            UpdateInfoForm();
        }
        public void CheckToChangePath()
        {
            var currentPoint = new Point3D((int) X, (int) Y, (int) Z);
            var nextPoint = new Point3D(Path[Tracker + 1].X, Path[Tracker + 1].Y, Path[Tracker + 1].Z);
            if (IsNextPointNear(currentPoint, nextPoint, AircraftOptions.CloseDistance)) ChangeShifts();
        }

        private void CheckToHeightHold()
        {
            if (Type == AircraftType.Umv && Z >= AircraftOptions.UmvTracksGainHeight[TrackId])
                ChangeFlightStage(FlightStage.HeightHold);
        }

        private void UpdateInfoForm()
        {
            InfoForm.Location = new Point((int)X + ImageSize, (int)Y - InfoForm.Height - ImageSize / 2);
            InfoForm.Text = Name + "\n" + ((int)Z).ToString() + " | " + Velocity.ToString();
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
                FlightStage = FlightStage.NeedToRemove;
            }
        }

        /// <summary>
        /// Метод определяет, находится ли текущее судно на заданном расстоянии от точки,
        /// которая является следующей точкой в его маршруте. Сравнивается расстояние,
        /// являющееся суммой расстояний по осям X и Y между двумя точками. Текущая точка,
        /// следующая точка и расстояние для сравнения передаются во входных параметрах.
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="nextPoint"></param>
        /// <param name="checkValue"></param>
        /// <returns></returns>

        private bool IsNextPointNear(Point3D currentPoint, Point3D nextPoint, int checkValue)
        {
            // Вычислить разность координат между точкой текущего местоположения и следующей точкой
            // по оси X и Y.
            // Взять эти разности по модулю.
            var dx = Math.Abs(nextPoint.X - currentPoint.X);
            var dy = Math.Abs(nextPoint.Y - currentPoint.Y);
            // Сложить их. Проверить, является ли полученная сумма меньшей либо равной заданному значению.
            // Вернуть результат проверки.
            return dx + dy <= checkValue;
        }

        private void CalcAndSetShifts_Ordinary(int tracker, Shifts shifts = null)
        {
            var shiftXY = GetShiftXY();
            var shiftZ = shiftXY / Math.Cos(Path[tracker].ShiftsData[TrackId][1]);

            SetShifts(shifts, 
                shiftXY * Math.Cos(Path[tracker].ShiftsData[TrackId][0]),
                shiftXY * Math.Sin(Path[tracker].ShiftsData[TrackId][0]),
                shiftZ * Math.Sin(Path[tracker].ShiftsData[TrackId][1]));
        }

        private void CalcAndSetShifts_Maneuver(int tracker, Shifts shifts = null)
        {
            var directShift = GetShiftXY();

            SetShifts(shifts, 
                directShift * Math.Cos(Path[tracker].ShiftsData[TrackId][0]),
                directShift * Math.Sin(Path[tracker].ShiftsData[TrackId][0]),
                GetHeightManeuverShift(HeightToGain, AircraftOptions.TimeToGainHeight));
        }

        private void CalcAndSetShifts_HeightHold(int tracker, Shifts shifts = null)
        {
            var directShift = GetShiftXY();

            SetShifts(shifts, 
                directShift * Math.Cos(Path[tracker].ShiftsData[TrackId][0]),
                directShift * Math.Sin(Path[tracker].ShiftsData[TrackId][0]), 
                0);
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

        /// <summary>
        /// Возвращает сдвиг для манёвра по высоте
        /// </summary>
        /// <param name="height">Высота в метрах, которую необходимио набрать</param>
        /// <param name="time">Время в секундах, за которое необходимо набрать высоту</param>
        /// <returns></returns>
        private double GetHeightManeuverShift(int height, int time)
        {
            var dh = height - Z;
            return dh / time / ProgramOptions.TimeCoefficient;
        }

        /// <summary>
        /// Возвращает расстояние в пикселях, проходимое ВС в плоскости XY за 1 с
        /// </summary>
        /// <returns></returns>
        private double GetShiftXY()
        {
            return Velocity / ProgramOptions.TimeCoefficient / 
                ProgramOptions.MetersInCell * ProgramOptions.PixelsInCell;
        }

        public void UpdateLocationAndShifts()
        {
            X = UpdateCoordinate(X, Path[Tracker + 1].X, Path[Tracker].X,
                OldPointsCoords.Item2.X, OldPointsCoords.Item1.X);
            Y = UpdateCoordinate(Y, Path[Tracker + 1].Y, Path[Tracker].Y, 
                OldPointsCoords.Item2.Y, OldPointsCoords.Item1.Y);

            Invoke_CalculationShifts(Tracker);
        }

        private double UpdateCoordinate(double coord, int newFinishPointCoord, 
            int newStartPointCoord, int oldFinishPointCoord, int oldStartPointCoord)
        {
            var ratio = (double)Math.Abs(newFinishPointCoord - newStartPointCoord) / 
                Math.Abs(oldFinishPointCoord - oldStartPointCoord);

            if (Math.Abs(oldFinishPointCoord - oldStartPointCoord) == 0)
            {
                ratio = newStartPointCoord - oldStartPointCoord;
                return coord += ratio;
            }
            else return coord *= ratio;
        }

        public void SaveOldPath()
        {
            OldPointsCoords = Tuple.Create(new Point(Path[Tracker].X, Path[Tracker].Y), 
                new Point(Path[Tracker + 1].X, Path[Tracker + 1].Y));
        }

        public void ChangeFlightStage(FlightStage stage)
        {
            FlightStage = stage;
            FlightStageChanged.Invoke(stage);
        }

        public Point3D GetFutureLocation()
        {
            return FutureLocation.GetFutureLocation(X, Y, Z, Shifts.Dx, Shifts.Dy, Shifts.Dz, 
                Tracker, Path, CalcAndSetShifts, AircraftOptions.PredictInterval);
        }

        public FlightStage GetFlightStage() { return FlightStage; }

        public Tuple<double, double, double> GetCoords()
        {
            return Tuple.Create(X, Y, Z);
        }
    }
}
