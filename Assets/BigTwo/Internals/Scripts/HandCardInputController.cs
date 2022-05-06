using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BigTwo
{
    public class HandCardInputController : MonoBehaviour
    {
        public class SelectCardEvent : UnityEvent<Card[]> { }

        private SelectCardEvent m_onSelectCard;

        private RaycastHit[] m_raycastHitHandCardInputs;
        private List<Card> m_listOfAscendedCard;

        public List<Card> ListOfAscendedCard
        {
            get
            {
                return m_listOfAscendedCard;
            }
        }

        public SelectCardEvent OnSelectCard
        {
            get
            {
                if (m_onSelectCard == null)
                {
                    m_onSelectCard = new SelectCardEvent();
                }

                return m_onSelectCard;
            }
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray screenRay = CameraSingleton.Instance.Camera.ScreenPointToRay(Input.mousePosition);
                int hitCount = Physics.RaycastNonAlloc(screenRay, m_raycastHitHandCardInputs, 100f);

                float closestDistance = float.MaxValue;
                HandCardInput closestHandCardInput = null;

                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit raycastHit = m_raycastHitHandCardInputs[i];
                    HandCardInput handCardInput = raycastHit.collider.GetComponentInParent<HandCardInput>();
                    if (handCardInput)
                    {
                        float distance = (raycastHit.point - CameraSingleton.Instance.transform.position).sqrMagnitude;

                        if (closestDistance > distance)
                        {
                            closestDistance = distance;
                            closestHandCardInput = handCardInput;
                        }
                    }
                }

                if (closestHandCardInput)
                {
                    bool isAscend = closestHandCardInput.IsAscend;
                    if (!isAscend && m_listOfAscendedCard.Count < Constant.CARD_MAXIMUM_COMBINATION) //Only able to toggle it on 
                    {
                        closestHandCardInput.ToggleAscend();
                        m_listOfAscendedCard.Add(closestHandCardInput.Card);

                        OnSelectCard.Invoke(m_listOfAscendedCard.ToArray());
                    }
                    else if (isAscend)
                    {
                        closestHandCardInput.ToggleAscend();
                        m_listOfAscendedCard.Remove(closestHandCardInput.Card);

                        OnSelectCard.Invoke(m_listOfAscendedCard.ToArray());
                    }
                }
            }
        }

        private void Initialize()
        {
            m_raycastHitHandCardInputs = new RaycastHit[Constant.CARD_MAXIMUM_COUNT / Constant.PLAYER_MAXIMUM_COUNT];
            m_listOfAscendedCard = new List<Card>(Constant.CARD_MAXIMUM_COMBINATION);
        }

        public void ClearAscendedCards()
        {
            ListOfAscendedCard.Clear();
        }
    }
}
