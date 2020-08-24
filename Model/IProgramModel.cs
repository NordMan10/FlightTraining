using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FlightTraining.Model
{
    public interface IProgramModel
    {
        Stopwatch Stopwatch { get; }

        Timer GraphicTimer { get; }

        GameStage Stage { get; }

        event Action<GameStage> StageChanged;

        void Begin();

        void Start();

        void Stop();

        void Pause();
    }
}
