using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigTwo
{
    public class UIPassIndicator : MonoBehaviour
    {
        private Sequence m_sequenceShow;

        [SerializeField]
        private RectTransform m_rectSelf;

        public void Show(Action onComplete = null)
        {
            gameObject.SetActive(true);
            if (m_sequenceShow == null)
            {
                m_sequenceShow = DOTween.Sequence();
                m_sequenceShow.SetAutoKill(false);

                float totalAnimationDuration = 0f;

                m_sequenceShow.Insert(totalAnimationDuration, m_rectSelf.DOScale(1f, Constant.ANIMATION_DURATION).From(0f).SetEase(Ease.OutBack, 2f));
                totalAnimationDuration += Constant.ANIMATION_DURATION;

                totalAnimationDuration += 0.5f;

                m_sequenceShow.InsertCallback(totalAnimationDuration, () =>
                {
                    onComplete?.Invoke();
                });

                m_sequenceShow.Insert(totalAnimationDuration, m_rectSelf.DOScale(0f, Constant.ANIMATION_DURATION).From(1f).SetEase(Ease.InBack, 2f));
                totalAnimationDuration += Constant.ANIMATION_DURATION;
            }
            else
            {
                m_sequenceShow.Complete();
                m_sequenceShow.Restart();
            }
        }
    }
}
