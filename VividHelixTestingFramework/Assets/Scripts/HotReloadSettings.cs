// Copyright 2023 Vivid Helix
// This file is part of ViviHelixTestFramework.
// ViviHelixTestFramework is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// ViviHelixTestFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License along with ViviHelixTestFramework. If not, see <https://www.gnu.org/licenses/>.

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
