using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.CSharp;
using sonOmeter.Classes.Sonar2D;
using UKLib.ErrorList;
using UKLib.Xml;


namespace sonOmeter.Classes
{
    /// <summary>
    /// This struct provides config switches for specialized sonar line export (CSV files).
    /// </summary>    
    public class SonarExport
    {
        #region Variables
        private ExportSettings expSettings;

        Assembly asExport = null;
        #endregion

        #region Properties
        public bool CompileOK
        {
            get
            {
#if DEBUG
                return true;
#else
                return asExport != null;
#endif
            }
        }

        public ExportSettings ExpSettings
        {
            get { return expSettings; }
        }
        #endregion

        #region Serialization
        public void Deserialize(string fileName)
        {
            foreach (var item in expSettings.AvExportCustomer)
            {
                try
                {
                    var extTypes = new List<Type>() { item.GetType() };
                    XMLSer<CustomerExport> x = new XMLSer<CustomerExport>(extTypes);
                    expSettings.Customer = x.Deserialize(new XmlTextReader(fileName));
                    break;
                }
                catch
                {
                }
            }
        }

        public void Serialize(string fileName)
        {
            var tw = new XmlTextWriter(fileName, Encoding.UTF8);
            try
            {
                var extTypes = new List<Type>() { expSettings.Customer.GetType() };
                XMLSer<CustomerExport> x = new XMLSer<CustomerExport>(extTypes);

                x.Serialize(tw, expSettings.Customer);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                tw.Close();
            }

        }
        #endregion

        #region Constructors
        public SonarExport()
        {
            this.expSettings = new ExportSettings();
        }
        #endregion

        #region Compiler
        private CompilerResults Compile(List<ErrorListItem> CompilerErrors)
        {
            CodeDomProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.CompilerOptions = "/target:library /optimize /define:DEBUG";
            compilerParams.GenerateExecutable = false;
            compilerParams.WarningLevel = 4;
            // compilerParams.IncludeDebugInformation = true;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerParams.ReferencedAssemblies.Add("System.Xml.dll");
            compilerParams.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "UKLib.dll");
            compilerParams.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "sonOmeter.Classes.dll");


            string s = "";
            for (int i = 0; i < expSettings.Exportfunction.Length; i++)
            {
                s += expSettings.Exportfunction[i].Replace("\r", "").Replace("\n", "") + "\r\n";
            }
            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, s);

            CompilerErrors.Clear();
            bool realerror = false;

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    if (CompilerErrors != null)
                    {
                        if (error.IsWarning)
                            CompilerErrors.Add(new ErrorListItem(ErrorStatus.Warning, error.ErrorText, error.Line, error.Column));
                        else
                        {
                            realerror = true;
                            CompilerErrors.Add(new ErrorListItem(ErrorStatus.Error, error.ErrorText, error.Line, error.Column));
                        }
                    }
                }
            }

            return realerror ? null : results;
        }
        #endregion

        #region CompileExportFunc
        public void CompileExportFunc(List<ErrorListItem> CompilerErrors)
        {
            asExport = null;

#if DEBUG
            asExport = Assembly.GetExecutingAssembly();
#endif

#if !DEBUG
            CompilerResults results = Compile(CompilerErrors);

            if (results != null)
                asExport = results.CompiledAssembly;

#endif
            FindExportClasses(asExport);
        }

        #region FindExportClasses
        private void FindExportClasses(Assembly ass)
        {
            expSettings.AvExportCustomer.Clear();
            if (ass == null)
                return;
            Type[] types;
            try
            {
                types = ass.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            foreach (var item in types)
            {
                if (item != null)
                    if (item.IsSubclassOf(typeof(CustomerExport)))
                    {
                        try
                        {
                            expSettings.AvExportCustomer.Add(item.GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes) as CustomerExport);
                        }
                        catch
                        { }
                    }
            }
        }
        #endregion

        #endregion

        #region ExportSonarLine Wrapper
        public string ExportBeginRecord(SonarRecord record)
        {
            try
            {
                return expSettings.Customer.ExportBeginRecord(record);
            }
            catch
            {
                return ""; // im Errorfall nichts zurückgeben
            }
        }

        public string ExportEndRecord(SonarRecord record)
        {
            try
            {
                return expSettings.Customer.ExportEndRecord(record);
            }
            catch
            {
                return ""; // im Errorfall nichts zurückgeben
            }
        }

        public string ExportSonarLine(SonarLine line, SonarPanelType type, int recordnr, DateTime time)
        {
            try
            {
                return expSettings.Customer.ExportLine(line, type, expSettings, recordnr, time);
            }
            catch
            {
                return ""; // im Errorfall nichts zurückgeben
            }
        }

        public string ExportPoint(ManualPoint p, int RecordNr)
        {
            try
            {
                return expSettings.Customer.ExportPoint(p, RecordNr, expSettings);
            }
            catch
            {
                return ""; // im Errorfall nichts zurückgeben
            }
        }

        public string ExportLog
        {
            get
            {
                try
                {
                    return expSettings.Customer.log;
                }
                catch
                {
                    return ""; // im Errorfall nichts zurückgeben
                }
            }
        }
        #endregion
    }
}