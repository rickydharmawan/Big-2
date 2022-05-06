using UnityEngine;

namespace BigTwo
{
    public class Card : MonoBehaviour
    {
        [SerializeField]
        private CardData m_cardData;
        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        public CardData CardData
        {
            get
            {
                return m_cardData;
            }
        }

        public Card(CardData cardData)
        {
            m_cardData = cardData;
            Initialize();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Initialize();
            }
#endif
        }

        public void Initialize()
        {
            m_spriteRenderer.sprite = CardManager.Instance.GetCardSprite(m_cardData.Rank, m_cardData.Suit);
        }

        public bool IsHigherThan(Card otherCard)
        {
            CardData otherCardData = otherCard.CardData;
            return m_cardData.Rank > otherCardData.Rank || (m_cardData.Rank == otherCardData.Rank && m_cardData.Suit > otherCardData.Suit);
        }
    }
}
