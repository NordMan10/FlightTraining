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

        public Field(IProgramModel model, int width, int height)
        {
            this.model = model;
            Width = width;
            Height = height;
            ProgramOptions.PixelsInCell = width / ProgramOptions.CellsInHorizontal;
            Points = new Points(this);

            Aircrafts = new Dictionary<AircraftType, Dictionary<int, IAircraft>>
            {
                { AircraftType.Plane, new Dictionary<int, IAircraft>() },
                { AircraftType.Umv, new Dictionary<int, IAircraft>() }
            };

            AircraftPaths = new Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, List<Point3D>>>>
            {
                { AircraftFlow.Arrive, new Dictionary<AircraftType, Dictionary<int, List<Point3D>>>() },
                { AircraftFlow.Depurture, new Dictionary<AircraftType, Dictionary<int, List<Point3D>>>() },
                { AircraftFlow.Passing, new Dictionary<AircraftType, Dictionary<int, List<Point3D>>>() }
            };

            LabelsPoints = new Dictionary<Point3D, Label>();
        }

        public IPoints Points { get; }

        public Dictionary<AircraftType, Dictionary<int, IAircraft>> Aircrafts { get; private set; }

        public Dictionary<Point3D, Label> LabelsPoints { get; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, List<Point3D>>>> AircraftPaths { get; private set; }


        public void AddAircraft(AircraftType type, AircraftFlow flow, int trackId, Action<Control> addControl)
        {
            var aircrafts = Aircrafts[type];

            var id = 0;
            var entryTime = $"{Math.Round(model.GetStopwatchElapsedTime().TotalSeconds)}";

            if (aircrafts.Count > 0) id = aircrafts[aircrafts.Keys.Max(key => key)].Id + 1;

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
            Dictionary<AircraftType, List<List<Point3D>>> points)
        {
            foreach (var airflowTrackSets in airflowsTrackSets)
            {
                var aircraftTypePaths = new Dictionary<AircraftType, Dictionary<int, List<Point3D>>>();
                foreach (var aircraftTrackSets in airflowTrackSets.Value)
                {
                    var paths = new Dictionary<int, List<Point3D>>();
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

        public List<Point3D> GetPath(AircraftType type, KeyValuePair<int, int?[][]> trackSets, 
            Dictionary<AircraftType, List<List<Point3D>>> points)
        {
            var path = new List<Point3D>();
            var trackSet = trackSets.Value;

            for (var trackIndex = 0; trackIndex < trackSet.Length; trackIndex++)
            {
                if (trackSet[trackIndex] == null) continue;

                var track = trackSet[trackIndex];

                foreach (var pointId in track)
                {
                    var oneTypePoints = points[type];
                    path.Add(oneTypePoints[trackIndex][(int)pointId]);
                }
            }
            return path;
        }

        private void CalcShiftData(Dictionary<AircraftFlow, Dictionary<AircraftType, Dictionary<int, List<Point3D>>>> aircraftsFlows)
        {
            foreach (var aircraftsFlow in aircraftsFlows.Values)
            foreach (var paths in aircraftsFlow.Values)
                foreach (var path in paths)
                    for (var i = path.Value.Count - 1; i > 0; i--)
                        path.Value[i - 1].SetShiftsData(path.Key, GetAngles(path.Value[i - 1], path.Value[i]));
        }

        private double[] GetAngles(Point3D startPoint, Point3D endPoint)
        {
            double dx = endPoint.X - startPoint.X;
            double dy = endPoint.Y - startPoint.Y;
            double dz = endPoint.Z - startPoint.Z;

            var hypotenuseH = Math.Sqrt(dx * dx + dy * dy);

            var angleH = Math.Atan(dy / dx);

            if (dx == 0) 
                angleH = Math.PI / 2 * Math.Sign(dy);
            else if (dx < 0)
                angleH += Math.PI;

            var angleV = Math.Atan(dz / hypotenuseH);

            return new[] { angleH, angleV };
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

        public List<Point3D> GetRestrAreaPoints()
        {
            return Points.AreaPoints[AreaPointsType.RestrictedArea];
        }

        public void AddLabelsPoints(Action<Control> addControl)
        {
            CreateLabelsPoints();

            foreach (var label in LabelsPoints.Values)
                addControl(label);
        }

        public void UpdateLabelsLocation()
        {
            foreach (var labelSet in LabelsPoints)
            {
                labelSet.Value.Location = new Point(labelSet.Key.X, labelSet.Key.Y);
            }
        }

        private Label SetLabel(Point location, string text)
        {
            return new Label
            {
                Width = 70,
                Height = 30,
                Location = location,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Roboto", 11F, FontStyle.Bold, GraphicsUnit.Point, 204),
                BackColor = Color.Transparent
            };
        }

        private void CreateLabelsPoints()
        {
            foreach (var points in Points.AreaPoints)
            {
                foreach (var point in points.Value)
                {
                    var location = new Point(point.X, point.Y);
                    var text = GetAreaPointsLabelText(points.Key, point.Id);

                    LabelsPoints.Add(point, SetLabel(location, text));
                }
            }

            var pointsGroups = SplitNavPointsByGroups();

            foreach (var pointsGroupSet in pointsGroups)
            {
                var points = pointsGroupSet.Value;

                for (var i = 0; i < points.Count; i++)
                {
                    var location = new Point(points[i].X, points[i].Y);
                    var text = GetNavPointsLabelText(pointsGroupSet.Key, i + 1);

                    LabelsPoints.Add(points[i], SetLabel(location, text));
                }
            }
        }

        private static string GetAreaPointsLabelText(AreaPointsType type, int id)
        {
            return type == AreaPointsType.RestrictedArea ? $"PZ{id + 1:000}" : "";
        }

        private static string GetNavPointsLabelText(AircraftType type, int number)
        {
            return string.Format(type == AircraftType.Plane ? "SS{0:000}" : "UM{0:000}", number);
        }

        private Dictionary<AircraftType, List<Point3D>> SplitNavPointsByGroups()
        {
            var result = new Dictionary<AircraftType, List<Point3D>>();
            var planePoints = new List<Point3D>();
            var umvPoints = new List<Point3D>();

            foreach (var pointSet in Points.NavigationPoints)
            {
                if (pointSet.Key == NavigationPointsType.StartPlanePoints ||
                    pointSet.Key == NavigationPointsType.FinishPlanePoints)
                    planePoints.AddRange(pointSet.Value);
                else
                    umvPoints.AddRange(pointSet.Value);
            }

            result.Add(AircraftType.Plane, planePoints);
            result.Add(AircraftType.Umv, umvPoints);

            return result;
        }

    }
}
