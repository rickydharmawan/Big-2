using UnityEngine;

namespace BigTwo.Bot.States
{
    public struct Idle : IState<PlayerBot>
    {
        public void OnEnter(Brain<PlayerBot> brain, PlayerBot playerBot)
        {

        }

        public void OnUpdate(Brain<PlayerBot> brain, PlayerBot playerBot)
        {
            if (GameManager.Instance.GameState == GameState.TurnBegin && GameManager.Instance.PlayerTurn == playerBot)
            {
                Debug.Log($"Change state to think", playerBot);
                brain.ChangeState(new Think()
                {

                });
            }
        }

        public void OnExit(Brain<PlayerBot> brain, PlayerBot playerBot)
        {

        }
    }
}