using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public class UIResultLeaderboardRank : MonoBehaviour
    {
        [SerializeField]
        protected RectTransform m_rectSelf;
        [SerializeField]
        protected CanvasGroup m_canvasGroup;
        [SerializeField]
        protected Image m_imageBackground;
        [SerializeField]
        protected TextMeshProUGUI m_textRank;

        [SerializeField]
        protected Color[] m_backgroundColors;

        public void Initialize(int rank)
        {
            int backgroundColorIndex = Mathf.Clamp(rank - 1, 0, m_backgroundColors.Length - 1);
            m_imageBackground.color = m_backgroundColors[backgroundColorIndex];

            m_textRank.text = (rank).ToString();
        }

        public float Show(ref Sequence sequence, float totalAnimationDuration)
        {
            sequence.Insert(totalAnimationDuration, m_rectSelf.DOScale(1f, Constant.ANIMATION_DURATION * 2f).SetEase(Ease.InQuad).From(1.5f));
            sequence.Insert(totalAnimationDuration, m_canvasGroup.DOFade(1f, Constant.ANIMATION_DURATION * 2f).SetEase(Ease.InCubic).From(0f));
            totalAnimationDuration += Constant.ANIMATION_DURATION * 2f;

            return totalAnimationDuration;
        }
    }
}
