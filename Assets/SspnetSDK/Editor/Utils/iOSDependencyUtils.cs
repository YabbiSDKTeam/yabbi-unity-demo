using System;
using System.IO;

namespace SspnetSDK.Editor.Utils
{
    public class iOSDependencyUtils
    {
        public static string GetiOSContent(string path)
        {
            var iOSContent = string.Empty;
            try
            {
                var lines = File.ReadAllLines(path.Replace("Consentmanager", "Consent"));
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.Contains("<iosPods>")) iOSContent += line + "\n";

                    if (line.Contains("<iosPod name=")) iOSContent += line + "\n";

                    if (line.Contains("<sources>")) iOSContent += line + "\n";

                    if (line.Contains("<source>")) iOSContent += line + "\n";

                    if (line.Contains("</sources>")) iOSContent += line + "\n";

                    if (line.Contains("</iosPod>")) iOSContent += line + "\n";

                    if (line.Contains("</iosPods>")) iOSContent += line;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return iOSContent;
        }
    }
}