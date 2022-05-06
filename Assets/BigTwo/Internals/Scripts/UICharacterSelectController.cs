using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public class UICharacterSelectController : MonoBehaviour
    {
        protected Sequence m_sequenceHide;

        [SerializeField]
        protected PlayerMain m_playerMain;

        [SerializeField]
        protected UIAvatar m_uiAvatar;

        [SerializeField]
        protected RectTransform m_rectCanvas;
        [SerializeField]
        protected Canvas m_canvas;
        [SerializeField]
        protected GraphicRaycaster m_graphicRaycaster;
        [SerializeField]
        protected Image m_imageBackground;
        [SerializeField]
        protected TextMeshProUGUI m_textTitle;
        [SerializeField]
        protected RectTransform m_rectPanel;

        public void ConfirmSelection()
        {
            m_playerMain.SetAvatar(m_uiAvatar);
            GameManager.Instance.ChangeGameState(GameState.Setup);

            Hide(() =>
            {
                GameManager.Instance.ChangeGameState(GameState.Preparation);
            });
        }

        public void Hide(Action onComplete = null)
        {
            if (m_sequenceHide == null)
            {
                m_sequenceHide = DOTween.Sequence();
                m_sequenceHide.SetAutoKill(false);

                float totalAnimationDuration = 0f;

                m_sequenceHide.InsertCallback(totalAnimationDuration, () =>
                {
                    m_graphicRaycaster.enabled = false;
                });

                m_sequenceHide.Insert(totalAnimationDuration, m_textTitle.rectTransform.DOAnchorPosY(m_textTitle.rectTransform.anchoredPosition.y + m_rectCanvas.rect.size.y, Constant.ANIMATION_DURATION * 3f));
                m_sequenceHide.Insert(totalAnimationDuration, m_rectPanel.DOAnchorPosY(m_rectPanel.anchoredPosition.y - m_rectCanvas.rect.size.y, Constant.ANIMATION_DURATION * 3f));
                totalAnimationDuration += Constant.ANIMATION_DURATION;
                m_sequenceHide.Insert(totalAnimationDuration, m_imageBackground.DOFade(0f, Constant.ANIMATION_DURATION));
                totalAnimationDuration += Constant.ANIMATION_DURATION * 2f;

                m_sequenceHide.InsertCallback(totalAnimationDuration, () =>
                {
                    onComplete?.Invoke();
                });
            }
            else
            {
                m_sequenceHide.Complete();
                m_sequenceHide.Restart();
            }
        }
    }
}
