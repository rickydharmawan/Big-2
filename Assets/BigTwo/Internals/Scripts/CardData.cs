using System;
using UnityEngine;

namespace BigTwo
{
    [Serializable]
    public class CardData
    {
        [SerializeField]
        private int m_suit;
        [SerializeField]
        private int m_rank;

        public int Suit
        {
            get
            {
                return m_suit;
            }
        }

        public int Rank
        {
            get
            {
                return m_rank;
            }
        }

        public CardData(int suit, int rank)
        {
            m_suit = suit;
            m_rank = rank;
        }
    }
}
