namespace RPG.Core
{
    public interface IAction
    {
        string Name { get; }
        void Cancel();
    }
}