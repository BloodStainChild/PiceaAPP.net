using System;
using System.IO;
using System.Windows.Forms;

namespace PiceaWindowsFormsApp
{
    class ConfigManager
    {
        private static readonly string filePath = "config.txt";


        public static bool SaveConfig(string PiceaIP, string PiceaPort, string jwt, string deviceid)
        {
            try
            {
                File.WriteAllLines(filePath, new string[]
                {
                PiceaIP,
                PiceaPort,
                jwt,
                deviceid
                });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Konfiguration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool LoadConfig(out string PiceaIP, out string PiceaPort, out string jwt, out string deviceid)
        {
            PiceaIP = "";
            PiceaPort = "";
            jwt = "";
            deviceid = "";

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length >= 3)
                    {
                        PiceaIP = lines[0];
                        PiceaPort = lines[1];
                        jwt = lines[2];
                        deviceid = lines[3];
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Konfiguration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
