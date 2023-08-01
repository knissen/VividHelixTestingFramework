// Copyright 2023 Vivid Helix
// This file is part of ViviHelixTestFramework.
// ViviHelixTestFramework is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// ViviHelixTestFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License along with ViviHelixTestFramework. If not, see <https://www.gnu.org/licenses/>.

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
