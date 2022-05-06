using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BigTwo
{
    public class Hand : MonoBehaviour
    {
        [SerializeField]
        private List<Card> m_listOfCard;

        [SerializeField]
        private Vector3 m_spacing;
        [SerializeField]
        private GameObject m_gameObjectCardAnchor;

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

        public bool IsEmpty
        {
            get
            {
                return m_listOfCard == null || m_listOfCard.Count == 0;
            }
        }

        public void Initialize(Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];
                card.transform.parent = m_gameObjectCardAnchor.transform;
            }
            m_listOfCard = new List<Card>(cards);
        }

        public void Add(params Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];
                card.transform.parent = m_gameObjectCardAnchor.transform;
                card.transform.localRotation = Quaternion.identity;
            }
            ListOfCard.AddRange(cards);
            Arrange();
        }

        public void Remove(params Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                ListOfCard.Remove(cards[i]);
            }
        }

        public Vector3[] GetCardPositions()
        {
            int cardCount = m_listOfCard.Count;
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
                cardPositions[i] = new Vector3(horizontalSpacing, 0f, depthSpacing);
            }

            return cardPositions;
        }

        public void Arrange(bool isAnimating = false)
        {
            Vector3[] cardPositions = GetCardPositions();
            for (int i = 0; i < cardPositions.Length; i++)
            {
                Vector3 position = cardPositions[i];

                if (isAnimating)
                {
                    m_listOfCard[i].transform.DOLocalMove(position, Constant.ANIMATION_DURATION);
                }
                else
                {
                    m_listOfCard[i].transform.localPosition = position;
                }
            }
        }
    }
}
