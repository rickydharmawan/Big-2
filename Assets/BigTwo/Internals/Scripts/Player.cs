using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigTwo
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        protected UIAvatar m_uiAvatar;

        [SerializeField]
        protected Hand m_hand;

        public UIAvatar UIAvatar
        {
            get
            {
                return m_uiAvatar;
            }
        }

        public Hand Hand
        {
            get
            {
                return m_hand;
            }
        }

        public bool IsHandEmpty
        {
            get
            {
                return m_hand.IsEmpty;
            }
        }

        public virtual void Initialize()
        { 

        }

        public virtual void InitializeHand(Card[] cards)
        {
            m_hand.Initialize(cards);
        }

        public virtual void AddHandCard(params Card[] cards)
        {
            m_hand.Add(cards);
        }

        public virtual void RemoveHandCard(params Card[] cards)
        {
            m_hand.Remove(cards);
        }

        public virtual void ArrangeHandCard(bool isAnimating)
        {
            Hand.Arrange(isAnimating);
        }
    }
}
