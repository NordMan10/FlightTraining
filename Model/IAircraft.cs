using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace FlightTraining.Model
{
    
    public interface IAircraft
    {
        AircraftType Type { get; }
        int Id { get; }
        Image Image { get; }
        int ImageSize { get; }
        Label InfoForm { get; }
        int TrackId { get; }
        void Move();

        /// <summary>
        /// Проверяет прибытие в очередную точку для замены смещений, и, если прибыл, заменяет смещения
        /// </summary>
        void CheckToChangePath();

        /// <summary>
        /// Вызывает метод, возвращающий точку, в которой окажется ВС через указанный промежуток времени
        /// </summary>
        /// <returns></returns>
        Point3D GetFutureLocation();

        void UpdateLocationAndShifts();

        void SaveOldPath();

        void ChangeFlightStage(FlightStage stage);

        void SetHeightToGain(int height);

        FlightStage GetFlightStage();

        Tuple<double, double, double> GetCoords();
    }
}
