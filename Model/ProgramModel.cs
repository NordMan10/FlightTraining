using System;
using System.Collections.Generic;
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

        public Timer GraphicTimer { get; private set; }

        public GameStage Stage { get; private set; }

        public event Action<GameStage> StageChanged;

        public void Begin()
        {
            Stopwatch = new Stopwatch();
            InitGraphicTimer();
            ChangeStage(GameStage.Started);
        }

        public void Start()
        {
            Stopwatch.Start();
            GraphicTimer.Start();

            ChangeStage(GameStage.Simulating);
        }

        public void Stop()
        {
            Stopwatch.Reset();
            GraphicTimer.Stop();
            
            ChangeStage(GameStage.Started);
        }

        public void Pause()
        {
            Stopwatch.Stop();
            GraphicTimer.Stop();

            ChangeStage(GameStage.Paused);
        }

        private void InitGraphicTimer()
        {
            GraphicTimer = new Timer { Interval = ProgramOptions.GraphicTimerInterval };
        }

        private void ChangeStage(GameStage stage)
        {
            Stage = stage;
            StageChanged?.Invoke(stage);
        }

    }
}
