using System.IO;

namespace SspnetSDK.Editor.Utils
{
    public class AndroidDependencyUtils
    {
        public static string GetAndroidDependencyVersion(string value)
        {
            var androidDependencyVersion = value.Substring(value.LastIndexOf(':') + 1);
            if (androidDependencyVersion.Contains("@aar"))
                androidDependencyVersion = androidDependencyVersion.Substring(0,
                    androidDependencyVersion.IndexOf('@'));

            return androidDependencyVersion;
        }

        public static string GetAndroidContent(string path)
        {
            var androidContent = string.Empty;
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (line.Contains("<androidPackages>")) androidContent += line + "\n";

                if (line.Contains("<androidPackage spec=")) androidContent += line + "\n";

                if (line.Contains("<repositories>")) androidContent += line + "\n";

                if (line.Contains("<repository>")) androidContent += line + "\n";

                if (line.Contains("</repositories>")) androidContent += line + "\n";

                if (line.Contains("</androidPackages>")) androidContent += line;
            }

            return androidContent;
        }
    }
}