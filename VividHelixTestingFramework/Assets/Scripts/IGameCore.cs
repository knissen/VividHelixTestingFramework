namespace DefaultNamespace
{
    public interface IGameCore
    {
        object Save(bool destroy);
        void Load(object previous);
        void Update();
    }
}