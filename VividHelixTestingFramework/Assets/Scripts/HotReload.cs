using DefaultNamespace;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace VividHelix.HotReload
{
    public class HotReload : MonoBehaviour
    {
        public bool AutoReload;
        public bool LogToConsole;
        public KeyCode ForceReloadKey = KeyCode.Alpha0;

        public HotReloadSettings settings;

        private IGameCore gameCore;

#if UNITY_EDITOR
        private string DllDirectory => settings.DllBytesDirectory;
#else
    private static string DllDirectory = "Assets\\GameCore\\Resources\\";
#endif

        private string DllPath => DllDirectory + settings.DllName + ".bytes";
        private string PdbPath => DllDirectory + settings.PdbName + ".bytes";

        private DateTime lastModifiedTime;

        #region UnityLifecycle Events

        public void Start()
        {
            if (settings == null)
            {
                Debug.LogError("No Settings set for Hot Reload Component. Check for empty reference on component. Run setup from the VividHelix menu to create settings asset.");

                Destroy(this);

                return;
            }

            if (!AutoReload)
                ReloadDLLFile(new Stopwatch(), null, false);
        }

        public void Update()
        {
            try
            {
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX) && UNITY_EDITOR
                ReloadDllIfNeeded();
#endif
                gameCore?.Update();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Reload Methods

        private void ReloadDllIfNeeded()
        {
            if ((AutoReload && (File.GetLastWriteTime(DllPath) != lastModifiedTime) || Input.GetKeyDown(ForceReloadKey)))
            {
                if (LogToConsole)
                    Debug.LogError($"Reloading: {lastModifiedTime} // {File.GetLastWriteTime(DllPath)}");

                var stopwatch = new Stopwatch();
                var saved = gameCore?.Save(true);

                stopwatch.Start();
                var shouldLogReload = lastModifiedTime.Ticks != 0;

                ReloadDLLFile(stopwatch, saved, shouldLogReload);
            }
        }

        private void ReloadDLLFile(Stopwatch stopwatch, object saved, bool shouldLogReload)
        {
            lastModifiedTime = File.GetLastWriteTime(DllPath);
            var dllBytes = File.ReadAllBytes(DllPath);
            var mdbBytes = File.ReadAllBytes(PdbPath);

            if (shouldLogReload)
                Debug.LogError($"Reloaded dll took {stopwatch.ElapsedMilliseconds / 1000f:0.000}");

            LoadDll(dllBytes, mdbBytes, saved);
        }

        private void LoadDll(byte[] dllBytes, byte[] mdbBytes = null, object saved = null)
        {
            var assembly = (mdbBytes != null) ? Assembly.Load(dllBytes, mdbBytes) : Assembly.Load(dllBytes);
            var gameCoreType = assembly.ExportedTypes.FirstOrDefault(x => x.Name.Equals(settings.ClassName) && x.Namespace.Equals(settings.Namespace));
            gameCore = Activator.CreateInstance(gameCoreType) as IGameCore;
            gameCore.Load(saved);
        }

        #endregion
    } 
}
