using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FlightTraining.Model
{
    public class ProgramModel : IProgramModel
    {
        public ProgramModel()
        {
            Stage = GameStage.NotStarted;
        }

        public Stopwatch Stopwatch { get; private set; }

        public GameStage Stage { get; private set; }

        public event Action<GameStage> StageChanged;

        public void Begin()
        {
            Stopwatch = new Stopwatch();

            ChangeStage(GameStage.Started);
        }

        public void Start()
        {
            Stopwatch.Start();

            ChangeStage(GameStage.Simulating);
        }

        public void Stop()
        {
            Stopwatch.Reset();
            
            ChangeStage(GameStage.Started);
        }

        public void Pause()
        {
            Stopwatch.Stop();

            ChangeStage(GameStage.Paused);
        }

        private void ChangeStage(GameStage stage)
        {
            Stage = stage;
            StageChanged?.Invoke(stage);
        }

        public TimeSpan GetStopwatchElapsedTime()
        {
            return Stopwatch.Elapsed;
        }

        //void SetGraphicTimer_TickEvent(event Action action)
        //{
        //    GraphicTimer += action;
        //}

    }
}
