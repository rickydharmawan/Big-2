using System;
using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
            Initialize();
        }

        private void OnValidate()
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

        public Sprite GetCardSprite(int rank, int suite)
        {
            rank = Mathf.Clamp(rank, Constant.CARD_RANK_MIN, Constant.CARD_RANK_MAX);
            suite = Mathf.Clamp(suite, Constant.CARD_SUITE_MIN, Constant.CARD_SUITE_MAX);

            return m_cardRankSuites[suite - 1].CardRankSprites[rank - 1].Sprite;
        }
    }
}
