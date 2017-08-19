using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UKLib.Hex;
using System.Threading;
using UKLib.Debug;

namespace sonOmeter.SonarFlashLoader
{
    public partial class frmFlashSonar : Form
    {
        public frmFlashSonar()
        {
            InitializeComponent();
            for (int i = 1; i < 20; i++)
                cmbPort.Items.Add("COM" + i.ToString());
            cmbPort.SelectedIndex = 0;
            cmbBaudRate.SelectedIndex = 1;
            
            

        }

        bool AbortUpload = false;

        private void button1_Click(object sender, EventArgs e)
        {
            tbLog.Text = "";
            int tbLogIndex = 0;
            
            int maxAddr = 0;
            if (serial.IsOpen) serial.Close();
            serial.PortName = cmbPort.Text;
            serial.BaudRate = int.Parse(cmbBaudRate.Text);
            serial.ReadBufferSize = 32768;
            
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            if (!serial.IsOpen) return;


            AbortUpload = false;
            btnAbort.Visible = true;
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                IntelHexFile ihf = new IntelHexFile(dlgOpen.FileName);

                maxAddr = ihf.Size;
                
                // 16byte alligned
                maxAddr = maxAddr + (16 - maxAddr % 16);

                #region SendFile
                pgBar.Value = 0;
                pgBar.Maximum = maxAddr;
                serial.ReadExisting();
                serial.Write("%prog\r\n");

                #region Init
                bool ende = false;
                do
                {
                    if (serial.BytesToRead > 0)
                    {
                        byte b = (byte)serial.ReadByte();

                        switch (b)
                        {
                            case 0x00:
                                tbLog.Text += "\r\nSegment 0 Ereased (0x00)";
                                tbLogIndex = 0;
                                break;
                            case 0x40:
                                tbLog.Text += "\r\nSegment 1 Ereased (0x40)";
                                tbLogIndex = 0;
                                break;
                            case 0x80:
                                tbLog.Text += "\r\nSegment 2 Ereased (0x80)";
                                tbLogIndex = 0;
                                break;
                            case 0x55:
                                ende = true;
                                tbLog.Text += "\r\nStart Programming (0x55)";
                                break;
                            default:
                                string s = "";
                                if (tbLogIndex == 0) s += "\r\n";
                                s += b.ToString("X2") + " ";
                                tbLog.Text += s;
                                tbLogIndex++;
                                if (tbLogIndex == 7) tbLogIndex = 0;
                                break;

                        }
                    }
                    Application.DoEvents();
                }
                while (!ende & !AbortUpload);
                #endregion

                tbLog.AppendText("\r\n");
                byte[] leer = new byte[1] { 32 };
               
                byte[] b0x00 = new byte[1] { 0x00 };
                byte[] b0x0D = new byte[1] { 0x0D };

                pgBar.Maximum = maxAddr;                
                serial.ReadTimeout = 1000;
               
                for (int se = 0; se <= maxAddr; se++)
                {                    
                    serial.Write(new byte[1] {ihf[se]}, 0, 1);
                    Application.DoEvents();
                    serial.Write(leer, 0, 1);
                                  
                    if (se % 16 == 15)
                    {                      
                        pgBar.Value = se;   
                        Application.DoEvents();
                        if (AbortUpload)
                            break;
                    }                                                        
                }
                

                // Send Flash Endmarker
                serial.Write(b0x00,0,1);
                Application.DoEvents();
                serial.Write(b0x0D,0,1);

                Thread.Sleep(500);

                byte[] buffer = new byte[serial.BytesToRead];             
                serial.Read(buffer, 0, buffer.Length);

                #region Verify Flash
                bool flashOk = true;
                if (buffer.Length >= maxAddr)
                {
                    for (int se = 0; se <= maxAddr; se++)
                    {
                        if (buffer[se] != ihf[se])
                        {
                            flashOk = false;
                            break;
                        }
                    }
                }
                else
                    flashOk = false;

                tbLog.AppendText("\r\nFlashstatus: " + flashOk.ToString());
                #endregion

                tbLog.AppendText("\r\nFinished\r\n");                              

                #endregion
            }
            serial.Close();
            btnAbort.Visible = false;
            pgBar.Value = 0;
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            AbortUpload = true;
        }
       
    }
}