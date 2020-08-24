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
        string Name { get; }
        double X { get; }
        double Y { get; }
        double Z { get; }
        int Velocity { get; }
        int HeightToGain { get; }
        string EntryTime { get; }
        Image Image { get; }
        int ImageSize { get; }
        Label InfoForm { get; }
        int TrackId { get; }
        List<IThreeDPoint> Path { get; }
        bool NeedToRemove { get; } // сделать метод проверки состояния
        FlightStage FlightStage { get; }
        void Move();

        /// <summary>
        /// Проверяет прибытие в очередную точку для замены смещений, и, если прибыл, заменяет смещения
        /// </summary>
        void CheckToChangePath();

        IThreeDPoint GetFutureLocation();

        void UpdateLocationAndShifts();

        void SaveOldPath();

        void ChangeFlightStage(FlightStage stage);

        void SetHeightToGain(int height);
    }
}
