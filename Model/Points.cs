using System;
using System.Collections.Generic;
using System.Drawing;
using FlightTraining.Model.Enums;


namespace FlightTraining.Model
{
    public class Points : IPoints
    {
        private readonly IField field;

        public Points(IField field)
        {
            this.field = field;
            InitAllPoints();
        }

        public Dictionary<LayoutPointsType, List<Tuple<Point, Point>>> LayoutPoints { get; private set; }

        public Dictionary<AreaPointsType, List<Point3D>> AreaPoints { get; private set; }

        public Dictionary<NavigationPointsType, List<Point3D>> NavigationPoints { get; private set; }

        public Dictionary<AircraftType, List<List<Point3D>>> AircraftsPoints { get; private set; }

        public void InitAllPoints(bool isUpdate = false)
        {
            InitLayoutPoints();
            InitRestrictedAreaPoints(isUpdate);
            InitNavigationPoints(isUpdate);
            InitAircraftsPoints();
        }

        private void InitLayoutPoints()
        {

            LayoutPoints = new Dictionary<LayoutPointsType, List<Tuple<Point, Point>>>
            {
                {LayoutPointsType.XAxis, new List<Tuple<Point, Point>>()},
                {LayoutPointsType.YAxis, new List<Tuple<Point, Point>>()}
            };

            SetLayoutCoords();
        }

        private void SetLayoutCoords()
        {
            LayoutPoints[LayoutPointsType.XAxis].Clear();

            for (var i = 1; i < field.Width / ProgramOptions.PixelsInCell + 1; i++)
            {
                LayoutPoints[LayoutPointsType.XAxis].Add(Tuple.Create(new Point(i * ProgramOptions.PixelsInCell, 0),
                    new Point(i * ProgramOptions.PixelsInCell, field.Height)));
            }

            LayoutPoints[LayoutPointsType.YAxis].Clear();

            for (var i = 1; i < field.Height / ProgramOptions.PixelsInCell + 1; i++)
            {
                LayoutPoints[LayoutPointsType.YAxis].Add(Tuple.Create(new Point(0, i * ProgramOptions.PixelsInCell),
                    new Point(field.Width, i * ProgramOptions.PixelsInCell)));
            }
        }

        private void InitRestrictedAreaPoints(bool isUpdate)
        {
            if (!isUpdate)
                AreaPoints = new Dictionary<AreaPointsType, List<Point3D>>()
                {
                    {
                        AreaPointsType.RestrictedArea,
                        new List<Point3D>
                        {
                            new Point3D(0),
                            new Point3D(2),
                            new Point3D(3),
                            new Point3D(1)
                        }
                    }
                };

            AreaPoints[AreaPointsType.RestrictedArea][0]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(1000, 1000, 8000));
            AreaPoints[AreaPointsType.RestrictedArea][1]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(1000, 1000, 13000));
            AreaPoints[AreaPointsType.RestrictedArea][2]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(-1000, 1000, 13000));
            AreaPoints[AreaPointsType.RestrictedArea][3]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(-1000, 1000, 8000));
        }

        /// <summary>
        /// Координаты записываем в таком порядке: x, y, z. То есть по схеме: сначала вертикаль, потом высота и потом горизонталь
        /// </summary>
        /// <param name="isUpdate"></param>
        private void InitNavigationPoints(bool isUpdate)
        {
            if (!isUpdate)
                NavigationPoints = new Dictionary<NavigationPointsType, List<Point3D>>
                {
                    {
                        NavigationPointsType.StartPlanePoints,
                        new List<Point3D> {new Point3D(0), new Point3D(1), new Point3D(2), new Point3D(3)}
                    },
                    {
                        NavigationPointsType.StartUmvPoints,
                        new List<Point3D>
                        {
                            new Point3D(0)
                        }
                    },
                    {
                        NavigationPointsType.FinishPlanePoints,
                        new List<Point3D>
                        {
                            new Point3D(0),
                            new Point3D(1),
                            new Point3D(2),
                            new Point3D(3)

                        }
                    },
                    {
                        NavigationPointsType.FinishUmvPoints,
                        new List<Point3D>
                        {
                            new Point3D(0),
                            new Point3D(1)
                        }
                    },
                    {
                        NavigationPointsType.IntermediateUmvPoints,
                        new List<Point3D>
                        {
                            new Point3D(0),
                            new Point3D(1)

                        }
                    },
                };

            NavigationPoints[NavigationPointsType.StartPlanePoints][0]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(0, 800, 0));
            NavigationPoints[NavigationPointsType.StartPlanePoints][1]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(0, 10, 13000));
            NavigationPoints[NavigationPointsType.StartPlanePoints][2]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(3000, 800, 0));
            NavigationPoints[NavigationPointsType.StartPlanePoints][3]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(-3000, 800, 0));

            NavigationPoints[NavigationPointsType.StartUmvPoints][0]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(5000, 800, 7000));

            NavigationPoints[NavigationPointsType.FinishPlanePoints][0]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(0, 10, 8000));
            NavigationPoints[NavigationPointsType.FinishPlanePoints][1]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(3000, 800, 22000));
            NavigationPoints[NavigationPointsType.FinishPlanePoints][2]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(0, 800, 22000));
            NavigationPoints[NavigationPointsType.FinishPlanePoints][3]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(-3000, 800, 22000));

            NavigationPoints[NavigationPointsType.FinishUmvPoints][0]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(-4000, 800, 3000));
            NavigationPoints[NavigationPointsType.FinishUmvPoints][1]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(-4000, 800, 14000));

            NavigationPoints[NavigationPointsType.IntermediateUmvPoints][0]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(3000, 800, 14000));
            NavigationPoints[NavigationPointsType.IntermediateUmvPoints][1]
                .UpdateCoords(Convertation.TransformCoordsFromSchemeToProgram(2000, 800, 20000));
        }

        private void InitAircraftsPoints()
        {
            var planePoints = new List<List<Point3D>>();
            planePoints.Add(NavigationPoints[NavigationPointsType.StartPlanePoints]);
            planePoints.Add(NavigationPoints[NavigationPointsType.FinishPlanePoints]);

            var umvPoints = new List<List<Point3D>>();
            umvPoints.Add(NavigationPoints[NavigationPointsType.StartUmvPoints]);
            umvPoints.Add(NavigationPoints[NavigationPointsType.IntermediateUmvPoints]);
            umvPoints.Add(NavigationPoints[NavigationPointsType.FinishUmvPoints]);

            AircraftsPoints = new Dictionary<AircraftType, List<List<Point3D>>>();
            AircraftsPoints.Add(AircraftType.Plane, planePoints);
            AircraftsPoints.Add(AircraftType.Umv, umvPoints);
        }

        public void UpdateAllPointsCoords()
        {
            InitAllPoints(true);
        }
    }
}