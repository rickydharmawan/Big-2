using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BigTwo
{
    public class HandCardInput : MonoBehaviour
    {
        private bool m_isAscend;
        private Card m_card;

        public bool IsAscend
        {
            get
            {
                return m_isAscend;
            }
        }

        public Card Card
        {
            get
            {
                return m_card;
            }
        }

        #region TEMP 
        private void Start()
        {
            Initialize(GetComponent<Card>());
        }
        #endregion TEMP 

        public void Initialize(Card card)
        {
            m_card = card;
        }

        public bool ToggleAscend()
        {
            return ToggleAscend(!m_isAscend);
        }

        public bool ToggleAscend(bool isAscend)
        {
            if (isAscend)
            {
                Ascend();
            }
            else
            {
                Descend();
            }
            return isAscend;
        }

        public void Ascend()
        {
            m_isAscend = true;

            transform.DOKill();
            transform.DOLocalMoveY(1f, Constant.ANIMATION_DURATION);
        }

        public void Descend()
        {
            m_isAscend = false;

            transform.DOKill();
            transform.DOLocalMoveY(0f, Constant.ANIMATION_DURATION);
        }
    }
}
