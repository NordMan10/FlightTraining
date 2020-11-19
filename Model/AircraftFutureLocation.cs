using System;
using System.Collections.Generic;

namespace FlightTraining.Model
{
    public class AircraftFutureLocation
    {
        private double x;
        private double y;
        private double z;

        private readonly Shifts shifts = new Shifts();
        private int tracker;
        private List<Point3D> path = new List<Point3D>();
        private Action<int, Shifts> calcAndSetShifts = (tracker, shifts) => throw new NotImplementedException();
        private bool isPointInFinish = false;

        /// <summary>
        /// Возвращает точку, в которой окажется ВС через указанный промежуток времени
        /// </summary>
        /// <param name="x_">X координата ВС</param>
        /// <param name="y_">Y координата ВС</param>
        /// <param name="z_">Z координата ВС</param>
        /// <param name="dx">Смещение ВС по X</param>
        /// <param name="dy">Смещение ВС по Y</param>
        /// <param name="dz">Смещение ВС по Z</param>
        /// <param name="tracker_"></param>
        /// <param name="path_"></param>
        /// <param name="calcAndSetShifts_"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Point3D GetFutureLocation(double x_, double y_, double z_, double dx, double dy, 
            double dz, int tracker_, List<Point3D> path_, Action<int, Shifts> calcAndSetShifts_, int interval)
        {
            if (isPointInFinish) return new Point3D((int)x, (int)y, (int)z);

            x = x_;
            y = y_;
            z = z_;

            shifts.Dx = dx;
            shifts.Dy = dy;
            shifts.Dz = dz;

            tracker = tracker_;
            path = path_;

            calcAndSetShifts = calcAndSetShifts_;

            for (var i = 0; i < interval; i++)
            {
                CheckToChangePath();
                x += shifts.Dx * ProgramOptions.TimeCoefficient;
                y += shifts.Dy * ProgramOptions.TimeCoefficient;
                z += shifts.Dz * ProgramOptions.TimeCoefficient;
            }

            return new Point3D((int)x, (int)y, (int)z);
        }

        private void CheckToChangePath()
        {
            if (IsPointNear()) ChangeShifts();
        }

        private bool IsPointNear()
        {
            var dx = Math.Abs(path[tracker + 1].X - x);
            var dy = Math.Abs(path[tracker + 1].Y - y);
            if (dx + dy <= AircraftOptions.CloseDistance) return true;
            else return false;
        }

        private void ChangeShifts()
        {
            if (tracker + 1 < path.Count - 1)
            {
                tracker++;
                calcAndSetShifts(tracker, shifts);
            }
            else
            {
                isPointInFinish = true;
                shifts.Dx = shifts.Dy = shifts.Dz = 0;
            }

        }
    }
}
