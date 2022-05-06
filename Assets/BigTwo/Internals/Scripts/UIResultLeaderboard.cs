using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public class UIResultLeaderboard : MonoBehaviour
    {
        protected Sequence m_sequenceShow;
        protected Image[] m_imageCards;

        [SerializeField]
        protected UIAvatar m_uiAvatar;
        [SerializeField]
        protected RectTransform m_rectCardsContainer;
        [SerializeField]
        protected UIResultLeaderboardRank m_uiLeaderboardRank;

        public void Initialize(Player player, int rank)
        {
            m_uiAvatar.SetAvatar(player.UIAvatar);
            AvatarState avatarState = AvatarState.Normal;
            if (rank <= 1)
            {
                avatarState = AvatarState.Happy;
            }
            else if(rank >= 3)
            {
                avatarState = AvatarState.Sad;
            }
            m_uiAvatar.ChangeFace(avatarState);
            m_imageCards = new Image[player.Hand.ListOfCard.Count];
            for (int i = 0; i < player.Hand.ListOfCard.Count; i++)
            {
                Card card = player.Hand.ListOfCard[i];
                Image imageCard = GameManager.Instance.ObjectPool.Get("Result Card").GetComponent<Image>();
                imageCard.transform.SetParent(m_rectCardsContainer);
                imageCard.transform.localScale = Vector3.one;
                imageCard.sprite = CardManager.Instance.GetCardSprite(card.CardData.Rank, card.CardData.Suit);
                m_imageCards[i] = imageCard;
            }
            m_uiLeaderboardRank.Initialize(rank);
        }

        public void Show(Action onComplete = null)
        {
            if (m_sequenceShow != null)
            {
                m_sequenceShow.Complete();
            }

            m_sequenceShow = DOTween.Sequence();

            float totalAnimationDuration = 0f;

            for (int i = 0; i < m_imageCards.Length; i++)
            {
                int index = i;
                Image imageCard = m_imageCards[index];
                m_sequenceShow.Insert(totalAnimationDuration, imageCard.rectTransform.DOScale(1f, Constant.ANIMATION_DURATION * 0.5f).SetEase(Ease.OutBack).From(0f));
                totalAnimationDuration += Constant.ANIMATION_DURATION * 0.25f;
            }
            totalAnimationDuration += Constant.ANIMATION_DURATION * 0.25f;

            totalAnimationDuration = m_uiLeaderboardRank.Show(ref m_sequenceShow, totalAnimationDuration);
        }
    }
}
