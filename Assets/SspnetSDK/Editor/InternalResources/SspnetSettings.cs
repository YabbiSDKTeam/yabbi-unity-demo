using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SspnetSDK.Editor.InternalResources
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SspnetSettings : ScriptableObject 
    {
        private const string SspnetSettingsExportPath = "SspnetSDK/Editor/InternalResources/SspnetSettings.asset";
        private static SspnetSettings instance;

        [SerializeField] private bool accessFineLocationPermission = true;
        [SerializeField] private bool accessCoarseLocationPermission = true;

        [SerializeField] private bool androidMultidex = true;

        [SerializeField] private bool nSUserTrackingUsageDescription = true;
        [SerializeField] private bool nSLocationWhenInUseUsageDescription = true;

        [SerializeField] private bool iOSSKAdNetworkItems = true;
        [SerializeField] private List<string> iOsskAdNetworkItemsList;
        
        public static SspnetSettings Instance
        {
            get
            {
                if (instance != null) return instance;
                var settingsFilePath = Path.Combine("Assets", SspnetSettingsExportPath);
                var settingsDir = Path.GetDirectoryName(settingsFilePath);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir ?? string.Empty);
                }

                instance = AssetDatabase.LoadAssetAtPath<SspnetSettings>(settingsFilePath);
                if (instance != null) return instance;
                instance = CreateInstance<SspnetSettings>();
                AssetDatabase.CreateAsset(instance, settingsFilePath);

                return instance;
            }
        }
        
         public bool AccessCoarseLocationPermission
        {
            get => accessCoarseLocationPermission;
            set => Instance.accessCoarseLocationPermission = value;
        }

        public bool AccessFineLocationPermission
        {
            get => accessFineLocationPermission;
            set => Instance.accessFineLocationPermission = value;
        }

        public bool AndroidMultidex
        {
            get => androidMultidex;
            set => Instance.androidMultidex = value;
        }

        public bool NSUserTrackingUsageDescription
        {
            get => nSUserTrackingUsageDescription;
            set => Instance.nSUserTrackingUsageDescription = value;
        }

        public bool NSLocationWhenInUseUsageDescription
        {
            get => nSLocationWhenInUseUsageDescription;
            set => Instance.nSLocationWhenInUseUsageDescription = value;
        }

        public bool IOSSkAdNetworkItems
        {
            get => iOSSKAdNetworkItems;
            set => Instance.iOSSKAdNetworkItems = value;
        }

        public List<string> IOSSkAdNetworkItemsList
        {
            get => iOsskAdNetworkItemsList;
            set => Instance.iOsskAdNetworkItemsList = value;
        }

        public void SaveAsync()
        {
            EditorUtility.SetDirty(instance);
        }
    }
}