using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
            Planes = new Dictionary<int, IAircraft>();
            Umvs = new Dictionary<int, IAircraft>();
            PlanePaths = new Dictionary<int, List<IThreeDPoint>>();
            UmvPaths = new Dictionary<int, List<IThreeDPoint>>();
            CreatePaths();
        }

        public IPoints Points { get; private set; }

        public Dictionary<int, IAircraft> Planes { get; private set; }

        public Dictionary<int, IAircraft> Umvs { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Dictionary<int, List<IThreeDPoint>> PlanePaths { get; private set; }

        public Dictionary<int, List<IThreeDPoint>> UmvPaths { get; private set; }

        public void AddAircraft(AircraftType type, int trackId, Action<Control> addControl)
        {
            int id;
            string name;
            int velocity;
            string entryTime = string.Format("{0}", Math.Round(model.Stopwatch.Elapsed.TotalSeconds));
            Image image;
            var path = new List<IThreeDPoint>();

            if (type == AircraftType.Plane)
            {
                if (Planes.Count > 0) id = Planes[Planes.Keys.Max(key => key)].Id + 1;
                else id = 0;
                name = "A";
                velocity = ProgramOptions.PlaneVelocity;
                image = new Bitmap(@"images\plane.png");
                path = PlanePaths[trackId];

                Planes.Add(id, new Aircraft(type, id, name, velocity, entryTime, image, 
                    ProgramOptions.PlaneImageSize, trackId, path, ProgramOptions.PixelsInCell));
                addControl(Planes[Planes.Keys.Max(key => key)].InfoForm);
            }
            else
            {
                if (Umvs.Count > 0) id = Umvs[Umvs.Keys.Max(key => key)].Id + 1;
                else id = 0;
                name = "UMV";
                velocity = ProgramOptions.UmvVelocity;
                image = new Bitmap(@"images\quadrocopter.png");
                path = UmvPaths[trackId];

                Umvs.Add(id, new Aircraft(type, id, name, velocity, entryTime, image, 
                    ProgramOptions.UmvImageSize, trackId, path, ProgramOptions.PixelsInCell));
                addControl(Umvs[Umvs.Keys.Max(key => key)].InfoForm);
            }
        }

        public void RemoveAircraft(AircraftType type, int id, Action<Control> removeControl)
        {
            if (type == AircraftType.Plane)
            {
                removeControl(Planes[id].InfoForm);
                Planes.Remove(id);
            }
            else
            {
                removeControl(Umvs[id].InfoForm);
                Umvs.Remove(id);
            }
        }

        public void CreatePaths()
        {
            PlanePaths.Clear();
            UmvPaths.Clear();
            CreatePath(ProgramOptions.PlaneTracks, Points.PlanePoints, AircraftType.Plane);
            CreatePath(ProgramOptions.UmvTracks, Points.UmvPoints, AircraftType.Umv);
            CalcShiftData(PlanePaths);
            CalcShiftData(UmvPaths);
        }

        private void CreatePath(Dictionary<int, int?[]> trackSets, List<Dictionary<int, IThreeDPoint>> points, AircraftType type)
        {
            foreach (var trackSet in trackSets)
            {
                var path = new List<IThreeDPoint>();
                for (var i = 0; i < trackSet.Value.Length; i++)
                {
                    if (trackSet.Value[i] == null) continue;
                    path.Add(points[i][(int)trackSet.Value[i]]);
                }
                if (type == AircraftType.Plane)
                {
                    PlanePaths.Add(trackSet.Key, path);
                }
                else
                {
                    UmvPaths.Add(trackSet.Key, path);
                }
            }
        }

        private void CalcShiftData(Dictionary<int, List<IThreeDPoint>> paths)
        {
            foreach (var path in paths)
            {
                for (var i = path.Value.Count - 1; i > 0; i--)
                {
                    path.Value[i - 1].SetShiftsData(path.Key, GetAngles(path.Value[i - 1], path.Value[i]));
                }
            }
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
                if (angleH < 0)
                {
                    if (dx < 0) angleH += Math.PI;
                    else angleH += Math.PI / 2;
                }
                else if (angleH > 0)
                    if (dx < 0) angleH += Math.PI;
            }

            var angleV = Math.Atan(dz / hypotenuseH);

            return new double[] { angleH, angleV };
        }

        public void UpdateCoords(int newWidth, int newHeight)
        {
            Width = newWidth;
            Height = newHeight;

            ProgramOptions.PixelsInCell = Math.Min(Width / ProgramOptions.CellsInHorizontal, Height / ProgramOptions.CellsInVertical);

            SaveAircraftOldPaths();
            Points.InitAllPoints();
            UpdateAircraftLocAndShifts();
        }

        private void SaveAircraftOldPaths()
        {
            foreach (var plane in Planes.Values)
                plane.SaveOldPath();
            foreach (var umv in Umvs.Values)
                umv.SaveOldPath();
        }

        private void UpdateAircraftLocAndShifts()
        {
            foreach (var plane in Planes.Values)
                plane.UpdateLocationAndShifts();
            foreach (var plane in Umvs.Values)
                plane.UpdateLocationAndShifts();
        }

        public List<IThreeDPoint> GetRestrZonePoints()
        {
            return Points.RestrictedArea;
        }

    }
}
