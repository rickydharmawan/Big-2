using DG.Tweening;
using System;
using UnityEngine;

namespace BigTwo
{
    public class Field : MonoBehaviour
    {
        private static Field m_instance;

        private Sequence m_sequenceSubmit;

        private Player m_player;
        [SerializeField]
        private CardCombination m_cardCombination;

        [SerializeField]
        private Transform m_transformCardsContainer;

        [SerializeField]
        private DiscardPile m_discardPile;

        [SerializeField]
        private Vector3 m_spacing;

        public static Field Instance
        {
            get
            {
                return m_instance;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return m_cardCombination == null || m_cardCombination.CombinationType == CardCombination.Type.None;
            }
        }

        public CardCombination CardCombination
        {
            get
            {
                return m_cardCombination;
            }
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void Awake()
        {
            Initialize();
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

        public Vector3[] GetCardPositions(int cardCount)
        {
            Vector3[] cardPositions = new Vector3[cardCount];
            int halfCardCount = cardCount / 2;
            bool isCardCountEven = cardCount % 2 == 0;
            for (int i = 0; i < cardCount; i++)
            {
                float horizontalSpacing;
                float depthSpacing;
                if (isCardCountEven)
                {
                    horizontalSpacing = (0.5f - halfCardCount + i) * m_spacing.x;
                    depthSpacing = (0.5f - halfCardCount + i) * m_spacing.z;
                }
                else
                {
                    horizontalSpacing = (i - halfCardCount) * m_spacing.x;
                    depthSpacing = (i - halfCardCount) * m_spacing.z;
                }
                cardPositions[i] = m_transformCardsContainer.position + new Vector3(horizontalSpacing, 0f, depthSpacing);
            }

            return cardPositions;
        }

        public bool SubmitCardCombination(Player player, CardCombination otherCardCombination, Action<bool> onComplete = null)
        {
            if (m_cardCombination == null || otherCardCombination.IsHigherThan(m_cardCombination))
            {
                m_player = player;
                if (m_cardCombination != null)
                {
                    m_discardPile.AddCards(m_cardCombination.Cards);
                }
                CardCombination previousCardCombination = m_cardCombination;
                m_cardCombination = otherCardCombination;
                player.RemoveHandCard(otherCardCombination.Cards);

                if (m_sequenceSubmit != null)
                {
                    m_sequenceSubmit.Complete();
                }

                m_sequenceSubmit = DOTween.Sequence();
                float totalAnimationDuration = 0f;

                Vector3[] cardPositions = GetCardPositions(otherCardCombination.Cards.Length);
                for (int i = 0; i < otherCardCombination.Cards.Length; i++)
                {
                    Card card = otherCardCombination.Cards[i];
                    card.transform.SetParent(m_transformCardsContainer);
                    m_sequenceSubmit.Insert(totalAnimationDuration, card.transform.DOMove(cardPositions[i], Constant.ANIMATION_DURATION));
                    m_sequenceSubmit.Insert(totalAnimationDuration, card.transform.DORotateQuaternion(Quaternion.identity, Constant.ANIMATION_DURATION));
                    totalAnimationDuration += Constant.ANIMATION_DURATION / 4f;
                }

                totalAnimationDuration += Constant.ANIMATION_DURATION * 3f / 4f;

                m_sequenceSubmit.InsertCallback(totalAnimationDuration, () =>
                {
                    onComplete?.Invoke(true);
                    if (player.IsHandEmpty)
                    {
                        GameManager.Instance.ChangeGameState(GameState.End);
                    }
                    else
                    {
                        GameManager.Instance.NextTurn();
                    }
                });

                return true;
            }

            onComplete?.Invoke(false);
            return false;
        }

        public void Discard()
        {
            if (m_cardCombination != null && m_cardCombination.Cards != null)
            {
                m_discardPile.AddCards(m_cardCombination.Cards);
            }
            m_cardCombination = null;
        }
    }
}
