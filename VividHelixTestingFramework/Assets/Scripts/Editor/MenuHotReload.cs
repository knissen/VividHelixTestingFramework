// Copyright 2023 Vivid Helix
// This file is part of ViviHelixTestFramework.
// ViviHelixTestFramework is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// ViviHelixTestFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License along with ViviHelixTestFramework. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace VividHelix.HotReload
{
    public class MenuHotReload : EditorWindow
    {
        [MenuItem("VividHelix/Config HotSwap")]
        public static void ConfigureHotSwapProject()
        {
            Debug.Log("Running VividHelix HotReload configuration");

            HotReloadSettings settings = GetOrCreateSettings();

            if (settings == null)
            {
                Debug.LogError("Settings not found. Check console for error message.");
                return;
            }

            // Find Visual Studio project
            if (TryGetPathForProjectFile(out string projPath))
            {
                settings.gameCoreProjectFile = projPath;
                EditorUtility.SetDirty(settings);

                XmlDocument projDoc = new XmlDocument();
                projDoc.Load(projPath);

                SetupUnityInstallPath(projDoc);

                projDoc.Save(projPath);
            }
            else
            {
                Debug.Log("Config Cancelled");
                return;
            }

            // Prompt for path to Unity install

            // 

            AssetDatabase.SaveAssetIfDirty(settings);
            AssetDatabase.Refresh();

            Selection.activeObject = settings;
        }

        private static void SetupUnityInstallPath(XmlDocument projDoc)
        {
            XmlNodeList nodes = projDoc.GetElementsByTagName("UnityInstallFolder");

            if (nodes.Count == 1)
            {
                nodes[0].InnerText = Path.GetDirectoryName(EditorApplication.applicationPath) + @"\Data\";
            }
        }

        private static HotReloadSettings GetOrCreateSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:HotReloadSettins");

            if (guids.Length > 1)
            {
                Debug.LogError("Too many HotReloadSettings found. There shoudl be only one. Using first found by default");
            }

            if (guids.Length == 0)
            {
                HotReloadSettings settings = ScriptableObject.CreateInstance(typeof(HotReloadSettings)) as HotReloadSettings;

                AssetDatabase.CreateAsset(settings, "Assets/HotReloadSettings.asset");

                return settings;
            }

            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(HotReloadSettings)) as HotReloadSettings;
        }

        private static bool TryGetPathForProjectFile(out string projPath)
        {
            projPath = EditorUtility.OpenFilePanel("Find GameCore Project File", "../", "csproj");

            if (string.IsNullOrEmpty(projPath))
                return false;

            return true;
        }
    } 
}
