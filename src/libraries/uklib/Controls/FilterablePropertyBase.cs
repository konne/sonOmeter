using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace UKLib.Controls
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DynamicFilterAttribute : Attribute
    {
        private string propertyName;
        private string showOn;

        public DynamicFilterAttribute(string propertyName, string showOn)
        {
            this.propertyName = propertyName;
            this.showOn = showOn;
        }

        public DynamicFilterAttribute()
        {
            this.propertyName = "";
            this.showOn = "";
        }

        public virtual bool IncludeProperty(PropertyDescriptorCollection pdc, Object callingObject)
        {
            PropertyDescriptor pd = pdc[this.propertyName];
            return (this.showOn.IndexOf(pd.GetValue(callingObject).ToString()) > -1);
        }        
    }

    public class FilterablePropertyBase : ICustomTypeDescriptor
    {
        protected PropertyDescriptorCollection GetFilteredProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);
            PropertyDescriptorCollection finalProps = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

            int numP = pdc.Count;

            for (int i = 0; i<numP; i++)
            {
                PropertyDescriptor pd = pdc[i];

                bool include = false;
                bool dynamic = false;

                int numA = pd.Attributes.Count;

                for (int j = 0; (j < numA) && !include; j++)
                {
                    Attribute a = pd.Attributes[j];

                    if (a is DynamicFilterAttribute)
                    {
                        dynamic = true;
                        include = (a as DynamicFilterAttribute).IncludeProperty(pdc, this);
                    }
                }

                if (!dynamic || include)
                    finalProps.Add(pd);
            }

            return finalProps;
        }

        #region ICustomTypeDescriptor Members
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetFilteredProperties(attributes);
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return GetFilteredProperties(new Attribute[0]);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        #endregion
    }
}
