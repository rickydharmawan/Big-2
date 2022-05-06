using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public class UIResultController : MonoBehaviour
    {
        protected UIResultLeaderboard[] m_uiLeaderboards;

        [SerializeField]
        protected Canvas m_canvas;

        [SerializeField]
        protected Image m_imageBackground;
        [SerializeField]
        protected RectTransform m_rectLeaderboardsContainer;
        [SerializeField]
        protected RectTransform m_rectButtonRetry;

        public void Initialize(Player[] players)
        {
            Dictionary<int, List<Player>> dictionaryOfPlayerRank = new Dictionary<int, List<Player>>(players.Length);
            List<int> listOfCardCount = new List<int>(players.Length);

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                int cardCount = player.Hand.ListOfCard.Count;
                if (dictionaryOfPlayerRank.TryGetValue(cardCount, out List<Player> listOfPlayer))
                {
                    listOfPlayer.Add(player);
                    dictionaryOfPlayerRank[cardCount] = listOfPlayer;
                }
                else
                {
                    listOfCardCount.Add(cardCount);
                    dictionaryOfPlayerRank.Add(cardCount, new List<Player>() { player });
                }
            }

            listOfCardCount.Sort();

            int leaderboardIteration = 0;
            m_uiLeaderboards = new UIResultLeaderboard[players.Length];
            for (int i = 0; i < listOfCardCount.Count; i++)
            {
                int cardCount = listOfCardCount[i];
                if (dictionaryOfPlayerRank.TryGetValue(cardCount, out List<Player> listOfPlayer))
                {
                    for (int j = 0; j < listOfPlayer.Count; j++)
                    {
                        UIResultLeaderboard uiLeaderboard = GameManager.Instance.ObjectPool.Get("Result Leaderboard").GetComponent<UIResultLeaderboard>();
                        uiLeaderboard.transform.SetParent(m_rectLeaderboardsContainer);
                        uiLeaderboard.transform.localScale = Vector3.one;
                        uiLeaderboard.Initialize(listOfPlayer[j], i + 1);

                        m_uiLeaderboards[leaderboardIteration] = uiLeaderboard;

                        leaderboardIteration++;
                    }
                }
            }
        }

        public void Show()
        {
            m_canvas.enabled = true;

            for (int i = 0; i < m_uiLeaderboards.Length; i++)
            {
                UIResultLeaderboard uiLeaderboard = m_uiLeaderboards[i];
                uiLeaderboard.Show();
            }

            m_rectButtonRetry.DOScale(1f, Constant.ANIMATION_DURATION).From(0f);
        }
    }
}
