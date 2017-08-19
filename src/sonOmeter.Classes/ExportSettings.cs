namespace sonOmeter.Classes
{
    #region Usings
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;
    using UKLib.ExtendedAttributes;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// This class encapsulates all settings for export sonar files.
    /// </summary>
    public class ExportSettings : INotifyPropertyChanged
    {
        #region Variables 
        CustomerExport customer = new CustomerExport();
        #endregion

        #region Properties
     

        private List<CustomerExport> avExportCustomer = new List<CustomerExport>();
        [Browsable(false)]
        [XmlIgnore]
        public List<CustomerExport> AvExportCustomer
        {
            get
            {
                return avExportCustomer;
            }
        }


        [StandardValueExclusive(true)]
        [StandardValueProperty("AvExportCustomer")]
        [TypeConverter(typeof(StandardValueProviderExpandableTypeConverter))]
        [Category("Export")]
        [DisplayName("Customer")]
        [Description("Expand this node to set your customized export function.")]
        public CustomerExport Customer
        {
            get
            {
                return customer;
            }
            set
            {
                if (value != customer)
                {
                    customer = value;
                    OnPropertyChanged("Customer");
                }
            }
        }


        [Description("tbd"), Category("Export"), DisplayName("Min Depth")]
        [XmlIgnore]
        public double ExportMinDepth
        {
            get { return GSC.Settings.ExportMinDepth; }
            set { GSC.Settings.ExportMinDepth = value; }
        }

        [Description("tbd"), Category("Export"), DisplayName("Max Depth")]
        [XmlIgnore]
        public double ExportMaxDepth
        {
            get { return GSC.Settings.ExportMaxDepth; }
            set { GSC.Settings.ExportMaxDepth = value; }
        }

        [Description("Export data with cut lines enabled."), Category("Export"), DisplayName("Enable Cut"), DefaultValue(true)]
        [XmlIgnore]
        public bool ExportWithCut
        {
            get { return GSC.Settings.ExportWithCut; }
            set { GSC.Settings.ExportWithCut = value; }
        }

        [Description("Export data with virtual archeology enabled."), Category("Export"), DisplayName("Enable Arch"), DefaultValue(false)]
        [XmlIgnore]
        public bool ExportWithArch
        {
            get { return GSC.Settings.ExportWithArch; }
            set { GSC.Settings.ExportWithArch = value; }
        }

        [Description("Append buoys to export (formatted like manual points)."), Category("Export"), DisplayName("Append Buoys"), DefaultValue(false)]
        [XmlIgnore]
        public bool ExportWithBuoys
        {
            get { return GSC.Settings.ExportWithBuoys; }
            set { GSC.Settings.ExportWithBuoys = value; }
        }

        [Description("Only export data with this sonar type"), Category("Export"), DisplayName("Sonar Type"), DefaultValue(SonarPanelType.HF)]
        [XmlIgnore]
        public SonarPanelType ExportType
        {
            get { return GSC.Settings.ExportSonarType; }
            set { GSC.Settings.ExportSonarType = value; }
        }

        [XmlIgnore]
        public double PitchH
        {
            get { return GSC.Settings.PitchH; }
            set { GSC.Settings.PitchH = value; }
        }

        [XmlIgnore]
        public double PitchV
        {
            get { return GSC.Settings.PitchV; }
            set { GSC.Settings.PitchV = value; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public string[] Exportfunction
        {
            get { return GSC.Settings.ExportFunction; }
            set { GSC.Settings.ExportFunction = value; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public PosType ScanPosType
        {
            get { return GSC.Settings.ScanPosType; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public ExportSettings()
        {
        }
        #endregion

        #region PropertyChanged
        private void OnPropertyChanged(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}