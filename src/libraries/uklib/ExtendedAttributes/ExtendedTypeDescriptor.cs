using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace UKLib.ExtendedAttributes
{
    #region DynamicAttributeCollection
    public class DynamicAttributeCollection : Collection<Attribute>
    {
        #region Event
        public ListChangedEventHandler ListChanged;
        #endregion

        #region this[]
        public Attribute this[Type type]
        {
            get
            {
                int idx = IndexOf(type);
                if (idx > -1)
                    return base[idx];
                else
                    return null;
            }
            set
            {
                int idx = IndexOf(type);
                if (idx > -1)
                    base[idx] = value;
                else
                    base.Add(value);
            }
        }
        #endregion

        #region Functions
        public int IndexOf(Type type)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (base.Items[i].GetType() == type)
                    return i;
            }
            return -1;
        }

        public bool Contains(Type type)
        {
            return IndexOf(type) != -1;
        }

        public bool Remove(Type type)
        {
            int idx = IndexOf(type);
            if (idx == -1)
                return false;

            RemoveAt(idx);
            return true;
        }
        #endregion

        #region Override
        protected override void InsertItem(int index, Attribute item)
        {
            base.InsertItem(index, item);
            if (ListChanged != null)
            {
                ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            }
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            if (ListChanged != null)
            {
                ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            }
        }

        protected override void SetItem(int index, Attribute item)
        {
            base.SetItem(index, item);
            if (ListChanged != null)
            {
                ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (ListChanged != null)
            {
                ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }
        #endregion
    }
    #endregion

    #region DynamicAttributePropertyDescriptor
    public class DynamicAttributePropertyDescriptor : PropertyDescriptor
    {
        #region Variables & Properties
        protected PropertyDescriptor basePropertyDescriptor;

        protected DynamicAttributeCollection dynamicAttributes = new DynamicAttributeCollection();

        public DynamicAttributeCollection DynamicAttributes
        {
            get
            {
                return dynamicAttributes;
            }
        }
        #endregion

        #region Constructor
        public DynamicAttributePropertyDescriptor(PropertyDescriptor basePropertyDescriptor)
            : this(basePropertyDescriptor, null)
        {
        }

        public DynamicAttributePropertyDescriptor(PropertyDescriptor basePropertyDescriptor, Attribute[] attmore)
            : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;

            foreach (Attribute itm in basePropertyDescriptor.Attributes)
            {
                dynamicAttributes.Add(itm);
            }

            if (attmore != null)
            {
                foreach (Attribute itm in attmore)
                {
                    if (!dynamicAttributes.Contains(itm))
                        dynamicAttributes.Add(itm);
                }
            }

            dynamicAttributes.ListChanged += new ListChangedEventHandler(dynamicAttributes_ListChanged);
            dynamicAttributes_ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }
        #endregion

        #region ListChanged

        void dynamicAttributes_ListChanged(object sender, ListChangedEventArgs e)
        {
            Attribute[] atts = new Attribute[dynamicAttributes.Count];
            dynamicAttributes.CopyTo(atts, 0);
            this.AttributeArray = atts;
        }
        #endregion

        #region abstract Functions

        public override bool CanResetValue(object component)
        {
            return basePropertyDescriptor.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return basePropertyDescriptor.ComponentType; }
        }

        public override object GetValue(object component)
        {
            return basePropertyDescriptor.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get
            {
                ReadOnlyAttribute ra = this.Attributes[typeof(ReadOnlyAttribute)] as ReadOnlyAttribute;
                if (ra != null)
                {
                    if (ra == ReadOnlyAttribute.No) return false;
                }
                return true;
            }
        }

        public override Type PropertyType
        {
            get { return basePropertyDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            basePropertyDescriptor.CanResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            basePropertyDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return basePropertyDescriptor.ShouldSerializeValue(component);
        }
        #endregion
    }
    #endregion

    #region LocalizedPropertyDescriptor
    public class LocalizedPropertyDescriptor : DynamicAttributePropertyDescriptor
    {
        #region Constructor
        public LocalizedPropertyDescriptor(PropertyDescriptor basePropertyDescriptor, Attribute[] attmore)
            : base(basePropertyDescriptor, attmore)
        {
            base.DynamicAttributes.ListChanged += new ListChangedEventHandler(dynamicAttributes_ListChanged);
            ReadUIStrings();
        }
        #endregion

        #region Listchanged
        bool changing = false;
        void dynamicAttributes_ListChanged(object sender, ListChangedEventArgs e)
        {
            ReadUIStrings();
        }
        #endregion

        #region Funtions
        private void ReadUIStrings()
        {
            if (changing) return;
            changing = true;
            CultureInfo ci = Thread.CurrentThread.CurrentUICulture;
            Type cType = basePropertyDescriptor.ComponentType;
            string category = null;
            string displayName = null;
            string description = null;

            string resourceTable = cType.Namespace + "." + cType.Name;
            string entryName = basePropertyDescriptor.Name;
            string categoryFieldName = entryName + "_Category";
            string displayNameFieldName = entryName + "_DisplayName";
            string descriptionFieldName = entryName + "_Description";

            LocalizedResourcePropertyAttribute at = this.Attributes[typeof(LocalizedResourcePropertyAttribute)] as LocalizedResourcePropertyAttribute;
            if (at != null)
            {
                categoryFieldName = at.CategoryFieldName ?? categoryFieldName;
                displayNameFieldName = at.DisplayNameFieldName ?? displayNameFieldName;
                descriptionFieldName = at.DescriptionFieldName ?? descriptionFieldName;
                resourceTable = at.ResourceTable ?? resourceTable;
            }

            // check if resource exists
            if (cType.Assembly.GetManifestResourceInfo(resourceTable) != null)
            {
                // load resource
                ResourceManager rm = new ResourceManager(resourceTable, cType.Assembly);
                rm.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
                try
                {
                    category = rm.GetString(categoryFieldName);
                    displayName = rm.GetString(displayNameFieldName);
                    description = rm.GetString(descriptionFieldName);
                }
                catch
                {
                }
            }

            for (int i = 0; i < this.dynamicAttributes.Count; i++)
            {
                if (this.dynamicAttributes[i].GetType().Equals(typeof(LocalizedPropertyAttribute)))
                {
                    LocalizedPropertyAttribute lpa = dynamicAttributes[i] as LocalizedPropertyAttribute;
                    if (lpa.Culture.Equals(ci))
                    {
                        category = lpa.Category ?? category;
                        displayName = lpa.DisplayName ?? displayName;
                        description = lpa.Description ?? description;
                    }
                }
            }

            if (category != null) this.DynamicAttributes[typeof(CategoryAttribute)] = new CategoryAttribute(category);
            if (displayName != null) this.DynamicAttributes[typeof(DisplayNameAttribute)] = new DisplayNameAttribute(displayName);
            if (description != null) this.DynamicAttributes[typeof(DescriptionAttribute)] = new DescriptionAttribute(description);
            changing = false;
        }
        #endregion
    }
    #endregion

    //#region ExtendedTypeDescriptorContainer
    //public class ExtendedTypeDescriptorContainer : ICustomTypeDescriptor
    //{
    //    #region Variables
    //    object baseclass;

    //    PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
    //    #endregion

    //    #region Constructor
    //    public ExtendedTypeDescriptorContainer(object o)
    //    {
    //        baseclass = o;
    //        PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(baseclass);
    //        foreach (PropertyDescriptor itm in pdc)
    //        {
    //            PropertyInfo pi = o.GetType().GetProperty(itm.Name);

    //            //if (pi.PropertyType.IsClass)
    //            //{
    //            //    ExtendedTypeDescriptorContainer extdc = new ExtendedTypeDescriptorContainer(itm);                   
    //            //}

    //            Object[] ao = pi.GetCustomAttributes(true);
    //            Attribute[] newAttributes = null;
    //            if (ao != null)
    //            {
    //                newAttributes = new Attribute[ao.Length];
    //                for (int i = 0; i < ao.Length; i++)
    //                    newAttributes[i] = ao[i] as Attribute;
    //            }
    //            properties.Add(new ExtendedPropertyDescriptor(itm, newAttributes));
    //        }
    //    }
    //    #endregion

    //    #region this[]
    //    public ExtendedPropertyDescriptor this[int idx]
    //    {
    //        get
    //        {
    //            if (idx <= (properties.Count - 1))
    //                return properties[idx] as ExtendedPropertyDescriptor;
    //            else
    //                return null;
    //        }
    //    }

    //    public ExtendedPropertyDescriptor this[string name]
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return properties[name] as ExtendedPropertyDescriptor;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion

    //    #region ICustomTypeDescriptor Members

    //    public AttributeCollection GetAttributes()
    //    {
    //        return TypeDescriptor.GetAttributes(baseclass);
    //    }

    //    public string GetClassName()
    //    {
    //        return TypeDescriptor.GetClassName(baseclass, true);
    //    }

    //    public string GetComponentName()
    //    {
    //        return TypeDescriptor.GetComponentName(baseclass, true);
    //    }

    //    public TypeConverter GetConverter()
    //    {
    //        return TypeDescriptor.GetConverter(baseclass);
    //    }

    //    public EventDescriptor GetDefaultEvent()
    //    {
    //        return TypeDescriptor.GetDefaultEvent(baseclass);
    //    }

    //    public PropertyDescriptor GetDefaultProperty()
    //    {
    //        return TypeDescriptor.GetDefaultProperty(baseclass);
    //    }

    //    public object GetEditor(Type editorBaseType)
    //    {
    //        return TypeDescriptor.GetEditor(baseclass, editorBaseType);
    //    }

    //    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    //    {
    //        return TypeDescriptor.GetEvents(baseclass, attributes);
    //    }

    //    public EventDescriptorCollection GetEvents()
    //    {
    //        return TypeDescriptor.GetEvents(baseclass);
    //    }

    //    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    //    {
    //        return properties;
    //    }

    //    public PropertyDescriptorCollection GetProperties()
    //    {
    //        return properties;
    //    }

    //    public object GetPropertyOwner(PropertyDescriptor pd)
    //    {
    //        return baseclass;
    //    }
    //    #endregion
    //}
    //#endregion

    #region DynamicAttributeTypeDescriptorProvider
    public class DynamicAttributeTypeDescriptorProvider<T> : TypeDescriptionProvider
    {
        private TypeDescriptionProvider baseProvider;
        private PropertyDescriptorCollection propCache;

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ExtendedTypeDescriptor(this, baseProvider.GetTypeDescriptor(objectType, instance), objectType);
        }

        public DynamicAttributeTypeDescriptorProvider()
        {
            baseProvider = TypeDescriptor.GetProvider(typeof(T));
        }

        public DynamicAttributeTypeDescriptorProvider(object instance)
        {
            baseProvider = TypeDescriptor.GetProvider(instance);
        }

        public virtual PropertyDescriptor GetExtendedPropertyDescriptor(PropertyDescriptor pd, Attribute[] newAttributes)
        {
            return new DynamicAttributePropertyDescriptor(pd, newAttributes);
        }

        #region ExtendedTypeDescriptor
        private class ExtendedTypeDescriptor : CustomTypeDescriptor
        {
            #region Variables
            private Type objectType;
            private DynamicAttributeTypeDescriptorProvider<T> provider;
            #endregion

            #region Constructor
            public ExtendedTypeDescriptor(DynamicAttributeTypeDescriptorProvider<T> provider, ICustomTypeDescriptor descriptor, Type objectType)
                : base(descriptor)
            {
                if (provider == null) throw new ArgumentNullException("provider");
                if (descriptor == null) throw new ArgumentNullException("descriptor");
                if (objectType == null) throw new ArgumentNullException("objectType");
                this.objectType = objectType;
                this.provider = provider;
            }
            #endregion

            #region Functions
            private PropertyDescriptorCollection GetExtendedProperties(Attribute[] attributes)
            {
                PropertyDescriptorCollection properties = provider.propCache;
                if (properties == null)
                {
                    properties = new PropertyDescriptorCollection(null);
                    PropertyDescriptorCollection pdc = base.GetProperties(attributes);
                    foreach (PropertyDescriptor itm in pdc)
                    {
                        PropertyInfo pi = objectType.GetProperty(itm.Name);
                        Object[] ao = pi.GetCustomAttributes(true);
                        Attribute[] newAttributes = null;
                        if (ao != null)
                        {
                            newAttributes = new Attribute[ao.Length];
                            for (int i = 0; i < ao.Length; i++)
                                newAttributes[i] = ao[i] as Attribute;
                        }
                        properties.Add(provider.GetExtendedPropertyDescriptor(itm, newAttributes));
                    }
                    provider.propCache = properties;
                }
                if (attributes != null)
                {
                    PropertyDescriptorCollection popatt = new PropertyDescriptorCollection(null);
                    foreach (DynamicAttributePropertyDescriptor itm in properties)
                    {
                        if (itm.Attributes.Contains(attributes)) popatt.Add(itm);
                    }
                    properties = popatt;
                }
                return properties;
            }
            #endregion

            #region override
            public override PropertyDescriptorCollection GetProperties()
            {
                return GetExtendedProperties(null);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return GetExtendedProperties(attributes);
            }
            #endregion
        }
        #endregion
    }
    #endregion

    #region LocalizedTypeDescriptorProvider
    public class LocalizedTypeDescriptorProvider<T> : DynamicAttributeTypeDescriptorProvider<T>
    {
        public override PropertyDescriptor GetExtendedPropertyDescriptor(PropertyDescriptor pd, Attribute[] newAttributes)
        {
            return new LocalizedPropertyDescriptor(pd, newAttributes);
        }
    }
    #endregion

    // example: reflection Set Browseable Attribute
    //    private void MakePropertyBrowsable(object selectedObject, string
    //propertyName, bool browsableFlag) {

    //    // Declare our locals.
    //    PropertyDescriptorCollection props =3D null;
    //    AttributeCollection attribs =3D null;
    //    Type attrType =3D null;
    //    BrowsableAttribute attr =3D null;
    //    FieldInfo fld =3D null;

    //    // Get a list of properties from the object passed.
    //    props = TypeDescriptor.GetProperties(
    //        selectedObject
    //        );

    //    // Get a list of attributes applied to the property specified
    //    // on the object passed.
    //    attribs = props[propertyName].Attributes;

    //    // Pull out the specific BrowsableAttribute instance.
    //    attr = (BrowsableAttribute)attribs[BrowsableAttribute.Default.GetType()];

    //    // Get the type of the attribute for the
    //    // purposes of doing our reflection stuff.
    //    attrType = attr.GetType();

    //    // Get a reference to the private field in
    //    // the actualy BrowsableAttribute class.
    //    fld = attrType.GetField(
    //        "browsable",
    //        BindingFlags.Instance |
    //        BindingFlags.NonPublic
    //        );

    //    // Set the value of the private field.
    //    fld.SetValue(
    //        attr,
    //        browsableFlag
    //        );

    //}
}
