namespace BigTwo
{
    public struct EventGameState
    {
        private static EventGameState m_event;

        public GameState GameState { get; set; }

        public static void Invoke(GameState gameState)
        {
            m_event.GameState = gameState;
            EventManager.Invoke(m_event);
        }
    }
}