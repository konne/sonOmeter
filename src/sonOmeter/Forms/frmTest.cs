using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UKLib.Survey.Parse;
using sonOmeter.Server.Classes;
using System.Drawing.Drawing2D;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmTest.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public class frmTest :
        Form
    //DockDotNET.DockWindow
    {
        private OpenFileDialog openFileDialog1;
        private Button button1;
        private Button button2;
        private Panel panel1;
        private Button button3;
        private ListBox listBox1;
        private PropertyGrid propertyGrid1;
        private TrackBar trackBar1;
        private System.ComponentModel.IContainer components = null;

        SonarProject project;

        public frmTest(SonarProject prj)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            project = prj;
        }

        sonOmeterServerClass server = null;

        public frmTest(sonOmeterServerClass server)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.server = server;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();

                }
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(154, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Controls.Add(this.propertyGrid1);
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(668, 544);
            this.panel1.TabIndex = 4;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(113, 231);
            this.trackBar1.Maximum = 0;
            this.trackBar1.Minimum = -20;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 104);
            this.trackBar1.TabIndex = 7;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(300, 17);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(266, 177);
            this.propertyGrid1.TabIndex = 6;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(26, 112);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(237, 82);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(26, 59);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // frmTest
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(668, 544);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTest";
            this.Text = "frmAbout";
            this.TransparencyKey = System.Drawing.Color.RosyBrown;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmTest_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void frmTest_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, e.KeyData.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var vfw = new AForge.Video.FFMPEG.VideoFileWriter();

            //vfw.Open("test.mp4", 1900, 1080, 1, AForge.Video.FFMPEG.VideoCodec.MPEG4);

            //for (int i = 0; i < 10; i++)
            //{
            //    var bb = new Bitmap(1900, 1080);
            //    Graphics g = Graphics.FromImage(bb);
            //    g.SmoothingMode = SmoothingMode.HighSpeed;
            //    g.DrawString("Hallo" + i.ToString(), new Font(FontFamily.GenericMonospace, 10), new SolidBrush(Color.Yellow), new PointF(300, 300));
            //    vfw.WriteVideoFrame(bb);
            //    bb.Dispose();              
            //}
            //vfw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
        }
    }
}
