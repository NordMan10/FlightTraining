using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FlightTraining.Model;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace FlightTraining.Views
{
    public partial class FieldControl : UserControl
    {
        private IProgramModel model;
        private IField field;
        private readonly Action<Control> addControl;
        private readonly Action<Control> removeControl;
        private bool configured = false;

        public FieldControl()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Resize += FieldControl_Resize;

            addControl = control => Controls.Add(control);
            removeControl = control => Controls.Remove(control);

            PlaneTimer = UmvTimer = new Timer();
            InitTimers();
        }

        public Timer PlaneTimer { get; private set; }

        public Timer UmvTimer { get; private set; }

        private void InitTimers()
        {
            PlaneTimer = new Timer();
            UmvTimer = new Timer();
            PlaneTimer.Interval = GetRandomTimerInterval();
            UmvTimer.Interval = GetRandomTimerInterval();
            PlaneTimer.Tick += PlaneTimer_Tick;
            UmvTimer.Tick += UmvTimer_Tick;
        }

        private void PlaneTimer_Tick(object sender, EventArgs e)
        {
            AircraftTimer_TickHandler(AircraftType.Plane, ProgramOptions.PlaneTracks.Count, PlaneTimer);
        }

        private void UmvTimer_Tick(object sender, EventArgs e)
        {
            AircraftTimer_TickHandler(AircraftType.Umv, ProgramOptions.UmvTracks.Count, UmvTimer);
        }

        private void AircraftTimer_TickHandler(AircraftType type, int tracksCount, Timer timer)
        {
            var randomTrackId = GetRandomTrackId(0, tracksCount);
            field.AddAircraft(type, randomTrackId, addControl);
            timer.Interval = GetRandomTimerInterval();
        }

        public int GetRandomTrackId(int leftBorder, int rightBorder)
        {
            return ProgramOptions.Random.Next(leftBorder, rightBorder);
        }

        private int GetRandomTimerInterval()
        {
            return ProgramOptions.Random.Next(ProgramOptions.AircraftInterval.Item1, ProgramOptions.AircraftInterval.Item2);
        }

        public void AddFirstAircrafts()
        {
            field.AddAircraft(AircraftType.Plane, GetRandomTrackId(0, ProgramOptions.PlaneTracks.Count - 1), addControl);
            field.AddAircraft(AircraftType.Umv, 1/*GetRandomTrackId(0, ProgramOptions.UmvTracks.Count - 1)*/, addControl);

        }

        private void FieldControl_Resize(object sender, EventArgs e)
        {
            if (configured)
            {
                field.UpdateCoords(Width, Height);
                field.CreatePaths();
                Invalidate();
            }
        }

        public void Configure(IProgramModel model_)
        {
            if (configured) return;
            model = model_;
            field = new Field(model, Width, Height);

            Invalidate();
            configured = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            e.Graphics.DrawLayout(field.Points.XLayoutPoints, field.Points.YLayoutPoints);
            e.Graphics.DrawRestrZone(field.Points.RestrictedArea);
            
            var planeTrajectories = GetTrajectoryPoints(ProgramOptions.PlaneTracks, field.Points.PlanePoints);
            e.Graphics.DrawPlaneTrajectories(planeTrajectories, GraphicsExtensions.PlaneTrajectoryPen);

            var umvTrajectories = GetTrajectoryPoints(ProgramOptions.UmvTracks, field.Points.UmvPoints);
            e.Graphics.DrawPlaneTrajectories(umvTrajectories, GraphicsExtensions.UmvTrajectoryPen);
            
            var planePoints = GetAircraftPoints(field.Points.PlanePoints);
            e.Graphics.DrawAircraftPoints(planePoints, GraphicsExtensions.PlanePointsBrush);

            var umvPoints = GetAircraftPoints(field.Points.UmvPoints);
            e.Graphics.DrawAircraftPoints(umvPoints, GraphicsExtensions.UmvPointsBrush);
            
            e.Graphics.DrawAircrafts(field.Planes);
            e.Graphics.DrawAircrafts(field.Umvs);

            foreach (var plane in field.Planes.Values)
            {
                var futureLocation = plane.GetFutureLocation();
                var rect = new Rectangle(futureLocation.X - 5, futureLocation.Y - 5, 10, 10);
                e.Graphics.DrawRoundPoint(rect, GraphicsExtensions.OrdinaryPen, GraphicsExtensions.PlanePointsBrush);
            }

            foreach (var umv in field.Umvs.Values)
            {
                var futureLocation = umv.GetFutureLocation();
                var rect = new Rectangle(futureLocation.X - 5, futureLocation.Y - 5, 10, 10);
                e.Graphics.DrawRoundPoint(rect, GraphicsExtensions.OrdinaryPen, GraphicsExtensions.UmvPointsBrush);
            }

        }

        private List<IThreeDPoint> GetAircraftPoints(List<Dictionary<int, IThreeDPoint>> airctaftPoints)
        {
            var result = new List<IThreeDPoint>();
            foreach (var points in airctaftPoints)
                foreach (var point in points.Values)
                    result.Add(point);
            return result;
        }

        private List<Tuple<Point, Point>> GetTrajectoryPoints(Dictionary<int, int?[]> tracks, List<Dictionary<int, IThreeDPoint>> points)
        {
            var result = new List<Tuple<Point, Point>>();
            foreach (var track in tracks)
            {
                var path = new List<IThreeDPoint>();
                for (var i = 0; i < track.Value.Length; i++)
                {
                    if (track.Value[i] == null) continue;
                    path.Add(points[i][(int)track.Value[i]]);
                }
                for (var i = 0; i < path.Count - 1; i++)
                {
                    result.Add(Tuple.Create(new Point(path[i].X, path[i].Y), new Point(path[i + 1].X, path[i + 1].Y)));
                }
            }
            return result;
        }

        public void StartTimers()
        {
            PlaneTimer.Start();
            UmvTimer.Start();
        }

        public void StopTimers()
        {
            PlaneTimer.Stop();
            UmvTimer.Stop();
        }

        /// <summary>
        /// Меняет координаты всех ВС на соответствующие смещения. ВС в свою очередь меняют координаты своих формуляров.
        /// </summary>
        public void MoveAllAircrafts()
        {
            UpdateAircrafts(field.Planes);
            UpdateAircrafts(field.Umvs);
        }

        private void UpdateAircrafts(Dictionary<int, IAircraft> aircrafts)
        {
            var idToRemove = new List<int>();
            foreach (var aircraft in aircrafts.Values)
            {
                aircraft.CheckToChangePath();
                if (aircraft.NeedToRemove) idToRemove.Add(aircraft.Id);
                else aircraft.Move();
            }
            for (var i = 0; i < idToRemove.Count; i++)
            {
                field.RemoveAircraft(aircrafts[idToRemove[i]].Type, idToRemove[i], removeControl);
            }
        }

        public void RemoveAllAircrafts()
        {
            var planeIds = field.Planes.Keys.ToList();
            for (var i = 0; i < planeIds.Count; i++)
                field.RemoveAircraft(field.Planes[planeIds[i]].Type, planeIds[i], removeControl);

            var umvIds = field.Umvs.Keys.ToList();
            for (var i = 0; i < umvIds.Count; i++)
                field.RemoveAircraft(field.Umvs[umvIds[i]].Type, umvIds[i], removeControl);
        }

        public void PredictAircraftsLocation()
        {
            var futurePlanesLocations = GetFutureAicraftLocations(field.Planes);
            var futureUmvsLocation = GetFutureAicraftLocations(field.Umvs);

            CheckToManeuver(futureUmvsLocation, futurePlanesLocations);
        }

        private Dictionary<int, IThreeDPoint> GetFutureAicraftLocations(Dictionary<int, IAircraft> aircrafts)
        {
            var futureAicraftLocations = new Dictionary<int, IThreeDPoint>();
            foreach (var aircraft in aircrafts.Values)
                futureAicraftLocations.Add(aircraft.Id, aircraft.GetFutureLocation());

            return futureAicraftLocations;
        }


        /// <summary>
        /// Возвращает дистанцию в пикселях между двумя точками на плоскости
        /// </summary>
        /// <param name="pt1">Первая точка</param>
        /// <param name="pt2">Вторая точка</param>
        /// <returns></returns>
        /// Возможно придётся рассчитывать и по третьей оси
        private double GetDistanceBetween(IThreeDPoint pt1, IThreeDPoint pt2)
        {
            var dx = pt2.X - pt1.X;
            var dy = pt2.Y - pt1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void CheckToManeuver(Dictionary<int, IThreeDPoint> umvsFutureLocation, Dictionary<int, IThreeDPoint> planesFutureLocations)
        {
            foreach (var umvFutureLocation in umvsFutureLocation)
                foreach (var planeFutureLocations in planesFutureLocations)
                    if (SetHeightToGain_IfConflict(umvFutureLocation, GetRectangleFromPoints(field.GetRestrZonePoints()), planeFutureLocations.Value))
                        if (field.Umvs[umvFutureLocation.Key].FlightStage == FlightStage.Ordinary)
                            field.Umvs[umvFutureLocation.Key].ChangeFlightStage(FlightStage.Maneuver);
                    //else field.Umvs[futureUmvLocation.Key].ChangeFlightStage(FlightStage.Ordinary);

        }

        private bool SetHeightToGain_IfConflict(KeyValuePair<int, IThreeDPoint> umvFutureLocation, Rectangle restrZone, IThreeDPoint planePoint)
        {
            var umv = field.Umvs[umvFutureLocation.Key];
            if (restrZone.Contains(new Point(umvFutureLocation.Value.X, umvFutureLocation.Value.Y)))
            {
                umv.SetHeightToGain(ProgramOptions.UmvTrackGainHeight[umv.TrackId]);
                return true;
            }
            else if (Convertation.ConvertPixelsToMeters(GetDistanceBetween(umvFutureLocation.Value, planePoint)) < ProgramOptions.ConflictDistance)
            {
                umv.SetHeightToGain(ProgramOptions.UmvTrackGainHeight[umv.TrackId]);
                return true;
            }
            return false;
        }

        private Rectangle GetRectangleFromPoints(List<IThreeDPoint> points)
        {
            return new Rectangle(points[0].X, points[0].Y, points[1].X - points[0].X, points[2].Y - points[0].Y);
        }
    }
}
