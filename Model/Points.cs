using System;
using System.Collections.Generic;
using System.Drawing;
using FlightTraining.Model.Enums;
using System.Windows.Forms;


namespace FlightTraining.Model
{
    public class Points : IPoints
    {
        private readonly IField field;
        public Points(IField field_)
        {
            field = field_;
            InitAllPoints();
        }
        public Dictionary<LayoutPointsType, List<Tuple<Point, Point>>> LayoutPoints { get; private set; }

        public Dictionary<AreaPointsType, List<IThreeDPoint>> AreaPoints { get; private set; }

        public Dictionary<NavigationPointsType, Dictionary<int, IThreeDPoint>> NavigationPoints { get; private set; }

        public Dictionary<AircraftType, List<Dictionary<int, IThreeDPoint>>> AircraftsPoints { get; private set; }


        public void InitAllPoints(bool isUpdate = false) //вернуть private
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
                { LayoutPointsType.XAxis, new List<Tuple<Point, Point>>() },
                { LayoutPointsType.YAxis, new List<Tuple<Point, Point>>() }
            };

            SetLayoutCoords();
        }

        private void SetLayoutCoords()
        {
            LayoutPoints[LayoutPointsType.XAxis].Clear();

            for (var i = 1; i < field.Width / ProgramOptions.PixelsInCell + 1; i++)
            {
                LayoutPoints[LayoutPointsType.XAxis].Add(Tuple.Create(new Point(i * ProgramOptions.PixelsInCell, 0), new Point(i * ProgramOptions.PixelsInCell, field.Height)));
            }

            LayoutPoints[LayoutPointsType.YAxis].Clear();

            for (var i = 1; i < field.Height / ProgramOptions.PixelsInCell + 1; i++)
            {
                LayoutPoints[LayoutPointsType.YAxis].Add(Tuple.Create(new Point(0, i * ProgramOptions.PixelsInCell), new Point(field.Width, i * ProgramOptions.PixelsInCell)));
            }
        }

        private void InitRestrictedAreaPoints(bool isUpdate)
        {
            if (!isUpdate)
                AreaPoints = new Dictionary<AreaPointsType, List<IThreeDPoint>>
                {
                    {
                        AreaPointsType.RestrictedArea,
                        new List<IThreeDPoint>
                        {
                            new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(1000, 1000, 8000)),
                            new ThreeDPoint(2, Convertation.ConvertFromSchemeToProgram(1000, 1000, 13000)),
                            new ThreeDPoint(3, Convertation.ConvertFromSchemeToProgram(-1000, 1000, 13000)),
                            new ThreeDPoint(1, Convertation.ConvertFromSchemeToProgram(-1000, 1000, 8000))
                        }
                    }
                };
            else
            {
                AreaPoints[AreaPointsType.RestrictedArea][0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(1000, 1000, 8000));
                AreaPoints[AreaPointsType.RestrictedArea][1].ChangeCoords(Convertation.ConvertFromSchemeToProgram(1000, 1000, 13000));
                AreaPoints[AreaPointsType.RestrictedArea][2].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-1000, 1000, 13000));
                AreaPoints[AreaPointsType.RestrictedArea][3].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-1000, 1000, 8000));
            }
        }

        /// <summary>
        /// Координаты записываем в таком порядке: x, y, z. То есть по схеме: сначала вертикаль, потом высота и потом горизонталь
        /// </summary>
        /// <param name="isUpdate"></param>
        private void InitNavigationPoints(bool isUpdate)
        {
            if (!isUpdate)
                NavigationPoints = new Dictionary<NavigationPointsType, Dictionary<int, IThreeDPoint>>
                {
                    {
                        NavigationPointsType.StartPlanePoints,
                        new Dictionary<int, IThreeDPoint>
                        {
                            { 0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(0, 800, 0)) },
                            { 1, new ThreeDPoint(1, Convertation.ConvertFromSchemeToProgram(0, 10, 13000)) },
                            { 2, new ThreeDPoint(2, Convertation.ConvertFromSchemeToProgram(3000, 10, 0)) },
                            { 3, new ThreeDPoint(3, Convertation.ConvertFromSchemeToProgram(-3000, 10, 0)) }
                        }
                    },
                    {
                        NavigationPointsType.StartUmvPoints,
                        new Dictionary<int, IThreeDPoint>
                        {
                            { 0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(5000, 800, 7000)) }
                        }
                    },
                    {
                        NavigationPointsType.FinishPlanePoints,
                        new Dictionary<int, IThreeDPoint>
                        {
                            { 0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(0, 10, 8000)) },
                            { 1, new ThreeDPoint(1, Convertation.ConvertFromSchemeToProgram(3000, 800, 22000)) },
                            { 2, new ThreeDPoint(2, Convertation.ConvertFromSchemeToProgram(0, 800, 22000)) },
                            { 3, new ThreeDPoint(3, Convertation.ConvertFromSchemeToProgram(-3000, 800, 22000)) }

                        }
                    },
                    {
                        NavigationPointsType.FinishUmvPoints,
                        new Dictionary<int, IThreeDPoint>
                        {
                            { 0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(-4000, 800, 3000)) },
                            { 1, new ThreeDPoint(1, Convertation.ConvertFromSchemeToProgram(-4000, 800, 14000)) }
                        }
                    },
                    {
                        NavigationPointsType.IntermediateUmvPoints,
                        new Dictionary<int, IThreeDPoint>
                        {
                            { 0, new ThreeDPoint(0, Convertation.ConvertFromSchemeToProgram(3000, 800, 14000)) },
                            { 1, new ThreeDPoint(1, Convertation.ConvertFromSchemeToProgram(2000, 800, 20000)) }

                        }
                    },
                };
            else
            {
                //foreach (var points in NavigationPoints.Values)
                //    foreach (var point in points.Values)
                //    {
                //        var temp = Convertation.ConvertFromProgramToScheme(point.X, point.Y, point.Z);
                //        point.ChangeCoords(Convertation.ConvertFromSchemeToProgram(temp.Item1, temp.Item2, temp.Item3));
                //    }
                NavigationPoints[NavigationPointsType.StartPlanePoints][0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(0, 800, 0));
                NavigationPoints[NavigationPointsType.StartPlanePoints][1].ChangeCoords(Convertation.ConvertFromSchemeToProgram(0, 10, 13000));
                NavigationPoints[NavigationPointsType.StartPlanePoints][2].ChangeCoords(Convertation.ConvertFromSchemeToProgram(3000, 800, 0));
                NavigationPoints[NavigationPointsType.StartPlanePoints][3].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-3000, 800, 0));


                NavigationPoints[NavigationPointsType.StartUmvPoints][0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(5000, 800, 7000));

                NavigationPoints[NavigationPointsType.FinishPlanePoints][0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(0, 10, 8000));
                NavigationPoints[NavigationPointsType.FinishPlanePoints][1].ChangeCoords(Convertation.ConvertFromSchemeToProgram(3000, 800, 22000));
                NavigationPoints[NavigationPointsType.FinishPlanePoints][2].ChangeCoords(Convertation.ConvertFromSchemeToProgram(0, 800, 22000));
                NavigationPoints[NavigationPointsType.FinishPlanePoints][3].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-3000, 800, 22000));


                NavigationPoints[NavigationPointsType.FinishUmvPoints][0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-4000, 800, 3000));
                NavigationPoints[NavigationPointsType.FinishUmvPoints][1].ChangeCoords(Convertation.ConvertFromSchemeToProgram(-4000, 800, 14000));

                NavigationPoints[NavigationPointsType.IntermediateUmvPoints][0].ChangeCoords(Convertation.ConvertFromSchemeToProgram(3000, 800, 14000));
                NavigationPoints[NavigationPointsType.IntermediateUmvPoints][1].ChangeCoords(Convertation.ConvertFromSchemeToProgram(2000, 800, 20000));

            }
        }

        private void InitAircraftsPoints()
        {
            var planePoints = new List<Dictionary<int, IThreeDPoint>>();
            planePoints.Add(NavigationPoints[NavigationPointsType.StartPlanePoints]);
            planePoints.Add(NavigationPoints[NavigationPointsType.FinishPlanePoints]);

            var umvPoints = new List<Dictionary<int, IThreeDPoint>>();
            umvPoints.Add(NavigationPoints[NavigationPointsType.StartUmvPoints]);
            umvPoints.Add(NavigationPoints[NavigationPointsType.IntermediateUmvPoints]);
            umvPoints.Add(NavigationPoints[NavigationPointsType.FinishUmvPoints]);

            AircraftsPoints = new Dictionary<AircraftType, List<Dictionary<int, IThreeDPoint>>>();
            AircraftsPoints.Add(AircraftType.Plane, planePoints);
            AircraftsPoints.Add(AircraftType.Umv, umvPoints);
        }

        public void UpdateAllPointsCoords()
        {
            InitAllPoints(true);
        }

        public Label SetLabel(Point location, string text)
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

        public void CreatePointsLabels(Dictionary<IThreeDPoint, Label> pointsLabels)
        {
            foreach (var points in AreaPoints)
            {
                foreach (var point in points.Value)
                {
                    var location = new Point(point.X, point.Y);
                    var text = GetAreaPointsLabelText(points.Key, point.Id);

                    pointsLabels.Add(point, SetLabel(location, text));
                }
            }

            var pointsGroups = SplitNavPointsByGroups();

            foreach (var pointsGroupSet in pointsGroups)
            {
                var pointsGroup = pointsGroupSet.Value;

                for (var i = 0; i < pointsGroup.Count; i++)
                {
                    var location = new Point(pointsGroup[i].X, pointsGroup[i].Y);
                    var text = GetNavPointsLabelText(pointsGroupSet.Key, i + 1);

                    pointsLabels.Add(pointsGroup[i], SetLabel(location, text));
                }
            }
        }

        //private void FillPointsLabels(PointsType type, List<IThreeDPoint> points, Dictionary<IThreeDPoint, Label> pointsLabels)
        //{
        //    foreach (var point in points)
        //    {
        //        var location = new Point(point.X, point.Y);
        //        var text = GetNavPointsLabelText(points.Key, point.Id);

        //        pointsLabels.Add(point, SetLabel(location, text));
        //    }
        //}

        private string GetAreaPointsLabelText(AreaPointsType type, int id)
        {
            var text = "";
            if (type == AreaPointsType.RestrictedArea)
                text = string.Format("PZ{0:000}", id + 1);
            return text;
        }

        private string GetNavPointsLabelText(AircraftType type, int number)
        {
            string text;
            if (type == AircraftType.Plane)
                text = string.Format("SS{0:000}", number);
            else
            {
                text = string.Format("UM{0:000}", number);
            }
            return text;
        }

        private Dictionary<AircraftType, List<IThreeDPoint>> SplitNavPointsByGroups()
        {
            var result = new Dictionary<AircraftType, List<IThreeDPoint>>();
            var planePoints = new List<IThreeDPoint>();
            var umvPoints = new List<IThreeDPoint>();

            foreach (var pointsSet in NavigationPoints)
            {
                if (pointsSet.Key == NavigationPointsType.StartPlanePoints ||
                    pointsSet.Key == NavigationPointsType.FinishPlanePoints)
                    foreach (var pointSet in pointsSet.Value)
                        planePoints.Add(pointSet.Value);
                else
                    foreach (var pointSet in pointsSet.Value)
                        umvPoints.Add(pointSet.Value);
            }

            result.Add(AircraftType.Plane, planePoints);
            result.Add(AircraftType.Umv, umvPoints);

            return result;
        }
    }
}