using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BigTwo
{
    public class UICardCombinationIndicatorHand : MonoBehaviour
    {
        private Sequence m_sequenceShow;
        private Sequence m_sequenceHide;

        private bool m_isShow;

        [SerializeField]
        private TextMeshProUGUI m_textCardCombination;

        private void OnDestroy()
        {
            if (m_sequenceShow != null)
            {
                m_sequenceShow.Kill();
            }
            if (m_sequenceHide != null)
            {
                m_sequenceHide.Kill();
            }
        }

        public void UpdateCardCombinationName(CardCombination.Type cardCombinationType)
        {
            switch (cardCombinationType)
            {
                case CardCombination.Type.Single:
                case CardCombination.Type.None:
                    Hide();
                    break;
                default:
                    m_textCardCombination.text = cardCombinationType.ToString();
                    Show();
                    break;
            }
        }

        public void Show()
        {
            m_isShow = true;
            gameObject.SetActive(true);

            if (m_sequenceHide != null)
            {
                m_sequenceHide.Complete();
            }

            if (m_sequenceShow == null)
            {
                m_sequenceShow = DOTween.Sequence();
                m_sequenceShow.SetAutoKill(false);

                float totalAnimationDuration = 0f;
                m_sequenceShow.Insert(totalAnimationDuration, m_textCardCombination.rectTransform.DOScale(1f, Constant.ANIMATION_DURATION).SetEase(Ease.OutBack, 2f).From(0f));
                m_sequenceShow.Insert(totalAnimationDuration, m_textCardCombination.DOFade(1f, Constant.ANIMATION_DURATION).From(0.5f));
            }
            else
            {
                m_sequenceShow.Complete();
                m_sequenceShow.Restart();
            }
        }

        public void Hide()
        {
            if (!m_isShow)
            {
                return;
            }

            m_isShow = false;

            if (m_sequenceShow != null)
            {
                m_sequenceShow.Complete();
            }

            if (m_sequenceHide == null)
            {
                m_sequenceHide = DOTween.Sequence();
                m_sequenceHide.SetAutoKill(false);

                float totalAnimationDuration = 0f;
                m_sequenceHide.Insert(totalAnimationDuration, m_textCardCombination.rectTransform.DOScale(0f, Constant.ANIMATION_DURATION).SetEase(Ease.InBack, 2f).From(1f));
                m_sequenceHide.Insert(totalAnimationDuration, m_textCardCombination.DOFade(0.5f, Constant.ANIMATION_DURATION).From(1f));
            }
            else
            {
                m_sequenceHide.Complete();
                m_sequenceHide.Restart();
            }
        }
    }
}
