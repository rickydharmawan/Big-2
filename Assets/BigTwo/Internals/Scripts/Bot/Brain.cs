namespace BigTwo.Bot
{
    public class Brain<T>
    {
        private T m_owner;
        public IState<T> CurrentState { get; private set; }

        public Brain(T owner, IState<T> state)
        {
            m_owner = owner;
            ChangeState(state);
        }

        public void ChangeState(IState<T> state)
        {
            CurrentState?.OnExit(this, m_owner);

            CurrentState = state;

            CurrentState.OnEnter(this, m_owner);
        }

        public void Update()
        {
            CurrentState?.OnUpdate(this, m_owner);
        }
    }
}