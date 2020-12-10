using FlightTraining.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlightTraining.Views
{
    public partial class MainControl : UserControl
    {
        private IProgramModel model;
        public MainControl()
        {
            InitializeComponent();

            DoubleBuffered = true;

        }

        private bool configured = false;

        public void Configure(IProgramModel model_)
        {
            if (configured) return;

            model = model_;
            fieldControl.Configure(model);

            startButton.Click += StartButton_Click;
            pauseButton.Click += PauseButton_Click;

            graphicTimer.Tick += GraphicTimer_Tick;
            clockTimer.Tick += ClockTimer_Tick;

            configured = true;
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            var elapsedTime = model.GetStopwatchElapsedTime();
            clock.Text = string.Format("{0:00}:{1:00}:{2:00}", 
                elapsedTime.Minutes, elapsedTime.Seconds, elapsedTime.Milliseconds / 10);
        }

        private void GraphicTimer_Tick(object sender, EventArgs e) 
        {
            fieldControl.PredictAircraftsLocation();
            fieldControl.MoveAllAircrafts();
            fieldControl.Invalidate();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartAllTimers();

            fieldControl.AddFirstAircrafts();
            fieldControl.Invalidate();

            startButton.BackColor = Color.Red;
            startButton.Text = "Stop";
            startButton.Click -= StartButton_Click;
            startButton.Click += StopButton_Click;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if(model.Stage == GameStage.Paused) ResetPauseButton();

            StopAllTimers();

            fieldControl.RemoveAllAircrafts();
            fieldControl.Invalidate();

            clock.Text = "00:00:00";

            startButton.BackColor = Color.LimeGreen;
            startButton.Text = "Start";
            startButton.Click -= StopButton_Click;
            startButton.Click += StartButton_Click;
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (model.Stage != GameStage.Simulating)
            {
                MessageBox.Show("Сначала запустите симуляцию", "Ошибка!", MessageBoxButtons.OK);
                return;
            }

            StopAllTimers(GameStage.Paused);

            pauseButton.Text = "Continue";
            pauseButton.Click -= PauseButton_Click;
            pauseButton.Click += ContinueButton_Click;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            StartAllTimers();

            ResetPauseButton();
        }

        private void ResetPauseButton()
        {
            pauseButton.Text = "Pause";
            pauseButton.Click -= ContinueButton_Click;
            pauseButton.Click += PauseButton_Click;
        }

        private void StartAllTimers()
        {
            model.Start();
            fieldControl.StartTimers();
            clockTimer.Start();
            graphicTimer.Start();
        }

        private void StopAllTimers(GameStage stage = GameStage.Started)
        {
            if (stage == GameStage.Paused) model.Pause();
            else model.Stop();
            fieldControl.StopTimers();
            clockTimer.Stop();
            graphicTimer.Stop();
        }
    }
}
