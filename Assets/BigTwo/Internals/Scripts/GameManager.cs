using DG.Tweening;
using UnityEngine;
using Wonderednow;

namespace BigTwo
{
    public enum GameState
    {
        None,
        Setup,
        Preparation,
        Start,
        TurnBegin,
        TurnTransition,
        TurnEnd,
        End,
        Result,
    }

    public class GameManager : MonoBehaviour, EventListener<EventGameState>
    {
        protected static GameManager m_instance;

        public static GameManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        protected Sequence m_sequenceDistributeCards;

        protected GameState m_gameState;
        [SerializeField]
        protected Field m_field;
        [SerializeField]
        protected Player[] m_players;

        protected int m_round;
        protected int m_turn;
        protected int m_turnOffset; //To determine which player has the three diamond at the start of the game

        protected int m_passCount;

        [SerializeField]
        protected UIGameController uiGameController;
        [SerializeField]
        protected ObjectPool m_objectPool;

        public int PlayerTurnIndex
        {
            get
            {
                return (m_turn + m_turnOffset) % m_players.Length;
            }
        }

        public Player PlayerTurn
        {
            get
            {
                return m_players[PlayerTurnIndex];
            }
        }

        public GameState GameState
        {
            get
            {
                return m_gameState;
            }
        }

        public bool IsFirstTurn
        {
            get
            {
                return m_turn == 0;
            }
        }

        public ObjectPool ObjectPool
        {
            get
            {
                return m_objectPool;
            }
        }

        protected void OnValidate()
        {
            Initialize();
        }
        protected virtual void OnEnable()
        {
            EventManager.AddListener<EventGameState>(this);
        }

        protected virtual void OnDisable()
        {
            EventManager.RemoveListener<EventGameState>(this);
        }

        protected void Awake()
        {
            Initialize();
        }

        protected void Start()
        {
            ChangeGameState(GameState.Setup);
        }

        public void Initialize()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else if (m_instance != this)
            {
                gameObject.SetActive(false);
            }
        }

        public void ChangeGameState(GameState gameState)
        {
            m_gameState = gameState;
            EventGameState.Invoke(gameState);
        }

        public void DistributeCards()
        {
            CardData[] cardsData = new CardData[Constant.CARD_MAXIMUM_COUNT];
            for (int i = 0; i < cardsData.Length; i++)
            {
                int suit = i / Constant.CARD_RANK_MAX + 1;
                int rank = i % Constant.CARD_RANK_MAX + 1;

                cardsData[i] = new CardData(suit, rank);
            }

            int n = cardsData.Length;
            System.Random random = new System.Random();
            while (n > 1)
            {
                int k = random.Next(n--);
                CardData temporaryCardData = cardsData[n];
                cardsData[n] = cardsData[k];
                cardsData[k] = temporaryCardData;
            }

            int cardsPerPlayerCount = Constant.CARD_MAXIMUM_COUNT / m_players.Length;
            int cardsLeftoverCount = Constant.CARD_MAXIMUM_COUNT % m_players.Length;

            int playerWhoOwnsThreeDiamond = -1;
            for (int i = 0; i < m_players.Length; i++)
            {
                Player player = m_players[i];

                Card[] cards = new Card[cardsPerPlayerCount];
                for (int j = 0; j < cardsPerPlayerCount; j++)
                {
                    int cardsDataIndex = i * cardsPerPlayerCount + j;
                    CardData cardData = cardsData[cardsDataIndex];

                    if (cardData.Suit == 1 && cardData.Rank == 1)
                    {
                        m_turnOffset = i;
                        playerWhoOwnsThreeDiamond = i;
                    }

                    Card card = m_objectPool.Get("Card").GetComponent<Card>();
                    card.Initialize(cardData);

                    cards[j] = card;
                }

                player.InitializeHand(cards);
            }

            if (cardsLeftoverCount > 0)
            {
                Card[] cards = new Card[cardsLeftoverCount];
                for (int i = 0; i < cardsLeftoverCount; i++)
                {
                    int cardsDataIndex = m_players.Length * cardsLeftoverCount + i;
                    CardData cardData = cardsData[cardsDataIndex];

                    Card card = m_objectPool.Get("Card").GetComponent<Card>();
                    card.Initialize(cardData);
                    cards[i] = card;
                }
                m_players[playerWhoOwnsThreeDiamond].AddHandCard(cards);
            }

            m_sequenceDistributeCards = DOTween.Sequence();

            float totalAnimationDuration = 0f;

            for (int i = 0; i < cardsPerPlayerCount; i++)
            {
                for (int j = 0; j < m_players.Length; j++)
                {
                    Player player = m_players[j];
                    Vector3[] cardPositions = player.Hand.GetCardPositions();
                    Card card = player.Hand.ListOfCard[i];
                    card.transform.rotation = Quaternion.identity;
                    card.ToggleVisibility(false);
                    Vector3 targetRotation = Vector3.zero;
                    if (player is PlayerBot)
                    {
                        targetRotation = Vector3.up * 180f;
                    }
                    m_sequenceDistributeCards.Insert(totalAnimationDuration, card.transform.DOLocalMove(cardPositions[i], Constant.ANIMATION_DURATION));
                    m_sequenceDistributeCards.Insert(totalAnimationDuration, card.transform.DOLocalRotate(targetRotation, Constant.ANIMATION_DURATION));
                    totalAnimationDuration += Constant.ANIMATION_DURATION / 4f;
                }
            }

            m_sequenceDistributeCards.OnComplete(() =>
            {
                ChangeGameState(GameState.Start);
            });
        }

        public void RandomizeBotsAvatar()
        {
            for (int i = 0; i < m_players.Length; i++)
            {
                if (m_players[i] is PlayerBot playerBot)
                {
                    playerBot.UIAvatar.RandomizeAppearance();
                }
            }
        }

        public void NextRound()
        {
            m_round++;

            m_passCount = 0;
            m_field.Discard();
        }

        public void NextTurn()
        {
            ChangeGameState(GameState.TurnTransition);
            m_turn++;
            m_passCount = 0;

            EventPlayerTurn.Invoke(PlayerTurn);

            uiGameController.ShowTurnIndicator();
        }

        public void PassTurn()
        {
            ChangeGameState(GameState.TurnEnd);
            m_passCount++;

            if (m_passCount >= m_players.Length - 1)
            {
                NextRound();
            }

            uiGameController.ShowPassIndicator(() =>
            {
                ChangeGameState(GameState.TurnTransition);
                m_turn++;

                EventPlayerTurn.Invoke(PlayerTurn);

                uiGameController.ShowTurnIndicator();
            });
        }

        public void GameEnd()
        {
            ChangeGameState(GameState.Result);
        }

        public void GameResult()
        {

        }

        public void OnEventInvoked(EventGameState eventType)
        {
            switch (eventType.GameState)
            {
                case GameState.Setup:
                    RandomizeBotsAvatar();
                    break;
                case GameState.Preparation:
                    DistributeCards();
                    break;
                case GameState.Start:
                    uiGameController.ShowTurnIndicator();

                    EventPlayerTurn.Invoke(PlayerTurn);
                    break;
                case GameState.End:
                    GameEnd();
                    break;
                case GameState.Result:
                    GameResult();
                    break;
            }
        }
    }
}
