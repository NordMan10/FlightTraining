namespace FlightTraining
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainControl = new FlightTraining.Views.MainControl();
            this.startControl = new FlightTraining.Views.StartControl();
            this.SuspendLayout();
            // 
            // mainControl
            // 
            this.mainControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainControl.Location = new System.Drawing.Point(0, 0);
            this.mainControl.Name = "mainControl";
            this.mainControl.Size = new System.Drawing.Size(1262, 721);
            this.mainControl.TabIndex = 1;
            // 
            // startControl
            // 
            this.startControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("startControl.BackgroundImage")));
            this.startControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.startControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startControl.Location = new System.Drawing.Point(0, 0);
            this.startControl.Name = "startControl";
            this.startControl.Size = new System.Drawing.Size(1262, 721);
            this.startControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, (int)(1262 / 1.33));
            this.Controls.Add(this.mainControl);
            this.Controls.Add(this.startControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "FlightTraining";
            this.ResumeLayout(false);

        }

        #endregion

        private Views.StartControl startControl;
        private Views.MainControl mainControl;
        
    }
}

