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

        /// <summary>
        /// Прибавляет к текущим координатам полученные
        /// </summary>
        void ChangeCoords(int x = 0, int y = 0, int z = 0);

        /// <summary>
        /// Заменяет текущие координаты на новые
        /// </summary>
        /// <param name="coords"></param>
        void ChangeCoords(Tuple<int, int, int> coords);

    }
}
