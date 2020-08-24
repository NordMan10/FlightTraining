using System;
using System.Collections.Generic;
using System.Drawing;

namespace FlightTraining.Model
{
    public interface IPoints
    {
        List<Tuple<Point, Point>> XLayoutPoints { get; }

        List<Tuple<Point, Point>> YLayoutPoints { get; }

        List<IThreeDPoint> RestrictedArea { get; }

        Dictionary<int, IThreeDPoint> StartPlanePoints { get; }

        Dictionary<int, IThreeDPoint> StartUmvPoints { get; }

        Dictionary<int, IThreeDPoint> FinishPlanePoints { get; }

        Dictionary<int, IThreeDPoint> OtherUmvPoints { get; } // при данной задаче решение: оставшуюся точку сбросить в один список, 
                                                              // сработает, но при добавлении других точек, надо будет что-то делать
        Dictionary<int, IThreeDPoint> FinishUmvPoints { get; }

        List<Dictionary<int, IThreeDPoint>> PlanePoints { get; }

        List<Dictionary<int, IThreeDPoint>> UmvPoints { get; }

        void UpdateLayoutCoords();

        void UpdateAircraftPointsCoords(int dw, int dh);

        void InitAllPoints();

    }
}
