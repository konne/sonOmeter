using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace UKLib.ExtendedAttributes
{
    #region Localize Attributes
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class LocalizedResourcePropertyAttribute : Attribute
    {
        #region Variables & Properties
        public string DisplayNameFieldName { get; set; }
        public string DescriptionFieldName { get; set; }
        public string CategoryFieldName { get; set; }
        public string ResourceTable { get; set; }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class LocalizedPropertyAttribute : Attribute
    {
        #region Constructor
        public LocalizedPropertyAttribute(string sCulture)
        {
            try
            {
                Culture = CultureInfo.GetCultureInfo(sCulture);
            }
            catch
            {
            }
        }
        #endregion

        #region Variables & Properties
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public CultureInfo Culture { get; private set; }
        #endregion

        #region override
        public override int GetHashCode()
        {
            if (Culture != null) return Culture.GetHashCode();
            return base.GetHashCode();
        }
        #endregion
    }
    #endregion,

    #region ResourceLookup Helper
    public static class ResourceLookup
    {
        private static Object lockObject = new Object();
        private static Dictionary<Type, ResourceManager> resManagerCache = null;
        public static ResourceManager GetResourceManager(Type T)
        {
            lock (lockObject)
            {
                if (resManagerCache == null)
                    resManagerCache = new Dictionary<Type, ResourceManager>();
                if (resManagerCache.ContainsKey(T))
                    return resManagerCache[T];
            }

            foreach (PropertyInfo staticProperty in T.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    var resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    lock (lockObject)
                    {
                        if (!resManagerCache.ContainsKey(T))
                            resManagerCache.Add(T, resourceManager);
                    }
                    return resourceManager;
                }
            }

            return null;
        }

        public static string GetResourceString(Type T, string resourceKey)
        {
            return GetResourceString(T, resourceKey, "");
        }


        public static string GetResourceString(Type T, string resourceKey, string preString)
        {
            var resm = ResourceLookup.GetResourceManager(T);
            if (resm != null)
            {
                var name = resm.GetString(resourceKey);
                if (name != null)
                {
                    return preString + name;
                }
                else
                {
#if DEBUG
                    System.Diagnostics.Trace.WriteLine(resourceKey + "\t" + resourceKey);
#endif
                }
            }
            return resourceKey;
        }
    }
    #endregion

    #region LocalizedDisplayNameAttribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false)]
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(Type resourceManagerProvider, string resourceKey)
            : this(resourceManagerProvider, resourceKey, false) { }

        public LocalizedDisplayNameAttribute(Type resourceManagerProvider, string resourceKey, bool appendDisplayName)
            : base(ResourceLookup.GetResourceString(resourceManagerProvider, resourceKey + (appendDisplayName ? "_DisplayName" : ""))) { }
    }
    #endregion

    #region LocalizedDescriptionAttribute
    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(Type resourceManagerProvider, string resourceKey)
            : this(resourceManagerProvider, resourceKey, false) { }

        public LocalizedDescriptionAttribute(Type resourceManagerProvider, string resourceKey, bool appendDescriptionName)
            : base(ResourceLookup.GetResourceString(resourceManagerProvider, resourceKey + (appendDescriptionName ? "_Description" : ""))) { }
    }
    #endregion

    #region LocalizedCategoryAttribute
    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(Type resourceManagerProvider, string resourceKey)
            : this(resourceManagerProvider, resourceKey, false, "") { }

        public LocalizedCategoryAttribute(Type resourceManagerProvider, string resourceKey, bool appendDisplayName)
            : this(resourceManagerProvider, resourceKey, appendDisplayName, "") { }

        public LocalizedCategoryAttribute(Type resourceManagerProvider, string resourceKey, bool appendDisplayName, string preString)
            : base(ResourceLookup.GetResourceString(resourceManagerProvider,
            resourceKey + (appendDisplayName ? "_Category" : ""), preString)) { }
    }
    #endregion

    #region LocalizedEnumDisplayName
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizedEnumDisplayName : Attribute
    {
        private string displayName = "";

        public LocalizedEnumDisplayName(Type resourceManagerProvider, string resourceKey, bool AppendEnumDisplayName)
        {
            displayName = ResourceLookup.GetResourceString(resourceManagerProvider, resourceKey + (AppendEnumDisplayName ? "_EnumDisplayName" : ""));
        }

        public LocalizedEnumDisplayName(Type resourceManagerProvider, string resourceKey) :
            this(resourceManagerProvider, resourceKey, false) { }

        public override string ToString()
        {
            return displayName;
        }
    }
    #endregion
}
