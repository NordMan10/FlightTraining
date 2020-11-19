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
        Dictionary<AreaPointsType, List<Point3D>> AreaPoints { get; }
        Dictionary<AircraftType, List<List<Point3D>>> AircraftsPoints { get; }
        Dictionary<NavigationPointsType, List<Point3D>> NavigationPoints { get; }
        void UpdateAllPointsCoords();
    }
}
