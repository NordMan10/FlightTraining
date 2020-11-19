using System;
using System.Collections.Generic;

namespace FlightTraining.Model
{
    public class Point3D
    {
        public int Id { get; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Z { get; private set; }

        public List<double[]> ShiftsData { get; }

        public Point3D(int id)
        {
            Id = id;
            ShiftsData = new List<double[]>();
        }

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            ShiftsData = new List<double[]>();
        }

        public void SetShiftsData(int index, double[] shiftsData)
        {
            var difference = index - (ShiftsData.Count - 1);
            for (var i = 0; i < difference; i++)
                ShiftsData.Add(new double[] { });
            ShiftsData[index] = shiftsData;
        }
        
        public void UpdateCoords(Tuple<int, int, int> coords)
        {
            X = coords.Item1;
            Y = coords.Item2;
            Z = coords.Item3;
        }
    }
}
