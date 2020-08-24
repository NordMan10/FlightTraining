using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace FlightTraining.Model
{
    public class Points : IPoints
    {
        private readonly IField field;
        public Points(IField field)
        {
            this.field = field;
            InitAllPoints();
            configured = true;
        }
        public List<Tuple<Point, Point>> XLayoutPoints { get; private set; }

        public List<Tuple<Point, Point>> YLayoutPoints { get; private set; }

        public List<IThreeDPoint> RestrictedArea { get; private set; }

        public Dictionary<int, IThreeDPoint> StartPlanePoints { get; private set; }

        public Dictionary<int, IThreeDPoint> StartUmvPoints { get; private set; }

        public Dictionary<int, IThreeDPoint> FinishPlanePoints { get; private set; }

        public Dictionary<int, IThreeDPoint> FinishUmvPoints { get; private set; }

        public Dictionary<int, IThreeDPoint> OtherUmvPoints { get; private set; }

        public List<Dictionary<int, IThreeDPoint>> PlanePoints { get; private set; }

        public List<Dictionary<int, IThreeDPoint>> UmvPoints { get; private set; }

        private readonly bool configured = false;

        public void InitAllPoints() //вернуть private
        {
            InitXAndYLayoutPoints();
            InitRestrictedAreaPoints();
            InitStartPlanePoints();
            InitStartUmvPoints();
            InitFinishPlanePoints();
            InitFinishUmvPoints();
            InitOtherUmvPoints();

            SetTrackPoints();
        }

        private void InitXAndYLayoutPoints() 
        {
            XLayoutPoints = new List<Tuple<Point, Point>>();
            YLayoutPoints = new List<Tuple<Point, Point>>();

            SetLayoutCoords();
        }

        private void SetLayoutCoords()
        {
            XLayoutPoints.Clear();
            for (var i = 1; i < field.Width / ProgramOptions.PixelsInCell; i++)
            {
                XLayoutPoints.Add(Tuple.Create(new Point(i * ProgramOptions.PixelsInCell, 0), new Point(i * ProgramOptions.PixelsInCell, field.Height)));
            }

            YLayoutPoints.Clear();
            for (var i = 1; i < field.Height / ProgramOptions.PixelsInCell; i++)
            {
                YLayoutPoints.Add(Tuple.Create(new Point(0, i * ProgramOptions.PixelsInCell), new Point(field.Width, i * ProgramOptions.PixelsInCell)));
            }
        }

        private void InitRestrictedAreaPoints()
        {
            if (!configured)
            { 
                RestrictedArea = new List<IThreeDPoint>();
            
                RestrictedArea.Add(new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(1000, 1000, 10000)));
                RestrictedArea.Add(new ThreeDPoint(2, Convertation.ConvertFromSchemeToProgram(1000, 1000, 15000)));
                RestrictedArea.Add(new ThreeDPoint(3, Convertation.ConvertFromSchemeToProgram(-1000, 1000, 15000)));
                RestrictedArea.Add(new ThreeDPoint(1, Convertation.ConvertFromSchemeToProgram(-1000, 1000, 10000)));
            }
            else
            {
                RestrictedArea[0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(1000, 1000, 10000));
                RestrictedArea[1].ChangeCoords(Convertation.ConvertFromSchemeToProgram(1000, 1000, 15000));
                RestrictedArea[2].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-1000, 1000, 15000));
                RestrictedArea[3].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-1000, 1000, 10000));
            }
        }

        private void InitStartPlanePoints()
        {
            if (!configured)
            {
                StartPlanePoints = new Dictionary<int, IThreeDPoint>();
                StartPlanePoints.Add(0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(0, 800, 0)));
            }
            else
            {
                StartPlanePoints[0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(0, 800, 0));
            }
        }

        private void InitStartUmvPoints()
        {
            if (!configured)
            {
                StartUmvPoints = new Dictionary<int, IThreeDPoint>();
                StartUmvPoints.Add(0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(5000, 800, 7000)));
            }
            else
            {
                StartUmvPoints[0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(5000, 800, 7000));
            }
        }

        private void InitFinishPlanePoints()
        {
            if (!configured)
            {
                FinishPlanePoints = new Dictionary<int, IThreeDPoint>();
                FinishPlanePoints.Add(0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(0, 10, 10000)));
            }
            else
            {
                FinishPlanePoints[0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(0, 10, 10000));
            }
        }

        private void InitFinishUmvPoints()
        {
            if (!configured)
            {
                FinishUmvPoints = new Dictionary<int, IThreeDPoint>();
                FinishUmvPoints.Add(0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(-4000, 800, 3000)));
            }
            else
            {
                FinishUmvPoints[0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-4000, 800, 3000));
            }
        }

        private void InitOtherUmvPoints()
        {
            if (!configured)
            {
                OtherUmvPoints = new Dictionary<int, IThreeDPoint>();
                OtherUmvPoints.Add(0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(3000, 800, 14000)));
            }
            else
            {
                OtherUmvPoints[0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(3000, 800, 14000));
            }
        }

        private void SetTrackPoints()
        {
            PlanePoints = new List<Dictionary<int, IThreeDPoint>>();
            PlanePoints.Add(StartPlanePoints);
            PlanePoints.Add(FinishPlanePoints);

            UmvPoints = new List<Dictionary<int, IThreeDPoint>>();
            UmvPoints.Add(StartUmvPoints);
            UmvPoints.Add(OtherUmvPoints);
            UmvPoints.Add(FinishUmvPoints);
        }

        public void UpdateLayoutCoords() { SetLayoutCoords(); }

        public void UpdateAircraftPointsCoords(int dw, int dh)
        {
            ChangeAircraftPointsCoords(PlanePoints, dw, dh);
            ChangeAircraftPointsCoords(UmvPoints, dw, dh);
        }

        private void ChangeAircraftPointsCoords(List<Dictionary<int, IThreeDPoint>> aircraftPoints, int dw, int dh)
        {
            foreach (var points in aircraftPoints)
                foreach (var point in points.Values)
                {
                    point.ChangeCoords(dw, dh);
                }
        }
    }
}