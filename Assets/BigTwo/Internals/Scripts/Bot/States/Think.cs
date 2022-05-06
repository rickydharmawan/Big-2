using UnityEngine;

namespace BigTwo.Bot.States
{
    public struct Think : IState<PlayerBot>
    {
        private bool m_hasSubmitCards;

        public void OnEnter(Brain<PlayerBot> brain, PlayerBot playerBot)
        {
            m_hasSubmitCards = false;
        }

        public void OnUpdate(Brain<PlayerBot> brain, PlayerBot playerBot)
        {
            if (!m_hasSubmitCards && GameManager.Instance.GameState == GameState.TurnBegin)
            {
                m_hasSubmitCards = true;
                CardCombination cardCombination = new CardCombination();
                if (Field.Instance.IsEmpty)
                {
                    cardCombination = playerBot.GetCardCombination(GameManager.Instance.IsFirstTurn);
                }
                else
                {
                    cardCombination = playerBot.GetHigherCardCombination();
                }

                if (cardCombination.CombinationType == CardCombination.Type.None)
                {
                    playerBot.UIAvatar.ChangeFace(AvatarState.Sad, 2f);
                    GameManager.Instance.PassTurn();
                    brain.ChangeState(new Idle()
                    {

                    });
                }
                else
                {
                    Field.Instance.SubmitCardCombination(playerBot, cardCombination, (status) =>
                    {
                        if (status)
                        {
                            playerBot.UIAvatar.ChangeFace(AvatarState.Happy, 2f);
                            playerBot.ArrangeHandCard(true);
                            brain.ChangeState(new Idle()
                            {

                            });
                        }
                    });
                }
            }
        }

        public void OnExit(Brain<PlayerBot> brain, PlayerBot playerBot)
        {
            m_hasSubmitCards = false;
        }
    }
}
