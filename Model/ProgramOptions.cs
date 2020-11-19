using System;

namespace FlightTraining.Model
{
    public static class ProgramOptions
    {
        public static readonly int MetersInCell = 1000;

        public static readonly int CellsInHorizontal = 23;

        public static readonly int CellsInVertical = 12;

        public static readonly int TopFieldBorder = 6000;

        public static readonly int GraphicTimerInterval = 250;

        //public static readonly int ClockTimerInterval = 10;

        public static readonly int RectPointDiameter = 8;

        public static readonly int XShiftForConvertation = 1000;

        public static readonly double TimeCoefficient = 1000 / GraphicTimerInterval;

        public static int PixelsInCell = 0;

        public static Random Random = new Random();
    }
}
