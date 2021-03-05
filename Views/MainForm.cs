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

        private void Model_StageChanged(ModelStage stage)
        {
            switch(stage)
            {
                case ModelStage.NotStarted:
                    ShowStartScreen(model);
                    break;
                case ModelStage.Started:
                    ShowMainScreen(model);
                    break;
                case ModelStage.Simulating:
                    break;
                case ModelStage.Paused:
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
