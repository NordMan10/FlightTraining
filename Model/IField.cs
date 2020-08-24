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

        Dictionary<int, IAircraft> Planes { get; }

        Dictionary<int, IAircraft> Umvs { get; }

        Dictionary<int, List<IThreeDPoint>> PlanePaths { get; }

        Dictionary<int, List<IThreeDPoint>> UmvPaths { get; }

        void AddAircraft(AircraftType type, int trackId, Action<Control> addControl);

        void RemoveAircraft(AircraftType type, int id, Action<Control> removeControl);

        void UpdateCoords(int newWidth, int newHeight);

        void CreatePaths();

        List<IThreeDPoint> GetRestrZonePoints();
    }
}
