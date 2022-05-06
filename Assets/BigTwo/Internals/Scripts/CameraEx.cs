using UnityEngine;

namespace BigTwo
{
    [ExecuteInEditMode]
    public class CameraEx : MonoBehaviour
    {
        [SerializeField]
        private Camera m_targetCamera;
        [SerializeField]
        protected Vector2 m_viewSize = new Vector2(10, 10);

        public Camera Camera
        {
            get
            {
                return m_targetCamera;
            }
        }

        private void OnEnable()
        {
            Apply();
        }

        private void OnValidate()
        {
            Apply();
        }

        private void OnDrawGizmos()
        {
            Apply();

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(m_viewSize.x, m_viewSize.y, 0.1f));
        }

        public void Apply()
        {
            if (gameObject.scene.rootCount == 0)
                return;

            if (m_targetCamera == null)
                m_targetCamera = GetComponent<Camera>();

            if (m_targetCamera == null)
                return;

            var cameraAspect = m_targetCamera.aspect;
            var viewAspect = m_viewSize.x / m_viewSize.y;

            if (cameraAspect < viewAspect) //portrait
            {
                m_targetCamera.orthographicSize = m_viewSize.y * viewAspect * (1f / m_targetCamera.aspect) * 0.5f;
            }
            else
            {
                m_targetCamera.orthographicSize = m_viewSize.y * cameraAspect * (1f / m_targetCamera.aspect) * 0.5f;
            }
        }
    }
}