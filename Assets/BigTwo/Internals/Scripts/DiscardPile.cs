using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace BigTwo
{
    public class DiscardPile : MonoBehaviour
    {
        protected Sequence m_sequenceDiscard;

        protected List<Card> m_listOfCard;

        [SerializeField]
        protected Transform m_transformCardsContainer;

        [SerializeField]
        protected Vector3 m_spacing;

        public List<Card> ListOfCard
        {
            get
            {
                if (m_listOfCard == null)
                {
                    m_listOfCard = new List<Card>();
                }

                return m_listOfCard;
            }
        }

        public void AddCards(Card[] cards)
        {
            int previousCardCount = ListOfCard.Count;
            ListOfCard.AddRange(cards);

            m_sequenceDiscard = DOTween.Sequence();
            float totalAnimationDuration = 0f;

            for (int i = 0; i < cards.Length; i++)
            {
                Vector3 cardPosition = m_transformCardsContainer.position + m_spacing * (previousCardCount + i);

                Card card = cards[i];
                card.transform.SetParent(m_transformCardsContainer);
                m_sequenceDiscard.Insert(totalAnimationDuration, card.transform.DOMove(cardPosition, Constant.ANIMATION_DURATION));
                m_sequenceDiscard.Insert(totalAnimationDuration, card.transform.DORotateQuaternion(Quaternion.identity, Constant.ANIMATION_DURATION));
                totalAnimationDuration += Constant.ANIMATION_DURATION / 4f;
            }

            totalAnimationDuration += Constant.ANIMATION_DURATION * 3f / 4f;
        }
    }
}
