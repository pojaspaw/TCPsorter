namespace SorterTCP.PlcConnectivity
{
    partial class SplashScreen
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
            this.prgBar_Splash = new System.Windows.Forms.ProgressBar();
            this.pcBox_splash = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pcBox_splash)).BeginInit();
            this.SuspendLayout();
            // 
            // prgBar_Splash
            // 
            this.prgBar_Splash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgBar_Splash.Location = new System.Drawing.Point(-1, 171);
            this.prgBar_Splash.MarqueeAnimationSpeed = 15;
            this.prgBar_Splash.Name = "prgBar_Splash";
            this.prgBar_Splash.Size = new System.Drawing.Size(535, 10);
            this.prgBar_Splash.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgBar_Splash.TabIndex = 1;
            // 
            // pcBox_splash
            // 
            this.pcBox_splash.Image = global::SorterTCP.Properties.Resources.logo_elux;
            this.pcBox_splash.Location = new System.Drawing.Point(60, 12);
            this.pcBox_splash.Name = "pcBox_splash";
            this.pcBox_splash.Size = new System.Drawing.Size(392, 79);
            this.pcBox_splash.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcBox_splash.TabIndex = 0;
            this.pcBox_splash.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Electrolux Sans SemiBold", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(161, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 65);
            this.label1.TabIndex = 2;
            this.label1.Text = "vSorter";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(533, 180);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.prgBar_Splash);
            this.Controls.Add(this.pcBox_splash);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            ((System.ComponentModel.ISupportInitialize)(this.pcBox_splash)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pcBox_splash;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ProgressBar prgBar_Splash;
    }
}