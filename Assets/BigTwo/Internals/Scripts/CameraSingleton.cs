using UnityEngine;

namespace BigTwo
{
    public class CameraSingleton : MonoBehaviour
    {
        private static CameraSingleton m_instance;

        public static CameraSingleton Instance
        {
            get
            {
                return m_instance;
            }
        }

        [SerializeField]
        private Camera m_camera;

        public Camera Camera
        {
            get
            {
                return m_camera;
            }
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else if (m_instance != this)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
