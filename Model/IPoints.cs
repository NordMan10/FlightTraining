using FlightTraining.Model.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace FlightTraining.Model
{
    public interface IPoints
    {
        Dictionary<LayoutPointsType, List<Tuple<Point, Point>>> LayoutPoints { get; }

        Dictionary<AreaPointsType, List<IThreeDPoint>> AreaPoints { get; }

        Dictionary<AircraftType, List<Dictionary<int, IThreeDPoint>>> AircraftsPoints { get; }

        void UpdateAllPointsCoords();

        Label SetLabel(Point location, string text);

        void CreatePointsLabels(Dictionary<IThreeDPoint, Label> pointsLabels);

    }
}
