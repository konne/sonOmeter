using System;
using System.Management;
using System.Collections.Specialized;

namespace UKLib.Net
{
    public class MacAddress
    {
        StringCollection macs = new StringCollection();
        StringCollection names = new StringCollection();

        public StringCollection Macs
        {
            get 
            {
                return macs;
            }
        }

        public StringCollection Names
        {
            get
            {
                return names;
            }
        }

        public MacAddress()
        {           
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            macs = new StringCollection();

            foreach (ManagementObject mo in moc)
            {
                object mac = mo["MacAddress"];               
                if (mac != null)
                {
                    macs.Add(mac.ToString());
                    names.Add(mo["Caption"].ToString());
                }
            }
        }
    }
}
