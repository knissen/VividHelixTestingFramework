using System;
using System.Collections.Generic;
using System.Linq;
using VividHelix.HotReload.GameCore.Models;
using UnityEngine;
using DefaultNamespace;

namespace VividHelix.HotReload.GameCore
{
    public class GameMain : IGameCore
    {
        public static GameMain Instance { get; private set; }

        public float deltaTime;

        private GameModel rootModel;
        private GameObject[] viewMarkers;

        private static Dictionary<Type, GameModel> registry = new Dictionary<Type, GameModel>();

        public static void Register<T>(T model) where T : GameModel
        {
            registry[model.GetType()] = model;
        }

        public void Load(object previous)
        {
            Debug.Log("Loading GameCore state");

            Instance = this;

            try
            {
                viewMarkers = Resources.FindObjectsOfTypeAll<ViewMarker>().Select(x => x.gameObject).ToArray();

                rootModel = new RootModel(registry);

                if (previous != null)
                    rootModel.LoadReloadState(previous as Dictionary<string, object>);

                rootModel.SafeInit();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public object Save(bool destroy)
        {
            Debug.Log("Saving GameCore state");

            if (destroy)
            {
                var saved = new Dictionary<string, object>();
                rootModel.SaveReloadState(saved);
                rootModel.SafeDestroy();
                return saved;
            }

            return null;
        }

        public void Update()
        {
            try
            {
                deltaTime = Time.deltaTime;

                rootModel.SafeUpdate();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
