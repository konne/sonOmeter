using System;
using System.Text;
using sonOmeter.Server.Classes;
using UKLib.Debug;
using System.IO;
using System.Xml.Serialization;

namespace sonOmeter.ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            sonOmeterServerClass serv = new sonOmeterServerClass();
            try
            {
                var sr = File.OpenRead("servercfg.xml");
                var xs = new XmlSerializer(typeof(sonOmeterServerConfig));
                serv.Settings = (sonOmeterServerConfig)xs.Deserialize(sr);
                sr.Close();             
            }
            catch (Exception ex)
            {
            }

            DebugClass.SynchronizingObject = null;
            DebugClass.OnDebugLines += new DebugLineEventHandler(DebugClass_OnDebugLines);
            if (serv == null) serv = new sonOmeterServerClass();

            //serv.StartConfigServer(9100);

            serv.Settings.SonID0Timer = false;
            serv.StartDataServer();
            Console.WriteLine("To Exit press 'q'+'Enter'.");
            string line = "";
            do
            {
                line = Console.ReadLine();

            } while (line != "q");

            serv.StopDataServer();
            //serv.StopConfigServer();

            TextWriter tw = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(sonOmeterServerClass));
                tw = File.CreateText("servercfg.xml");
                xs.Serialize(tw, serv.Settings);
            }
            catch
            {
            }
            if (tw != null) tw.Close();
        }     
       
        static void DebugClass_OnDebugLines(object sender, DebugLineEventArgs e)
        {
           if ((e.Level != DebugLevel.White) && (e.Level != DebugLevel.Green) && (e.Level != DebugLevel.Yellow))
            {               
                    Console.WriteLine(sender.GetType().ToString() + " - " + e.DebugLine+ "\r\n");
                    Console.WriteLine("---------------------------------------------");              
            } 
        }       
    }
}
