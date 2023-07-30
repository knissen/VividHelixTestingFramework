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
