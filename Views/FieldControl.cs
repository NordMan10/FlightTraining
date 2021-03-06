﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FlightTraining.Model;
using System.Drawing;
using FlightTraining.Model.Enums;

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

            ArrivePlaneTimer = new Timer();
            DepPlaneTimer = new Timer();
            UmvTimer = new Timer();

            InitTimers();
        }

        public Timer ArrivePlaneTimer { get; private set; }

        public Timer DepPlaneTimer { get; private set; }

        public Timer UmvTimer { get; private set; }

        #region Timers

        private void InitTimers()
        {
            ArrivePlaneTimer = new Timer();
            DepPlaneTimer = new Timer();
            UmvTimer = new Timer();
            ArrivePlaneTimer.Interval = GetRandomTimerInterval(AircraftFlow.Arrive);
            DepPlaneTimer.Interval = GetRandomTimerInterval(AircraftFlow.Depurture);
            UmvTimer.Interval = GetRandomTimerInterval(AircraftFlow.Passing);
            ArrivePlaneTimer.Tick += ArrivePlaneTimer_Tick;
            DepPlaneTimer.Tick += DepPlaneTimer_Tick;
            UmvTimer.Tick += UmvTimer_Tick;
        }

        private void ArrivePlaneTimer_Tick(object sender, EventArgs e)
        {
            AircraftTimer_TickHandler(AircraftType.Plane, AircraftFlow.Arrive, 
                AircraftOptions.AircraftsTracks[AircraftFlow.Arrive][AircraftType.Plane].Count, ArrivePlaneTimer);
        }

        private void DepPlaneTimer_Tick(object sender, EventArgs e)
        {
            AircraftTimer_TickHandler(AircraftType.Plane, AircraftFlow.Depurture, 
                AircraftOptions.AircraftsTracks[AircraftFlow.Depurture][AircraftType.Plane].Count, DepPlaneTimer);
        }

        private void UmvTimer_Tick(object sender, EventArgs e)
        {
            AircraftTimer_TickHandler(AircraftType.Umv, AircraftFlow.Passing, 
                AircraftOptions.AircraftsTracks[AircraftFlow.Passing][AircraftType.Umv].Count, UmvTimer);
        }

        private void AircraftTimer_TickHandler(AircraftType type, AircraftFlow flow, int tracksCount, Timer timer)
        {
            var randomTrackId = GetRandomTrackId(0, tracksCount);
            field.AddAircraft(type, flow, randomTrackId, addControl);
            timer.Interval = GetRandomTimerInterval(flow);
        }

        public int GetRandomTrackId(int leftBorder, int rightBorder)
        {
            return ProgramOptions.Random.Next(leftBorder, rightBorder);
        }

        private int GetRandomTimerInterval(AircraftFlow flow)
        {
            return ProgramOptions.Random.Next(AircraftOptions.AircraftInterval[flow].Item1, 
                AircraftOptions.AircraftInterval[flow].Item2);
        }

        #endregion 

        public void AddFirstAircrafts()
        {
            field.AddAircraft(AircraftType.Plane, AircraftFlow.Arrive, GetRandomTrackId(0, 
                AircraftOptions.AircraftsTracks[AircraftFlow.Arrive][AircraftType.Plane].Count - 1), addControl);
            field.AddAircraft(AircraftType.Plane, AircraftFlow.Depurture, GetRandomTrackId(0, 
                AircraftOptions.AircraftsTracks[AircraftFlow.Depurture][AircraftType.Plane].Count - 1), addControl);
            field.AddAircraft(AircraftType.Umv, AircraftFlow.Passing, GetRandomTrackId(0, 
                AircraftOptions.AircraftsTracks[AircraftFlow.Passing][AircraftType.Umv].Count), addControl); 
        }

        private void FieldControl_Resize(object sender, EventArgs e)
        {
            if (configured)
            {
                field.UpdateCoords(Width, Height);
                field.CreatePaths();
                field.UpdateLabelsLocation();
                Invalidate();
            }
        }

        public void Configure(IProgramModel model_)
        {
            if (configured) return;
            model = model_;
            field = new Field(model, Width, Height);

            field.AddLabelsPoints(addControl);

            Invalidate();
            configured = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            e.Graphics.DrawLayout(field.Points.LayoutPoints[LayoutPointsType.XAxis], 
                field.Points.LayoutPoints[LayoutPointsType.YAxis]);
            e.Graphics.DrawRestrZone(field.Points.AreaPoints[AreaPointsType.RestrictedArea]);
            
            var aircraftTrajectories = GetTrajectoryPoints(AircraftOptions.AircraftsTracks, 
                field.Points.AircraftsPoints);
            e.Graphics.DrawAircraftTrajectories(aircraftTrajectories);
            
            var planePoints = GetAircraftPoints(field.Points.AircraftsPoints[AircraftType.Plane]);
            e.Graphics.DrawAircraftPoints(planePoints, GraphicsExtensions.PlanePointsBrush);

            var umvPoints = GetAircraftPoints(field.Points.AircraftsPoints[AircraftType.Umv]);
            e.Graphics.DrawAircraftPoints(umvPoints, GraphicsExtensions.UmvPointsBrush);
            
            e.Graphics.DrawAircrafts(field.Aircrafts);

            e.Graphics.DrawFuturePoints(field.Aircrafts);
        }

        private List<Point3D> GetAircraftPoints(List<List<Point3D>> aircraftPoints)
        {
            return aircraftPoints.SelectMany(points => points).ToList();
        }

        private List<Tuple<AircraftType, Point, Point>> GetTrajectoryPoints(Dictionary<AircraftFlow, 
                Dictionary<AircraftType, Dictionary<int, int?[][]>>>airflowsTrackSets,
                Dictionary<AircraftType, List<List<Point3D>>> points)
        {
            var result = new List<Tuple<AircraftType, Point, Point>>();

            foreach (var airflowTrackSets in airflowsTrackSets)
            {
                var path = new List<Point3D>();
                foreach (var aircraftTrackSet in airflowTrackSets.Value)
                {
                    var type = aircraftTrackSet.Key;
                    foreach (var trackSet in aircraftTrackSet.Value)
                    {
                        path = field.GetPath(type, trackSet, points);
                        FillResult(type, ref result, path);
                    }
                }
            }
                
            return result;
        }

        private void FillResult(AircraftType type, 
            ref List<Tuple<AircraftType, Point, Point>> result, List<Point3D> path)
        {
            for (var i = 0; i < path.Count - 1; i++)
            {
                result.Add(Tuple.Create(type, 
                    new Point(path[i].X, path[i].Y), new Point(path[i + 1].X, path[i + 1].Y)));
            }
        }

        public void StartTimers()
        {
            ArrivePlaneTimer.Start();
            DepPlaneTimer.Start();
            UmvTimer.Start();
        }

        public void StopTimers()
        {   
            ArrivePlaneTimer.Stop();
            DepPlaneTimer.Stop();
            UmvTimer.Stop();
        }

        /// <summary>
        /// Меняет координаты всех ВС на соответствующие смещения. ВС в свою 
        /// очередь меняют координаты своих формуляров.
        /// </summary>
        public void MoveAllAircrafts()
        {
            UpdateAircrafts();
        }

        private void UpdateAircrafts()
        {
            var aircraftToRemove = new List<IAircraft>();
            foreach (var aircrafts in field.Aircrafts.Values)
                foreach (var aircraft in aircrafts.Values)
                {
                    aircraft.CheckToChangePath();
                    if (aircraft.GetFlightStage() == FlightStage.NeedToRemove) aircraftToRemove.Add(aircraft);
                    else aircraft.Move();
                }
                for (var i = 0; i < aircraftToRemove.Count; i++)
                    field.RemoveAircraft(aircraftToRemove[i].Type, aircraftToRemove[i].Id, removeControl);
        }

        public void RemoveAllAircrafts()
        {
            foreach (var aircrafts in field.Aircrafts.Values)
            {
                var planeIds = aircrafts.Keys.ToList();
                for (var i = 0; i < planeIds.Count; i++)
                    field.RemoveAircraft(aircrafts[planeIds[i]].Type, planeIds[i], removeControl);
            }
        }

        public void PredictAircraftsLocation()
        {
            var futurePlanesLocations = GetFutureAicraftLocations(field.Aircrafts[AircraftType.Plane]);
            var futureUmvsLocation = GetFutureAicraftLocations(field.Aircrafts[AircraftType.Umv]);

            CheckToManeuver(futureUmvsLocation, futurePlanesLocations);
        }

        private Dictionary<int, Point3D> GetFutureAicraftLocations(Dictionary<int, IAircraft> aircrafts)
        {
            var futureAicraftLocations = new Dictionary<int, Point3D>();
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
        private double GetDistanceBetween(Point3D pt1, Point3D pt2)
        {
            var dx = pt2.X - pt1.X;
            var dy = pt2.Y - pt1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void CheckToManeuver(Dictionary<int, Point3D> umvsFutureLocation, 
            Dictionary<int, Point3D> planesFutureLocations)
        {
            var umvs = field.Aircrafts[AircraftType.Umv];

            foreach (var umvFutureLocation in umvsFutureLocation)
                foreach (var planeFutureLocations in planesFutureLocations)
                    if (SetHeightToGain_IfConflict(umvFutureLocation, 
                        GetRectangleFromPoints(field.GetRestrAreaPoints()), planeFutureLocations.Value))
                        if (umvs[umvFutureLocation.Key].GetFlightStage() == FlightStage.Ordinary)
                            umvs[umvFutureLocation.Key].ChangeFlightStage(FlightStage.Maneuver);
        }

        private bool SetHeightToGain_IfConflict(KeyValuePair<int, Point3D> umvFutureLocation, 
            Rectangle restrZone, Point3D planePoint)
        {
            var umv = field.Aircrafts[AircraftType.Umv][umvFutureLocation.Key];
            var minDistX = Math.Min(Math.Abs(umvFutureLocation.Value.X - restrZone.Left),
                Math.Abs(umvFutureLocation.Value.X - restrZone.Right));

            if (restrZone.Contains(new Point(umvFutureLocation.Value.X, umvFutureLocation.Value.Y)))
            {
                
                umv.SetHeightToGain(AircraftOptions.UmvTracksGainHeight[umv.TrackId]);
                return true;
            }
            if (Convertation.ConvertPixelsToMeters(
                GetDistanceBetween(umvFutureLocation.Value, planePoint)) < AircraftOptions.ConflictDistance)
            {
                umv.SetHeightToGain(AircraftOptions.UmvTracksGainHeight[umv.TrackId]);
                return true;
            }
            return false;
        }

        private Rectangle GetRectangleFromPoints(List<Point3D> points)
        {
            return new Rectangle(points[0].X, points[0].Y, 
                points[1].X - points[0].X, points[2].Y - points[0].Y);
        }
    }
}
