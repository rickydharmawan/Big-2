using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public class PlayerMain : Player
    {
        private Card[] m_ascendedCards;

        [SerializeField]
        private HandCardInputController m_handCardInputController;

        [SerializeField]
        private UICardCombinationIndicatorHand m_uiCardCardCombinationIndicatorHand;
        [SerializeField]
        private Button m_buttonSubmit;

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            m_handCardInputController.OnSelectCard.AddListener(HandCardInputController_OnSelectCard);
        }

        public override void InitializeHand(Card[] cards)
        {
            base.InitializeHand(cards);

            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];
                card.ToggleVisibility(true);
                HandCardInput handCardInput = card.gameObject.AddComponent<HandCardInput>();
                handCardInput.Initialize(card);
            }
        }

        public override void AddHandCard(params Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];
                HandCardInput handCardInput = card.gameObject.AddComponent<HandCardInput>();
                handCardInput.Initialize(card);
            }
            base.AddHandCard(cards);
        }

        private void HandCardInputController_OnSelectCard(Card[] cards)
        {
            if (GameManager.Instance.GameState >= GameState.Start)
            {
                m_ascendedCards = cards;

                CardCombination.Type cardCombinationType = CardCombination.GetCardCombinationType(cards);
                m_uiCardCardCombinationIndicatorHand.UpdateCardCombinationName(cardCombinationType);

                m_buttonSubmit.interactable = cardCombinationType != CardCombination.Type.None;
            }
        }

        public void SetAvatar(UIAvatar otherAvatar)
        {
            m_uiAvatar.SetAvatar(otherAvatar);
        }

        public void SubmitHandCard()
        {
            if (GameManager.Instance.PlayerTurn == this)
            {
                CardCombination cardCombination = new CardCombination();
                CardCombination.Type cardCombinationType = CardCombination.GetCardCombinationType(m_ascendedCards);

                cardCombination.Submit(cardCombinationType, m_ascendedCards);

                Field.Instance.SubmitCardCombination(this, cardCombination, (status) =>
                {
                    if (status)
                    {
                        for (int i = 0; i < cardCombination.Cards.Length; i++)
                        {
                            Card card = cardCombination.Cards[i];
                            HandCardInput handCardInput = card.gameObject.GetComponent<HandCardInput>();
                            Destroy(handCardInput);
                        }
                        m_handCardInputController.ClearAscendedCards();
                        m_uiCardCardCombinationIndicatorHand.Hide();
                        Hand.Arrange(true);

                        m_uiAvatar.ChangeFace(AvatarState.Happy, 2f);
                    }
                });
            }
        }

        public void PassTurn()
        {
            if (GameManager.Instance.PlayerTurn == this)
            {
                m_uiAvatar.ChangeFace(AvatarState.Sad, 2f);
                GameManager.Instance.PassTurn();
            }
        }
    }
}
