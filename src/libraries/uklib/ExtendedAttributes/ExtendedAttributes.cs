using System;

namespace UKLib.ExtendedAttributes
{
    #region ExtendedBrowsableAttribute
    public class ExtendedBrowsableAttribute : Attribute
    {
        public static readonly ExtendedBrowsableAttribute Default = new ExtendedBrowsableAttribute(true);
        public static readonly ExtendedBrowsableAttribute No = new ExtendedBrowsableAttribute(false);
        public static readonly ExtendedBrowsableAttribute Yes = new ExtendedBrowsableAttribute(true);

        private bool browsable;

        public ExtendedBrowsableAttribute(bool browsable)
        {
            this.browsable = browsable;
        }

        public bool Browsable
        {
            get
            {
                return browsable;
            }
            set
            {
                browsable = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExtendedBrowsableAttribute))
                return false;
            if (obj == this)
                return true;
            return ((ExtendedBrowsableAttribute)obj).Browsable == browsable;
        }

        public override bool IsDefaultAttribute()
        {
            return browsable == ExtendedBrowsableAttribute.Default.Browsable;
        }

        public override int GetHashCode()
        {
            return browsable.GetHashCode();
        }
    }
    #endregion

    #region LocalizedEnumDisplayName
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDisplayName : Attribute
    {
        private string displayName = "";

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public EnumDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        public override string ToString()
        {
            return displayName;
        }
    }
    #endregion
}
