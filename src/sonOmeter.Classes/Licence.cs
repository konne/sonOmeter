using System;
using System.Collections.Generic;
using UKLib.Controls;
using UKLib.Net;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Summary description for Module Management
    /// </summary>
    public class Module
    {
        public enum Modules
        {
            ThreeD,
            CalcDepth,
            Submarine,
            VideoRender,
            RadioDetection,
            DXF,
            V21,
            V22,
            DynamicProfiles,
            Testing
        }

        #region Variables

        private List<Modules> moduleList = new List<Modules>();
        
        #endregion

        #region Properties     
        public bool this[Modules module]
        {
            get { return moduleList.Contains(module); }
        }
        #endregion
   
        #region Constructor
        public Module()
        {
            moduleList.Add(Modules.ThreeD);
            moduleList.Add(Modules.CalcDepth);
            moduleList.Add(Modules.Submarine);
            moduleList.Add(Modules.DXF);
            moduleList.Add(Modules.V21);
            moduleList.Add(Modules.V22);
            moduleList.Add(Modules.VideoRender);
            moduleList.Add(Modules.RadioDetection);
            moduleList.Add(Modules.DynamicProfiles);
            moduleList.Add(Modules.Testing);            
        }
#endregion

    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class LicencedPropertyAttribute : DynamicFilterAttribute
    {
        Module.Modules module;

        public LicencedPropertyAttribute(Module.Modules module)
        {
            this.module = module;
        }

        public override bool IncludeProperty(System.ComponentModel.PropertyDescriptorCollection pdc, object callingObject)
        {
            return GSC.Settings.Lic[module];
        }
    }
}
