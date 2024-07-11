#pragma warning disable 0649
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;

namespace SspnetSDK.Editor.Utils
{
    [System.Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class RemovableItem
    {
        public string name;
        public bool is_confirmation_required;
        public string path;
        public string description;
        public bool check_if_empty;
        public bool perform_only_if_total_remove;
        public string filter;
    }


    [InitializeOnLoad]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantJumpStatement")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class RemoveUtils
    {
        public static void RemovePlugin(string path, bool isCleanBeforeUpdate = false)
        {
            RemoveFiles(isCleanBeforeUpdate, path);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static void RemoveConsentManager(string path, bool isCleanBeforeUpdate = false)
        {
            RemoveFiles(isCleanBeforeUpdate, path);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void RemoveFiles(bool isCleanBeforeUpdate, string pathToRemoveList)
        {
            var items = readXML(pathToRemoveList);
            foreach (var t in items)
            {
                if (t.perform_only_if_total_remove && isCleanBeforeUpdate) continue;
                var confirmed = !t.is_confirmation_required || isCleanBeforeUpdate;
                var fullItemPath = Path.Combine(Application.dataPath, t.path);

                if (!confirmed)
                    if (EditorUtility.DisplayDialog("Remove " + t.name, t.description, "Yes", "No"))
                        confirmed = true;

                if (!confirmed) continue;
                var isChecked = !t.check_if_empty;
                if (!isChecked) isChecked = isFolderEmpty(fullItemPath);
                if (!isChecked) continue;

                if (string.IsNullOrEmpty(t.filter))
                {
                    FileUtil.DeleteFileOrDirectory(fullItemPath);
                    FileUtil.DeleteFileOrDirectory(fullItemPath + ".meta");
                    continue;
                }

                var isDirectoryExists = Directory.Exists(fullItemPath);
                if (!isDirectoryExists) continue;
                var filesList =
                    new List<string>(Directory.GetFiles(fullItemPath, "*", SearchOption.TopDirectoryOnly));
                filesList.AddRange(Directory.GetDirectories(fullItemPath, "*", SearchOption.TopDirectoryOnly));
                foreach (var t1 in from t1 in filesList
                         let fileName = Path.GetFileName(t1)
                         where Regex.IsMatch(fileName, t.filter, RegexOptions.IgnoreCase)
                         select t1)
                {
                    FileUtil.DeleteFileOrDirectory(t1);
                    FileUtil.DeleteFileOrDirectory(t1 + ".meta");
                }

                if (!isFolderEmpty(fullItemPath)) continue;
                FileUtil.DeleteFileOrDirectory(fullItemPath);
                FileUtil.DeleteFileOrDirectory(fullItemPath + ".meta");
            }
        }

        private static IEnumerable<RemovableItem> readXML(string path)
        {
            var itemToRemoveList = new List<RemovableItem>();
            var xDoc = new XmlDocument();
            xDoc.Load(Path.Combine(Application.dataPath, path));
            var xRoot = xDoc.DocumentElement;

            if (xRoot == null) return itemToRemoveList.ToArray();
            foreach (XmlNode xnode in xRoot)
            {
                var itemToRemove = new RemovableItem();
                foreach (XmlNode childNode in xnode.ChildNodes)
                {
                    if (childNode.Name.Equals("name")) itemToRemove.name = childNode.InnerText;

                    if (childNode.Name.Equals("is_confirmation_required"))
                    {
                        if (childNode.InnerText.Equals("true"))
                            itemToRemove.is_confirmation_required = true;
                        else if (childNode.InnerText.Equals("true")) itemToRemove.is_confirmation_required = false;
                    }

                    if (childNode.Name.Equals("path")) itemToRemove.path = childNode.InnerText;

                    if (childNode.Name.Equals("description")) itemToRemove.description = childNode.InnerText;

                    if (childNode.Name.Equals("check_if_empty"))
                    {
                        if (childNode.InnerText.Equals("true"))
                            itemToRemove.check_if_empty = true;
                        else if (childNode.InnerText.Equals("false")) itemToRemove.check_if_empty = false;
                    }

                    if (childNode.Name.Equals("perform_only_if_total_remove"))
                    {
                        if (childNode.InnerText.Equals("true"))
                            itemToRemove.perform_only_if_total_remove = true;
                        else if (childNode.InnerText.Equals("false")) itemToRemove.perform_only_if_total_remove = false;
                    }

                    if (childNode.Name.Equals("filter")) itemToRemove.filter = childNode.InnerText;
                }

                itemToRemoveList.Add(itemToRemove);
            }

            return itemToRemoveList.ToArray();
        }

        private static bool isFolderEmpty(string path)
        {
            if (!Directory.Exists(path)) return false;
            var filesPaths = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            var s = new List<string>(filesPaths);
            for (var i = 0; i < s.Count; i++)
                if (s[i].Contains(".DS_Store"))
                    s.RemoveAt(i);

            return s.Count == 0;
        }
    }
}