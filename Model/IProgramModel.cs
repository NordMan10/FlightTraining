using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FlightTraining.Model
{
    public interface IProgramModel
    {
        ModelStage Stage { get; }

        event Action<ModelStage> StageChanged;

        void Begin();

        void Start();

        void Stop();

        void Pause();

        TimeSpan GetStopwatchElapsedTime();
    }
}
