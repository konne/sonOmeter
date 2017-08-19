using System;
using System.Collections.Generic;
using System.Text;
using UKLib.DXF;
using System.IO;

namespace sonOmeter.Classes
{
    public class DXFFile
    {
        private DXFContainer dxf = new DXFContainer();
        private bool showInTrace = false;
        private string fileName = "DXF";

        public bool ShowInTrace
        {
            get { return showInTrace; }
            set { showInTrace = value; }
        }

        public DXFContainer DXF
        {
            get { return dxf; }
            set { dxf = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public DXFFile()
        {
        }

        public DXFFile(SonarProject project, string file, bool copy)
        {
            fileName = Path.GetFileNameWithoutExtension(file);

            dxf.ReadFile(file);
            dxf.OnUpdate();
            
            // Add it to the project and the window list.
            project.DXFFiles.Add(this);

            // Set to binary project mode.
            project.IsBinary = true;

            // Copy the file to the temporary folder.
            if (copy)
            {
                if (!SonarProject.TempPathExists)
                    SonarProject.CreateTempPath();

                string newFile = SonarProject.TempPath + "\\dxf\\" + fileName + ".dxf";

                File.Copy(file, newFile, true);
            }
        }
        
        public void Dispose()
        {
            dxf.Dispose();
        }
    }
}
