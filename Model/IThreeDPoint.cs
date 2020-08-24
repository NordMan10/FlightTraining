using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTraining.Model
{
    public interface IThreeDPoint
    {
        int Id { get; }

        int X { get; }

        int Y { get; }

        int Z { get; }

        List<double[]> ShiftsData { get; }

        void SetShiftsData(int index, double[] shifts);

        void ChangeCoords(int x = 0, int y = 0, int z = 0);

        void ChangeCoords(Tuple<int, int, int> coords);

    }
}
