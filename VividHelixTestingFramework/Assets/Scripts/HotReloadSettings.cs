using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VividHelix.HotReload
{
    [CreateAssetMenu(fileName = "HotReloadSettings", menuName = "VividHelix/HotReloadSettings")]
    public class HotReloadSettings : ScriptableObject
    {
        [Tooltip("Csproj file containing external project to reference")]
        public string gameCoreProjectFile;

        [Tooltip("Namespace used by core project, used when loading updated assembly")]
        public string Namespace = "VividHelix.HotReload.GameCore";
        public string ClassName = "GameMain";
        public string DllName = "GameCore.dll";
        public string PdbName = "GameCore.pdb";
        public string DllBytesDirectory = "Assets//GameCore//Resources//";
    } 
}
