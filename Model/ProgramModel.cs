using System;
using System.Diagnostics;

namespace FlightTraining.Model
{
    public class ProgramModel : IProgramModel
    {
        public ProgramModel()
        {
            Stage = ModelStage.NotStarted;
        }

        public Stopwatch Stopwatch { get; private set; }

        public ModelStage Stage { get; private set; }

        public event Action<ModelStage> StageChanged;

        public void Begin()
        {
            Stopwatch = new Stopwatch();

            ChangeStage(ModelStage.Started);
        }

        public void Start()
        {
            Stopwatch.Start();

            ChangeStage(ModelStage.Simulating);
        }

        public void Stop()
        {
            Stopwatch.Reset();
            
            ChangeStage(ModelStage.Started);
        }

        public void Pause()
        {
            Stopwatch.Stop();

            ChangeStage(ModelStage.Paused);
        }

        private void ChangeStage(ModelStage stage)
        {
            Stage = stage;
            StageChanged?.Invoke(Stage);
        }

        public TimeSpan GetStopwatchElapsedTime()
        {
            return Stopwatch.Elapsed;
        }
    }
}
