using FlightTraining.Model.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlightTraining.Model
{
    public interface IField
    {
        IPoints Points { get; }

        int Width { get; }

        int Height { get; }

        Dictionary<AircraftType, Dictionary<int, IAircraft>> Aircrafts { get; }

        Dictionary<IThreeDPoint, Label> PointsLabels { get; }

        void AddAircraft(AircraftType type, AircraftFlow flow, int trackId, Action<Control> addControl);

        void RemoveAircraft(AircraftType type, int id, Action<Control> removeControl);

        void UpdateCoords(int newWidth, int newHeight);

        void CreatePaths();

        List<IThreeDPoint> GetRestrZonePoints();

        List<IThreeDPoint> GetPath(AircraftType type, KeyValuePair<int, int?[][]> trackSet,
            Dictionary<AircraftType, List<Dictionary<int, IThreeDPoint>>> points);

        void AddPointsLabels(Action<Control> addControl);

        void UpdateLabelsLocation();

    }
}
