using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BigTwo
{
    public enum AvatarState
    {
        Normal,
        Happy,
        Sad,
    }
    public class UIAvatar : MonoBehaviour
    {
        protected AvatarState m_avatarState;
        protected Coroutine m_coroutineChangeFace;

        protected int m_body;
        protected int m_hair;
        protected int m_suit;

        [SerializeField]
        protected Image m_imageBody;
        [SerializeField]
        protected Image m_imageHair;
        [SerializeField]
        protected Image m_imageFace;
        [SerializeField]
        protected Image m_imageSuit;

        [SerializeField]
        protected Sprite[] m_spriteBodies;
        [SerializeField]
        protected Sprite[] m_spriteHairs;
        [SerializeField]
        protected Sprite[] m_spriteFaces;
        [SerializeField]
        protected Sprite[] m_spriteSuits;

        public int Body
        {
            get
            {
                return m_body;
            }
        }

        public int Hair
        {
            get
            {
                return m_hair;
            }
        }

        public int Suit
        {
            get
            {
                return m_suit;
            }
        }

        public void ChangeFace(AvatarState avatarState)
        {
            m_avatarState = (AvatarState)Mathf.Clamp((int)avatarState, 0, m_spriteFaces.Length - 1);
            m_imageFace.sprite = m_spriteFaces[(int)m_avatarState];
        }

        public void ChangeFace(AvatarState avatarState, float duration)
        {
            if (m_coroutineChangeFace != null)
            {
                StopCoroutine(m_coroutineChangeFace);
                m_coroutineChangeFace = null;
            }

            m_coroutineChangeFace = StartCoroutine(ChangeFaceCoroutine(avatarState, duration));
        }

        private IEnumerator ChangeFaceCoroutine(AvatarState avatarState, float duration)
        {
            ChangeFace(avatarState);
            yield return new WaitForSeconds(duration);
            ChangeFace(AvatarState.Normal);
        }

        public void ChangeBody(int body)
        {
            m_body = Mathf.Clamp(body, 0, m_spriteBodies.Length - 1);
            m_imageBody.sprite = m_spriteBodies[m_body];
        }

        public void ModifyBody(int modifier)
        {
            m_body = (m_body + modifier) % m_spriteBodies.Length;
            if (m_body < 0)
            {
                m_body += m_spriteBodies.Length;
            }
            ChangeBody(m_body);
        }

        public void NextBody()
        {
            ModifyBody(1);
        }

        public void PreviousBody()
        {
            ModifyBody(-1);
        }

        public void ChangeHair(int hair)
        {
            m_hair = Mathf.Clamp(hair, 0, m_spriteHairs.Length - 1);
            m_imageHair.sprite = m_spriteHairs[m_hair];
        }

        public void ModifyHair(int modifier)
        {
            m_hair = (m_hair + modifier) % m_spriteHairs.Length;
            if (m_hair < 0)
            {
                m_hair += m_spriteHairs.Length;
            }
            ChangeHair(m_hair);
        }

        public void NextHair()
        {
            ModifyHair(1);
        }

        public void PreviousHair()
        {
            ModifyHair(-1);
        }

        public void ChangeSuit(int suit)
        {
            m_suit = Mathf.Clamp(suit, 0, m_spriteSuits.Length - 1);
            m_imageSuit.sprite = m_spriteSuits[m_suit];
        }

        public void ModifySuit(int modifier)
        {
            m_suit = (m_suit + modifier) % m_spriteSuits.Length;
            if (m_suit < 0)
            {
                m_suit += m_spriteSuits.Length;
            }
            ChangeSuit(m_suit);
        }

        public void NextSuit()
        {
            ModifySuit(1);
        }

        public void PreviousSuit()
        {
            ModifySuit(-1);
        }

        public void RandomizeAppearance()
        {
            ChangeBody(Random.Range(0, m_spriteBodies.Length));
            ChangeHair(Random.Range(0, m_spriteHairs.Length));
            ChangeSuit(Random.Range(0, m_spriteSuits.Length));
        }

        public void SetAvatar(UIAvatar otherAvatar)
        {
            ChangeBody(otherAvatar.Body);
            ChangeHair(otherAvatar.Hair);
            ChangeSuit(otherAvatar.Suit);
        }
    }
}
