using DG.Tweening;
using UnityEngine;

namespace BigTwo
{
    public class Card : MonoBehaviour
    {
        [SerializeField]
        protected CardData m_cardData;
        [SerializeField]
        protected SpriteRenderer m_spriteRenderer;

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

        protected void OnValidate()
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

        public void Initialize(CardData cardData)
        {
            m_cardData = cardData;
            Initialize();
        }

        public bool IsHigherThan(Card otherCard)
        {
            CardData otherCardData = otherCard.CardData;
            return m_cardData.Rank > otherCardData.Rank || (m_cardData.Rank == otherCardData.Rank && m_cardData.Suit > otherCardData.Suit);
        }

        public void ToggleVisibility(bool isVisible, bool isAnimating = false)
        {
            Vector3 targetEulerAngle = transform.localEulerAngles;
            targetEulerAngle.y = (isVisible ? 0f : 180f);
            if (isAnimating)
            {
                transform.DOLocalRotate(targetEulerAngle, Constant.ANIMATION_DURATION);
            }
            else
            {
                transform.localEulerAngles = targetEulerAngle;
            }
        }
    }
}
