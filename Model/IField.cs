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

        Dictionary<Point3D, Label> LabelsPoints { get; }

        void AddAircraft(AircraftType type, AircraftFlow flow, int trackId, Action<Control> addControl);

        void RemoveAircraft(AircraftType type, int id, Action<Control> removeControl);

        void UpdateCoords(int newWidth, int newHeight);

        void CreatePaths();

        List<Point3D> GetRestrAreaPoints();

        List<Point3D> GetPath(AircraftType type, KeyValuePair<int, int?[][]> trackSets,
            Dictionary<AircraftType, List<List<Point3D>>> points);

        void AddLabelsPoints(Action<Control> addControl);

        void UpdateLabelsLocation();

    }
}
