using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlightTraining.Model
{
    public static class ProgramOptions
    {
        public static readonly int MetersInCell = 1000;

        public static readonly Dictionary<int, int?[]> PlaneTracks = new Dictionary<int, int?[]>()
        {
            { 0, new int?[] { 0, 0 } }
        };

        public static readonly Dictionary<int, int?[]> UmvTracks = new Dictionary<int, int?[]>()
        {
            { 0, new int?[] { 0, null, 0 } },
            { 1, new int?[] { 0, 0, 0 } }
        };

        public static readonly int CellsInHorizontal = 17;

        public static readonly int CellsInVertical = 12;

        public static readonly int PlaneVelocity = 100;

        public static readonly int UmvVelocity = 50;

        public static readonly int TopFieldBorder = 6000;

        public static readonly Tuple<int, int> AircraftInterval = Tuple.Create(20000, 30000);

        public static readonly int GraphicTimerInterval = 250;

        public static readonly int ClockTimerInterval = 10;

        public static readonly int PlaneImageSize = 40;

        public static readonly int UmvImageSize = 30;

        public static readonly int RectPointDiameter = 8;

        public static readonly int XShiftForConvertation = 1000;

        public static readonly int CloseDistance = 3;

        public static readonly double TimeCoafficient = 1000 / GraphicTimerInterval;

        public static int PixelsInCell = 0;

        public static Random Random = new Random();

        public static int PredictInterval = 45;

        public static Dictionary<int, int> UmvTrackGainHeight = new Dictionary<int, int>()
        {
            { 0, 1100 },
            { 1, 1300 }
        };

        public static int TimeToGainHeight = 40;

        public static int ConflictDistance = 2000;
    }
}
