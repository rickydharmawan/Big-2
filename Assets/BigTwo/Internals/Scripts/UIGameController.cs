using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public class UIGameController : MonoBehaviour
    {
        private Sequence m_sequenceTurnIndicator;

        private bool m_isUserTurn;

        [SerializeField]
        private RectTransform m_rectTurnIndicatorPanel;
        [SerializeField]
        private Image m_imageTurnIndicatorBackground;
        [SerializeField]
        private RectTransform m_rectTurnIndicatorForeground;
        [SerializeField]
        private CanvasGroup m_canvasGroupTurnIndicatorForeground;
        [SerializeField]
        private TextMeshProUGUI m_textTurnIndicator;
        [SerializeField]
        private TextMeshProUGUI m_textCardCombinationName;

        [SerializeField]
        private UIPassIndicator[] uiPassIndicators;

        private Button m_buttonTurnPass;
        private Button m_buttonCardSubmit;
        private Button m_buttonCardShuffle;

        private Sequence SequenceTurnIndicator
        {
            get
            {
                return m_sequenceTurnIndicator;
            }
            set
            {
                m_sequenceTurnIndicator = value;
            }
        }

        private void OnDestroy()
        {
            if (m_sequenceTurnIndicator != null)
            {
                m_sequenceTurnIndicator.Kill();
            }
        }

        private void UpdateTurnIndicator()
        {
            if (m_isUserTurn) //Users turn
            {
                m_textTurnIndicator.text = $"Your turn";
            }
            else //Opponents turn
            {
                int playerTurnIndex = GameManager.Instance.PlayerTurnIndex;
                m_textTurnIndicator.text = $"Player {playerTurnIndex + 1}'s turn";
            }
        }

        private void UpdatePlayersInput()
        {
            m_buttonTurnPass.enabled = m_isUserTurn;
            m_buttonCardSubmit.enabled = m_isUserTurn;
        }

        private void UpdateUserTurnStatus()
        {
            m_isUserTurn = !(GameManager.Instance.PlayerTurn is PlayerBot);
        }

        public void ShowTurnIndicator()
        {
            UpdateUserTurnStatus();
            UpdateTurnIndicator();

            m_rectTurnIndicatorPanel.gameObject.SetActive(true);

#if UNITY_EDITOR
            Debug.Log($"Start turn for player {GameManager.Instance.PlayerTurnIndex + 1}");
#endif

            if (m_sequenceTurnIndicator == null)
            {
                m_sequenceTurnIndicator = DOTween.Sequence();
                m_sequenceTurnIndicator.SetAutoKill(false);

                float totalAnimationDuration = 0f;

                m_sequenceTurnIndicator.Insert(totalAnimationDuration, m_imageTurnIndicatorBackground.DOFade(0.5f, Constant.ANIMATION_DURATION).From(0f));
                m_sequenceTurnIndicator.Insert(totalAnimationDuration, m_canvasGroupTurnIndicatorForeground.DOFade(1f, Constant.ANIMATION_DURATION).From(0.5f));
                m_sequenceTurnIndicator.Insert(totalAnimationDuration, m_rectTurnIndicatorForeground.DOJumpAnchorPos(m_rectTurnIndicatorForeground.anchoredPosition, 100f, 1, Constant.ANIMATION_DURATION));
                m_sequenceTurnIndicator.Insert(totalAnimationDuration, m_rectTurnIndicatorForeground.DOScale(1f, Constant.ANIMATION_DURATION).From(0f).SetEase(Ease.OutBack, 2f));
                totalAnimationDuration += Constant.ANIMATION_DURATION * 2f;

                totalAnimationDuration += 1f;

                m_sequenceTurnIndicator.Insert(totalAnimationDuration, m_imageTurnIndicatorBackground.DOFade(0f, Constant.ANIMATION_DURATION).From(0.5f));
                m_sequenceTurnIndicator.Insert(totalAnimationDuration, m_rectTurnIndicatorForeground.DOAnchorPosY(-200f, Constant.ANIMATION_DURATION));
                totalAnimationDuration += Constant.ANIMATION_DURATION;

                m_sequenceTurnIndicator.InsertCallback(totalAnimationDuration, () =>
                {
                    GameManager.Instance.ChangeGameState(GameState.TurnBegin);
                });
            }
            else
            {
                m_sequenceTurnIndicator.Complete();
                m_sequenceTurnIndicator.Restart();
            }
        }

        public void ShowPassIndicator(Action onComplete = null)
        {
            uiPassIndicators[GameManager.Instance.PlayerTurnIndex].Show(onComplete);
        }
    }
}
