using System.IO;
using System.Text.RegularExpressions;

namespace SspnetSDK.Editor.Utils
{
    public class FileUtils
    {
        public static void ReplaceInFile(
            string filePath, string searchText, string replaceText)
        {
            string contentString;
            using (var reader = new StreamReader(filePath))
            {
                contentString = reader.ReadToEnd();
                reader.Close();
            }

            contentString = Regex.Replace(contentString.Replace("\r", ""), searchText, replaceText);

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(contentString);
                writer.Close();
            }
        }
    }
}