using System;
using System.Collections.Generic;

namespace FlightTraining.Model
{
    public class ThreeDPoint : IThreeDPoint
    {
        public int Id { get; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Z { get; private set; }

        public List<double[]> ShiftsData { get; private set; }
        public ThreeDPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            ShiftsData = new List<double[]>();
        }

        public ThreeDPoint(int id, Tuple<int, int, int> point)
        {
            Id = id;
            X = point.Item1;
            Y = point.Item2;
            Z = point.Item3;
            ShiftsData = new List<double[]>();
        }

        public void SetShiftsData(int index, double[] shiftsData)
        {
            var difference = index - (ShiftsData.Count - 1);
            for (var i = 0; i < difference; i++)
                ShiftsData.Add(new double[] { });
            ShiftsData[index] = shiftsData;
        }
        
        public void ChangeCoords(int x = 0, int y = 0, int z = 0)
        {
            X += x;
            Y += y;
            Z += z;
        }

        
        public void ChangeCoords(Tuple<int, int, int> coords)
        {
            X = coords.Item1;
            Y = coords.Item2;
            Z = coords.Item3;
        }
    }
}
