using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BigTwo
{
    public class MainManager : MonoBehaviour
    {
        protected static MainManager m_instance;

        public static MainManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        protected void Awake()
        {
            Initialize();
        }

        private void Start()
        {
#if UNITY_EDITOR
            bool isGameSceneLoaded = SceneManager.GetSceneByName("Game").isLoaded || SceneManager.sceneCount > 1;
            bool shouldLoadGameScene = true;

            if (isGameSceneLoaded)
            {
                shouldLoadGameScene = false;
            }
#endif

            if (shouldLoadGameScene)
            {
                LoadScene("Game");
            }
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

        public void LoadScene(string sceneToLoad)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }

        private IEnumerator LoadSceneCoroutine(string sceneToLoad, string sceneToUnload, Action onComplete = null)
        {
            var async = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (!async.isDone)
            {
                yield return null;
            }

            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            onComplete?.Invoke();
        }

        public void ReloadScene(string sceneName, Action onComplete = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, sceneName, onComplete));
        }

        public void ReloadGameScene(Action onComplete = null)
        {
            ReloadScene("Game", onComplete);
        }
    }
}
