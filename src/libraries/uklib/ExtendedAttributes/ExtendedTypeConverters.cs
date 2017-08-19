namespace UKLib.ExtendedAttributes
{
    #region Usings
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    #endregion

    #region StandardValuePropertyAttribute
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class StandardValuePropertyAttribute : Attribute
    {
        #region Variables & Properties
        private string name;
        private Type staticType = null;
        private List<PropertyInfo> cache = null;
        #endregion

        #region Constructors
        public StandardValuePropertyAttribute(Type StaticType, string Name)
        {
            name = Name;
            staticType = StaticType;
        }

        public StandardValuePropertyAttribute(string Name)
            : this(null, Name)
        {

        }

        #endregion

        #region BuildCache
        private void BuildCache(object instance)
        {
            if (String.IsNullOrEmpty(name)) return;

            var tempCacheList = new List<PropertyInfo>();

            Type type;

            if (staticType != null)
                type = staticType;
            else
                type = instance.GetType();

            foreach (var item in name.Split(new char[] { '.' }))
            {
                var newCacheItem = type.GetProperty(item);
                if (newCacheItem == null)
                {
                    name = null;
                    return;
                }
                tempCacheList.Add(newCacheItem);
                type = newCacheItem.PropertyType;
            }
            cache = tempCacheList;
        }

        #endregion

        #region GetValue
        public object GetValue(object instance)
        {
            if (cache == null)
                BuildCache(instance);

            if (String.IsNullOrEmpty(name))
                return null;

            if (staticType != null)
                instance = null;

            foreach (var item in cache)
            {
                instance = item.GetValue(instance, null);
                if (instance == null) return null;
            }

            return instance;
        }
        #endregion

        #region SetValue
        public void SetValue(object instance, object value)
        {
            if (cache == null)
                BuildCache(instance);

            if (String.IsNullOrEmpty(name))
                return;

            if (staticType != null)
                instance = null;

            for (int i = 0; i < cache.Count - 1; i++)
            {
                instance = cache[i].GetValue(instance, null);
                if (instance == null) return;
            }
            cache[cache.Count - 1].SetValue(instance, value, null);
        }
        #endregion
    }
    #endregion

    #region StandardValueExclusiveAttribute
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class StandardValueExclusiveAttribute : Attribute
    {
        public static StandardValueExclusiveAttribute No = new StandardValueExclusiveAttribute(false);
        public static StandardValueExclusiveAttribute Yes = new StandardValueExclusiveAttribute(true);

        public StandardValueExclusiveAttribute(bool Exclusive)
        {
            this.Exclusive = Exclusive;
        }

        public override bool IsDefaultAttribute()
        {
            return this.Exclusive == No.Exclusive;
        }

        public bool Exclusive { get; private set; }
    }
    #endregion

    #region StandardValueAllowNewAsNullAttribute
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class StandardValueAllowNewAsNullAttribute : Attribute
    {
        public static StandardValueAllowNewAsNullAttribute No = new StandardValueAllowNewAsNullAttribute(false);
        public static StandardValueAllowNewAsNullAttribute Yes = new StandardValueAllowNewAsNullAttribute(true);

        public StandardValueAllowNewAsNullAttribute(bool Exclusive)
        {
            this.NewAsNull = Exclusive;
        }

        public override bool IsDefaultAttribute()
        {
            return this.NewAsNull == No.NewAsNull;
        }

        public bool NewAsNull { get; private set; }
    }
    #endregion

    #region StandardValueProviderTypeConverter
    public class StandardValueProviderTypeConverter : StringConverter
    {
        #region GetStandardValuesSupported
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return GetStandardValuesEnumerable(context) != null;
        }
        #endregion

        #region GetStandardValuesExclusive
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            var att = context.PropertyDescriptor.Attributes[typeof(StandardValueExclusiveAttribute)] as StandardValueExclusiveAttribute;
            if (att != null)
                return att.Exclusive;

            return false;
        }
        #endregion

        protected virtual string GetString(object o)
        {
            if (o != null)
                return o.ToString();
            return null;
        }

        protected IEnumerable GetStandardValuesEnumerable(ITypeDescriptorContext context)
        {
            var att = context.PropertyDescriptor.Attributes[typeof(StandardValuePropertyAttribute)] as StandardValuePropertyAttribute;
            if (att != null)
            {
                var refo = att.GetValue(context.Instance);
                return refo as IEnumerable;
            }
            return null;
        }

        #region ConvertFrom
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var list = GetStandardValuesEnumerable(context);
            if (list != null)
            {
                foreach (var item in list)
                {
                    string key = GetString(item);
                    if (value.ToString() == key)
                        return item;
                }
            }
            var att = context.PropertyDescriptor.Attributes[typeof(StandardValueAllowNewAsNullAttribute)] as StandardValueAllowNewAsNullAttribute;
            if ((att != null) && (att.NewAsNull))
                return null;

            return base.ConvertFrom(context, culture, value);
        }
        #endregion

        #region ConvertTo
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return GetString(value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion

        #region GetStandardValues
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = new List<string>();
            var att = context.PropertyDescriptor.Attributes[typeof(StandardValueAllowNewAsNullAttribute)] as StandardValueAllowNewAsNullAttribute;
            if ((att != null) && (att.NewAsNull))
                values.Add(null);

            var list = GetStandardValuesEnumerable(context);
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item != null)
                        values.Add(item.ToString());
                }
            }
            return new StandardValuesCollection(values);
        }
        #endregion
    }

    public class StandardValueProviderExpandableTypeConverter : StandardValueProviderTypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context,
                                                object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

    }
    #endregion

    #region LocalizedStandardValueProviderTypeConverter
    public class LocalizedStandardValueProviderTypeConverter<TResource> : StandardValueProviderTypeConverter
    {
        private ResourceManager resManager = ResourceLookup.GetResourceManager(typeof(TResource));

        protected override string GetString(object o)
        {
            if ((o != null) && (resManager != null))
            {
                var key = o.ToString();
                return resManager.GetString(key) ?? key;
            }
            return base.GetString(o);
        }
    }
    #endregion

    #region LocalizedToStringTypeConverter
    public class LocalizedToStringTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (typeof(string) == destinationType)
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string) == destinationType)
            {
                string name = value.ToString();
                try
                {
                    Type valType = value.GetType();
                    if (valType.IsEnum)
                    {
                        var attrs = valType.GetField(name).GetCustomAttributes(typeof(EnumDisplayName), false);
                        if (attrs.Length > 0) return ((EnumDisplayName)attrs[0]).ToString();

                        attrs = valType.GetField(name).GetCustomAttributes(typeof(LocalizedEnumDisplayName), false);
                        if (attrs.Length > 0) return ((LocalizedEnumDisplayName)attrs[0]).ToString();

                        return name;
                    }
                    else return name;
                }
                catch
                {
                    return name;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    #endregion

    #region LocalizedEnumTypeConverter
    public class LocalizedEnumTypeConverter : LocalizedToStringTypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string) == sourceType)
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (context.PropertyDescriptor.PropertyType.IsEnum)
            {
                try
                {
                    return Enum.Parse(context.PropertyDescriptor.PropertyType, value.ToString());
                }
                catch
                { }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {

            if (context.PropertyDescriptor.PropertyType.IsEnum)
            {
                var list = new List<string>();
                foreach (var item in Enum.GetNames(context.PropertyDescriptor.PropertyType))
                {
                    var attr = context.PropertyDescriptor.PropertyType.GetField(item).GetCustomAttributes(typeof(BrowsableAttribute), false);
                    if ((attr.Length == 0) || ((attr[0] as BrowsableAttribute).Browsable))
                        list.Add(item);
                }
                return new StandardValuesCollection(list);
            }

            return base.GetStandardValues(context);
        }
    }
    #endregion
}
