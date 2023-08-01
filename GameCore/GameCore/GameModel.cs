// Copyright 2023 Vivid Helix
// This file is part of ViviHelixTestFramework.
// ViviHelixTestFramework is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// ViviHelixTestFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License along with ViviHelixTestFramework. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VividHelix.HotReload.GameCore.Models
{
    public abstract class GameModel
    {
        private bool initCalled;
        private bool destroyCalled;
        private bool updateCalled;

        protected bool active = true;

        public List<GameModel> Children = new List<GameModel>();
        
        public GameModel Parent;

        public GameModel(bool active = true, params GameModel[] children)
        {
            this.active = active;

            children.Each(x => x.Children.Add(x));

            GameMain.Register(this);
        }

        public void RemoveAndDestroy(GameModel gameModel)
        {
            if (gameModel == null)
                return;
            if (Children.Contains(gameModel))
                Children.Remove(gameModel);

            gameModel.Destroy();
        }

        public void Add(GameModel gameModel)
        {
            if (Children.Contains(gameModel))
                Debug.LogError($"Child model {gameModel} added multiple times to {this}! ");

            Children.Add(gameModel);
            gameModel.Parent = this;
        }

        public GameModel AddAndInit(GameModel gameModel)
        {
            Add(gameModel);
            gameModel.Init();
            return gameModel;
        }

        public virtual void SaveReloadState(Dictionary<string, object> state)
        {
            Children.Each(x => x.SaveReloadState(state));
        }

        public virtual void LoadReloadState(Dictionary<string, object> state)
        {
            Children.Each(x => x.LoadReloadState(state));
        }

        protected virtual void Init()
        {
            initCalled = true;
        }

        protected virtual void Update()
        {
            updateCalled = true;
        }

        protected virtual void Destroy()
        {
            destroyCalled = true;
        }

        public void SafeInit()
        {
            if (initCalled)
                return;
            Init();
            if (!initCalled)
                Debug.LogError($"Init not called for ${GetType().Name}");
            Children.Each(x => x.SafeInit());
        }

        public void SafeUpdate()
        {
            if (!active)
                return;
            Update();
            if (!updateCalled)
                Debug.LogError($"Update not called for ${GetType().Name}");
            for (var i = 0; i < Children.Count; i++)
                if (Children[i].active)
                    Children[i].SafeUpdate();
        }

        public void SafeDestroy()
        {
            Destroy();
            if (!destroyCalled)
                Debug.LogError($"Destroy not called for ${GetType().Name}");
            Children.Each(x => x.SafeDestroy());
        }
    }
}
