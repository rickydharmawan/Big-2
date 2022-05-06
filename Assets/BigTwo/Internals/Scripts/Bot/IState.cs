namespace BigTwo.Bot
{
    public interface IState<T>
    {
        void OnEnter(Brain<T> brain, T t);
        void OnUpdate(Brain<T> brain, T t);
        void OnExit(Brain<T> brain, T t);
    }
}