using System;
using System.Windows.Forms;
using FlightTraining.Model;

namespace FlightTraining.Views
{
    public partial class StartControl : UserControl
    {
        private IProgramModel model;
        public StartControl()
        {
            InitializeComponent();

            //DoubleBuffered = true;
        }

        private bool configured = false;

        public void Configure(IProgramModel model_)
        {
            if (configured) return;
            model = model_;
            launchButton.Click += StartButton_Click;
            configured = true;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            model.Begin();
        }
    }
}
