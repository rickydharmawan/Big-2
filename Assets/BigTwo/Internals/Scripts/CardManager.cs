using System;
using UnityEngine;

namespace BigTwo
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager m_instance;

        public static CardManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        [Serializable]
        public struct CardRankSprite
        {
            [SerializeField]
            private Sprite m_sprite;

            public Sprite Sprite
            {
                get
                {
                    return m_sprite;
                }
            }
        }

        [Serializable]
        public struct CardSuiteSprite
        {
            [SerializeField]
            private string name;
            [SerializeField]
            private CardRankSprite[] m_cardRankSprites;

            public CardRankSprite[] CardRankSprites
            {
                get
                {
                    return m_cardRankSprites;
                }
            }
        }

        [SerializeField]
        private CardSuiteSprite[] m_cardRankSuites;

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

        public Sprite GetCardSprite(int rank, int suit)
        {
            rank = Mathf.Clamp(rank, Constant.CARD_RANK_MIN, Constant.CARD_RANK_MAX);
            suit = Mathf.Clamp(suit, Constant.CARD_SUIT_MIN, Constant.CARD_SUIT_MAX);

            return m_cardRankSuites[suit - 1].CardRankSprites[rank - 1].Sprite;
        }
    }
}
