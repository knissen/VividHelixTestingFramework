using System;
using System.Collections.Generic;
using UnityEngine;
using VividHelix.HotReload.GameCore.Models;
using VividHelix.HotReload.Tests;

namespace VividHelix.HotReload.GameCore
{
    public class RootModel : GameModel
    {
        private readonly Dictionary<Type, GameModel> registry;

        public RootModel(Dictionary<Type, GameModel> registry)
        {
            this.registry = registry;
        }

        protected override void Init()
        {
            base.Init();

            Debug.Log("Init Root Model");

            // Add Custom GameModel's to root here
            //AddModel(new SomeGameModel());

            AddModel(new TestRunner());
        }

        private T AddModel<T>(T model) where T : GameModel
        {
            AddAndInit(model);
            registry[typeof(T)] = model;
            return model;
        }
    }
}
