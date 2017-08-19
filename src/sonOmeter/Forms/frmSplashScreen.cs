using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using UKLib.Net;
using sonOmeter.Classes;
using System.Collections.Specialized;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Linq;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmSplashScreen.
    /// </summary>
    public partial class frmSplashScreen : System.Windows.Forms.Form
    {
        #region Variables
        int ShowStatusCounter=0;
        GSC settings = new GSC();
        #endregion

        #region Properties

        private bool aboutBox;
        private System.Windows.Forms.Timer tmShowStatus;
        private System.Windows.Forms.Button btnAcceptDemo;
        private Button btnSavePCID;
        private SaveFileDialog dlgSave;
        
        public string lbStatusText
        {
            set 
            {
                if (!aboutBox)
                    lbStatus.Text = value;
            }
        }

        public string lbRegisteredText
        {
            set 
            {
                lbRegistered.Text = "Registered for: " + value;
            }
        }

        private int pnHideStatus;

        public int PnHideStatus
        {
            get {return pnHideStatus;}
            set 
            { 
                pnHideStatus = value;
                if (pnHideStatus < -1) pnHideStatus=-1;
                if (pnHideStatus < 46)
                    pnHideLine.Height = (int)((pnHideStatus+1)*7.25);
            }
        }
        #endregion

        #region Create & Destroy

        public frmSplashScreen(bool aboutBox)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            PnHideStatus = -1;

            if (aboutBox) 
            {
                this.aboutBox = aboutBox;
                tmShowStatus.Enabled = aboutBox;
            }

            var assembly = System.Reflection.Assembly.GetCallingAssembly();

            var Version = "";

            try
            {
                Version = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).OfType<AssemblyInformationalVersionAttribute>().SingleOrDefault().InformationalVersion;

            }
            catch (Exception)
            {
                
            }
            
            lbVersion.Text = "Version 2010 - build "+ Version + "";
        }


        #endregion

        #region Splash Functions
        public void StartSplash()
        {
            lbStatus.Text = "Loading Modules";
            PnHideStatus = 40;
        }

        public void DoSplash()
        {
            switch (PnHideStatus)
            {
                case 8:
                    lbStatus.Text = "Loading Conversion Data Set";
                    break;
                case 12:
                    lbStatus.Text = "Loading Conversion Data Set (WGS84)";
                    break;
                case 15:
                    lbStatus.Text = "Loading Conversion Data Set (DHDN)";
                    break;
                case 18:
                    lbStatus.Text = "Loading Default Values";
                    break;
                case 24:
                    lbStatus.Text = "Loading Sonarfileformat";
                    break;
                case 30:
                    lbStatus.Text = "Initialising Modules";
                    break;
                case 32:
                    lbStatus.Text = "Initialising Main Form";
                    break;
                case 40:
                    lbStatus.Text = "Check Registration";

                    break;
            }
                
            if (pnHideStatus < 5) 
                Opacity =(pnHideStatus)/5.0;		

            PnHideStatus--;
        }

        public bool isReady()
        {
            if (!aboutBox) 
            {
                if (pnHideStatus < 0) 
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Timer
        public void tmShowStatus_Tick(object sender, System.EventArgs e)
        {
            if (aboutBox) 
            {                
                {
                    switch  (ShowStatusCounter) 
                    {
                        case 0:
                            lbStatus.Text = "Copyright © 2004-2010 softwaredriven / UKLib";
                            //lbStatus.Text = 
                            ShowStatusCounter++;
                            break;
                        case 1:
                            ShowStatusCounter++;
                            break;
                        case 2:
                            lbStatus.Text = "Coder: Uwe Mayer and Konrad Mattheis";
                            ShowStatusCounter++;
                            break;
                        default:
                            ShowStatusCounter = 0;
                            break;
                    }
                }
            }
            else
            {
            }
        }
        #endregion

        #region Click Events
        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            if (aboutBox)
                Close();
        }

        private void btnAcceptDemo_Click(object sender, System.EventArgs e)
        {
            btnAcceptDemo.Visible = false;
            PnHideStatus=32;
        
        }
        #endregion

       
                
    }
}
