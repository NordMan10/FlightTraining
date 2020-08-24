using FlightTraining.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlightTraining
{
    public partial class MainForm : Form
    {
        private IProgramModel model;

        public MainForm()
        {
            InitializeComponent();

            model = new ProgramModel();
            model.StageChanged += Model_StageChanged;

            ShowStartScreen(model);
        }

        private void Model_StageChanged(GameStage stage)
        {
            switch(stage)
            {
                case GameStage.NotStarted:
                    ShowStartScreen(model);
                    break;
                case GameStage.Started:
                    ShowMainScreen(model);
                    break;
                case GameStage.Simulating:
                    break;
                case GameStage.Paused:
                    break;
                default:
                    ShowStartScreen(model);
                    break;
            }
        }

        private void ShowStartScreen(IProgramModel model)
        {
            HideScreens();
            startControl.Configure(model);
            startControl.Show();
        }

        private void ShowMainScreen(IProgramModel model)
        {
            mainControl.Configure(model);
            mainControl.Show();
        }

        private void HideScreens()
        {
            startControl.Hide();
            mainControl.Hide();
        }
    }
}
