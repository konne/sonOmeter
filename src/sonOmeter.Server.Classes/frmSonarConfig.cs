using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Contronix.Net.Sockets;
using Contronix.Xml;
using Contronix.Strings;

namespace Contronix.sonOmeterServer.Classes
{
	/// <summary>
	/// Summary description for frmSonarConfig.
	/// </summary>
	public class frmSonarConfig : System.Windows.Forms.Form
	{
		#region Form variables
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.Button btnGetConfig;

		private System.Windows.Forms.ListBox lbDevices;
		private System.Windows.Forms.PropertyGrid pgDevices;
		private System.Windows.Forms.Button btnLog;
		private Contronix.Strings.HexListBox lbDataView;
		#endregion

		#region Variables
		private TcpClientThread configThread = new TcpClientThread();		
	//	private GlobalSettings settings = null;
		bool connected = false;
	//	bool started = false;
		Device lastLogDev = null;
		#endregion


        #region Properties
/*        public GlobalSettings Settings
		{
			get { return settings; }
			set { settings = value; }
		}*/
			
		#endregion

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSonarConfig()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSonarConfig));
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnGetConfig = new System.Windows.Forms.Button();
            this.pgDevices = new System.Windows.Forms.PropertyGrid();
            this.lbDevices = new System.Windows.Forms.ListBox();
            this.btnLog = new System.Windows.Forms.Button();
            this.lbDataView = new Contronix.Strings.HexListBox();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(16, 16);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(120, 32);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStop.Location = new System.Drawing.Point(152, 16);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(64, 32);
            this.btnStartStop.TabIndex = 1;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnGetConfig
            // 
            this.btnGetConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetConfig.Location = new System.Drawing.Point(16, 56);
            this.btnGetConfig.Name = "btnGetConfig";
            this.btnGetConfig.Size = new System.Drawing.Size(120, 32);
            this.btnGetConfig.TabIndex = 1;
            this.btnGetConfig.Text = "Get config";
            this.btnGetConfig.Click += new System.EventHandler(this.btnGetConfig_Click);
            // 
            // pgDevices
            // 
            this.pgDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgDevices.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pgDevices.Location = new System.Drawing.Point(232, 16);
            this.pgDevices.Name = "pgDevices";
            this.pgDevices.Size = new System.Drawing.Size(240, 384);
            this.pgDevices.TabIndex = 2;
            this.pgDevices.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgDevices_PropertyValueChanged);
            // 
            // lbDevices
            // 
            this.lbDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbDevices.Location = new System.Drawing.Point(16, 104);
            this.lbDevices.Name = "lbDevices";
            this.lbDevices.Size = new System.Drawing.Size(200, 290);
            this.lbDevices.TabIndex = 3;
            this.lbDevices.SelectedIndexChanged += new System.EventHandler(this.lbDevices_SelectedIndexChanged);
            // 
            // btnLog
            // 
            this.btnLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLog.Location = new System.Drawing.Point(152, 56);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(64, 32);
            this.btnLog.TabIndex = 1;
            this.btnLog.Text = "Log";
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // lbDataView
            // 
            this.lbDataView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDataView.BytesPerLine = 8;
            this.lbDataView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDataView.HorizontalScrollbar = true;
            this.lbDataView.Location = new System.Drawing.Point(232, 16);
            this.lbDataView.MaxLines = 255;
            this.lbDataView.Name = "lbDataView";
            this.lbDataView.Size = new System.Drawing.Size(240, 381);
            this.lbDataView.TabIndex = 4;
            this.lbDataView.Visible = false;
            // 
            // frmSonarConfig
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(488, 413);
            this.Controls.Add(this.lbDataView);
            this.Controls.Add(this.lbDevices);
            this.Controls.Add(this.pgDevices);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnGetConfig);
            this.Controls.Add(this.btnLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSonarConfig";
            this.Text = "Sonar Config";
            this.Closed += new System.EventHandler(this.frmSonarConfig_Closed);
            this.ResumeLayout(false);

		}
		#endregion

		private void btnConnect_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (connected && configThread.Connected)
				{
					if (configThread.Connected)
						configThread.Close();					
				}
				else if (!connected && !configThread.Connected)
				{
					configThread = new TcpClientThread();					
					configThread.EOLTag = "\r\n";
					configThread.DisconnectTag = "quit\r\n";
					configThread.SynchronizingObject = this;
					configThread.NewLine += new TcpLineEventHandler(configThread_NewLine);
//                    configThread.Connect(settings.HostName, settings.ControlPort);
				}

				connected = !connected;

				if (connected)
					btnConnect.Text = "Disconnect";
				else
					btnConnect.Text = "Connect";
			}
			catch (Exception ex)
			{
				Console.WriteLine("frmSonarConfig: "+ex.Message);
			}
		}

		private void btnStartStop_Click(object sender, System.EventArgs e)
		{
		}

		private void btnGetConfig_Click(object sender, System.EventArgs e)
		{
			try
			{
				configThread.SendString("<getconfig />\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine("frmSonarConfig: "+ex.Message);
			}
		}

		private void btnLog_Click(object sender, System.EventArgs e)
		{
			if (lbDevices.SelectedIndex != -1)
			{
				lbDataView.Visible = !lbDataView.Visible;
				pgDevices.Visible = !pgDevices.Visible;

				Device dev = lbDevices.SelectedItem as Device;
				configThread.SendString("<log id=\""+dev.ID+"\" enabled=\""+lbDataView.Visible.ToString().ToLower()+"\" />\n");

				if (lbDataView.Visible)
					lastLogDev = dev;
				else
					lastLogDev = null;
			}
		}

		private void frmSonarConfig_Closed(object sender, System.EventArgs e)
		{
			if (lastLogDev != null)
				configThread.SendString("<log id=\""+lastLogDev.ID+"\" enabled=\"false\" />\n");
		}

		private void configThread_NewLine(object sender, TcpLineEventArgs e)
		{
			try
			{
			//	int id = 0;
				XmlTextReader reader = new XmlTextReader(new StringReader(e.S));
				reader.WhitespaceHandling = WhitespaceHandling.None;

				reader.Read();
			
			/*	while (!reader.EOF)
				{
					if (reader.NodeType != XmlNodeType.Element)
					{
						reader.Read();
						continue;
					}

					switch (reader.Name)
					{
						case "connected":
							Console.WriteLine("Connection successful.");
							break;

						case "config":
							started = XmlReadConvert.Read(reader, "started", false);
							break;

						case "device":
							id = XmlReadConvert.Read(reader, "id", -1);

							if (id != -1)
							{
								string comport = XmlReadConvert.Read(reader, "comport", "COM1");
								string baudrate = XmlReadConvert.Read(reader, "baudrate", "9600");
								bool log = XmlReadConvert.Read(reader, "log", false);
								string type = XmlReadConvert.Read(reader, "type", "sonar");
								double dx = XmlReadConvert.Read(reader, "dx", 0.0);
								double dy = XmlReadConvert.Read(reader, "dy", 0.0);
								double dz = XmlReadConvert.Read(reader, "dz", 0.0);
								string desc = XmlReadConvert.Read(reader, "desc", "");

								Device dev = null;

								switch (type)
								{
									case "sonar":
										dev = new DeviceSonar(id);

										(dev as DeviceSonar).DX = dx;
										(dev as DeviceSonar).DX = dy;
										(dev as DeviceSonar).DX = dz;

										configThread.SendString("<getsonarconfig id=\""+dev.ID+"\" />\n");
										break;
									case "nmea":
										dev = new Device(Device.DeviceTypes.GPS, id);
										break;
									case "compass_zeiss":
										dev = new Device(Device.DeviceTypes.Compass, id);
										break;
								}

								if (dev != null)
								{
									dev.COMport = comport;
									dev.Description = desc;
									dev.BaudRate = baudrate;

									lbDevices.Items.Add(dev);
								}
							}
							break;

						case "sonarconfig":
							id = XmlReadConvert.Read(reader, "id", -1);

							if (id != -1)
							{
								bool hf = XmlReadConvert.Read(reader, "hf", false);
								bool nf = XmlReadConvert.Read(reader, "nf", false);
								int depth = XmlReadConvert.Read(reader, "depth", 50);
								int sonicspeed = XmlReadConvert.Read(reader, "sonicspeed", 1300);
								string gpsmask = XmlReadConvert.Read(reader, "gpsmask", "$GPGGA");
								
								foreach (Device dev in lbDevices.Items)
								{
									if (!(dev is DeviceSonar) || (dev.ID != id))
										continue;

									(dev as DeviceSonar).Depth = (DeviceSonar.SonarDepths)depth;
									(dev as DeviceSonar).HF = hf;
									(dev as DeviceSonar).NF = nf;
									(dev as DeviceSonar).SonicSpeed = sonicspeed;
									(dev as DeviceSonar).GPSmask = gpsmask;
								}
							}
							break;

						case "log":
							id = XmlReadConvert.Read(reader, "id", -1);

							if (id != -1)
							{
								string data = XmlReadConvert.Read(reader, "data", "");
								lbDataView.AddString(data);
							}
							break;
					}

					reader.Read();
				}*/
			}
			catch
			{
			}
		}

		#region ListBox and PropertyGrid
		private void lbDevices_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			pgDevices.SelectedObject = lbDevices.SelectedItem;

			if (lastLogDev != null)
				configThread.SendString("<log id=\""+lastLogDev.ID+"\" enabled=\"false\" />\n");

			if (lbDataView.Visible)
			{
				lastLogDev = lbDevices.SelectedItem as Device;
				configThread.SendString("<log id=\""+lastLogDev.ID+"\" enabled=\"true\" />\n");
			}
		}

		private void pgDevices_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			Device dev = pgDevices.SelectedObject as Device;
			Device devNew = null;
			int pos = lbDevices.Items.IndexOf(dev);
			
			switch (e.ChangedItem.Label)
			{
				case "Description":
				case "COMport":
					lbDevices.BeginUpdate();
					lbDevices.Items.Remove(dev);
					lbDevices.Items.Insert(pos, dev);
					lbDevices.SelectedItem = dev;
					lbDevices.EndUpdate();
					break;

				case "Type":
                    if (!(dev is DeviceSonar) && (dev.Type == Device.DeviceTypes.SonarV11))
						devNew = new DeviceSonar();
					else if ((dev is DeviceSonar) && (dev.Type != Device.DeviceTypes.SonarV11))
						devNew = new Device();
					else
						break;

					devNew.ID = dev.ID;
					devNew.COMport = dev.COMport;
					devNew.BaudRate = dev.BaudRate;
					devNew.Description = dev.Description;
					devNew.Type = dev.Type;

					lbDevices.BeginUpdate();
					lbDevices.Items.Remove(dev);
					lbDevices.Items.Insert(pos, devNew);
					lbDevices.SelectedItem = devNew;
					lbDevices.EndUpdate();

					pgDevices.SelectedObject = devNew;
					break;
			}
		}
		#endregion
	}
}
