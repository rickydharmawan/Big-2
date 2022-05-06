namespace BigTwo
{
    public struct EventPlayerTurn
    {
        private static EventPlayerTurn m_event;

        public Player Player { get; set; }

        public static void Invoke(Player player)
        {
            m_event.Player = player;
            EventManager.Invoke(m_event);
        }
    }
}
