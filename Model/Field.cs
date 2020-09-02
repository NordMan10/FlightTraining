using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FlightTraining.Model.Enums;

namespace FlightTraining.Model
{
    public class Field : IField
    {
        private readonly IProgramModel model;

        public Action<Control> AddControl { get; set; }

        public Field(IProgramModel model_, int width, int height)
        {
            model = model_;
            Width = width;
            Height = height;
            ProgramOptions.PixelsInCell = width / ProgramOptions.CellsInHorizontal;
            Points = new Points(this);

            Aircrafts = new Dictionary<AircraftType, Dictionary<int, IAircraft>>
            {
                { AircraftType.Plane, new Dictionary<int, IAircraft>() },
                { AircraftType.Umv, new Dictionary<int, IAircraft>() }
            };

            AircraftPaths = new Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>>
            {
                { AircraftFlow.Arrive, new Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>() },
                { AircraftFlow.Depurture, new Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>() },
                { AircraftFlow.Passing, new Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>() }
            };

            PointsLabels = new Dictionary<IThreeDPoint, Label>();
        }

        public IPoints Points { get; private set; }

        public Dictionary<AircraftType, Dictionary<int, IAircraft>> Aircrafts { get; private set; }

        public Dictionary<IThreeDPoint, Label> PointsLabels { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>> AircraftPaths { get; private set; }


        public void AddAircraft(AircraftType type, AircraftFlow flow, int trackId, Action<Control> addControl)
        {
            var aircrafts = Aircrafts[type];

            var id = 0;
            var entryTime = string.Format("{0}", Math.Round(model.GetStopwatchElapsedTime().TotalSeconds));

            if (aircrafts.Count > 0) id = aircrafts[aircrafts.Keys.Max(key => key)].Id + 1;
            //else id = 0;

            var name = AircraftOptions.Names[type];
            var velocity = AircraftOptions.AircraftVelocities[type];
            var image = new Bitmap(AircraftOptions.ImagePaths[type]);
            var path = AircraftPaths[flow][type][trackId];

            aircrafts.Add(id, new Aircraft(type, id, name, velocity, entryTime, image,
                AircraftOptions.AircraftsImageSizes[type], trackId, path));
            addControl(aircrafts[aircrafts.Keys.Max(key => key)].InfoForm);
            aircrafts.Last().Value.InfoForm.BringToFront();
        }

        public void RemoveAircraft(AircraftType type, int id, Action<Control> removeControl)
        {
            var aircrafts = Aircrafts[type];
            removeControl(aircrafts[id].InfoForm);
            aircrafts.Remove(id);
        }

        public void CreatePaths()
        {
            AircraftPaths[AircraftFlow.Arrive].Clear();
            AircraftPaths[AircraftFlow.Depurture].Clear();
            AircraftPaths[AircraftFlow.Passing].Clear();
            CreatePath(AircraftOptions.AircraftsTracks, Points.AircraftsPoints);
            CalcShiftData(AircraftPaths);
        }

        private void CreatePath(Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, int?[][]>>> airflowsTrackSets, 
            Dictionary<AircraftType, List<Dictionary<int, IThreeDPoint>>> points)
        {
            foreach (var airflowTrackSets in airflowsTrackSets)
            {
                var aircraftTypePaths = new Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>();
                foreach (var aircraftTrackSets in airflowTrackSets.Value)
                {
                    var paths = new Dictionary<int, List<IThreeDPoint>>();
                    foreach (var trackSet in aircraftTrackSets.Value)
                    {
                        var path = GetPath(aircraftTrackSets.Key, trackSet, points);
                        paths.Add(trackSet.Key, path);
                    }
                    aircraftTypePaths.Add(aircraftTrackSets.Key, paths);
                }
                AircraftPaths[airflowTrackSets.Key] = aircraftTypePaths;
            }
        }

        public List<IThreeDPoint> GetPath(AircraftType type, KeyValuePair<int, int?[][]> trackSet, 
            Dictionary<AircraftType, List<Dictionary<int, IThreeDPoint>>> points)
        {
            var path = new List<IThreeDPoint>();
            for (var trackPointSetIndex = 0; trackPointSetIndex < trackSet.Value.Length; trackPointSetIndex++)
            {
                if (trackSet.Value[trackPointSetIndex] == null) continue;
                var pointsIdsSet = trackSet.Value[trackPointSetIndex];
                for (var i = 0; i < pointsIdsSet.Length; i++)
                {
                    var collectionList = points[type];
                    var pointId = (int)pointsIdsSet[i];
                    path.Add(collectionList[trackPointSetIndex][pointId]);
                }
            }
            return path;
        }

        private void CalcShiftData(Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, List<IThreeDPoint>>>> aircraftsFlows)
        {
            foreach (var aircraftsFlow in aircraftsFlows.Values)
            foreach (var paths in aircraftsFlow.Values)
                foreach (var path in paths)
                    for (var i = path.Value.Count - 1; i > 0; i--)
                        path.Value[i - 1].SetShiftsData(path.Key, GetAngles(path.Value[i - 1], path.Value[i]));
        }

        private double[] GetAngles(IThreeDPoint startPoint, IThreeDPoint endPoint)
        {
            double dx = endPoint.X - startPoint.X;
            double dy = endPoint.Y - startPoint.Y;
            double dz = endPoint.Z - startPoint.Z;
            var hypotenuseH = Math.Sqrt(dx * dx + dy * dy);
            double angleH;
            if (dx == 0) angleH = Math.PI / 2 * (dy / Math.Abs(dy));
            else
            {
                angleH = Math.Atan(dy / dx);
                if (dx < 0)
                    angleH += Math.PI;
            }

            var angleV = Math.Atan(dz / hypotenuseH);

            return new double[] { angleH, angleV };
        }

        public void UpdateCoords(int newWidth, int newHeight)
        {
            Width = newWidth;
            Height = newHeight;

            ProgramOptions.PixelsInCell = Math.Min(Width / ProgramOptions.CellsInHorizontal, Height / ProgramOptions.CellsInVertical);

            SaveOldAircraftPaths();
            Points.UpdateAllPointsCoords();
            UpdateAircraftLocAndShifts();
        }

        private void SaveOldAircraftPaths()
        {
            foreach (var aircrafts in Aircrafts.Values)
                foreach (var aircraft in aircrafts.Values)
                    aircraft.SaveOldPath();
        }

        private void UpdateAircraftLocAndShifts()
        {
            foreach (var aircrafts in Aircrafts.Values)
                foreach (var aircraft in aircrafts.Values)
                    aircraft.UpdateLocationAndShifts();
        }

        public List<IThreeDPoint> GetRestrZonePoints()
        {
            return Points.AreaPoints[AreaPointsType.RestrictedArea];
        }

        public void AddPointsLabels(Action<Control> addControl)
        {
            Points.CreatePointsLabels(PointsLabels);

            foreach (var label in PointsLabels.Values)
            {
                addControl(label);
            }
        }

        public void UpdateLabelsLocation()
        {
            foreach (var labelSet in PointsLabels)
            {
                labelSet.Value.Location = new Point(labelSet.Key.X, labelSet.Key.Y);
            }
        }

    }
}
