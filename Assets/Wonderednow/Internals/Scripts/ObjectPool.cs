using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wonderednow
{
    /// <summary>
    /// Object Pool utility class. Uses Wonderednow.Pool class under the hood.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        public struct Datum
        {
            [SerializeField] private string m_id;
            [SerializeField] private GameObject m_prefab;
            [SerializeField] private int m_capacity;

            public string ID
            {
                get
                {
                    return m_id;
                }
            }

            public GameObject Prefab
            {
                get
                {
                    return m_prefab;
                }
            }

            public int Capacity
            {
                get
                {
                    return m_capacity;
                }
            }
        }

        private Dictionary<string, Pool<GameObject>> m_poolDictionary;
        private List<GameObject> m_activeObjects;
        private Dictionary<GameObject, string> m_activeObjectDictionary;

        [SerializeField] private Transform m_inactiveObjectsContainer;

        [SerializeField] private Datum[] m_data;

        private void Awake()
        {
            var totalCapacity = 0;
            m_poolDictionary = new Dictionary<string, Pool<GameObject>>();
            m_poolDictionary = new Dictionary<string, Pool<GameObject>>();
            for (int i = 0; i < m_data.Length; i++)
            {
                var id = m_data[i].ID;
                var prefab = m_data[i].Prefab;
                var capacity = m_data[i].Capacity;

                totalCapacity += capacity;

                var pool = AddPool(id, prefab);

                var temp = new GameObject[capacity];
                for (int j = 0; j < capacity; j++)
                {
                    var gameObject = pool.Get();
                    temp[j] = gameObject;
                }
                for (int j = 0; j < capacity; j++)
                {
                    pool.Recycle(temp[j]);
                }
            }

            m_activeObjects = new List<GameObject>(totalCapacity);
            m_activeObjectDictionary = new Dictionary<GameObject, string>(totalCapacity);
        }

        private void OnDestroy()
        {
            foreach (var kvp in m_poolDictionary)
            {
                kvp.Value.Dispose();
            }
            m_poolDictionary.Clear();

            m_activeObjectDictionary.Clear();
            while (m_activeObjects.Count > 0)
            {
                var o = m_activeObjects[0];
                m_activeObjects.RemoveAt(0);
                Destroy(o);
            }
        }

        /// <summary>
        /// Adds new gameObject pool.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public Pool<GameObject> AddPool(string id, GameObject prefab)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("ID is required for Spawn Info.");
            }

            var pool = new Pool<GameObject>(() =>
            {
                GameObject poolObject = Instantiate(prefab);
                gameObject.transform.SetParent(m_inactiveObjectsContainer ? m_inactiveObjectsContainer : null);
                return poolObject;
            }, (GameObject gameObject) =>
            {
                gameObject.gameObject.SetActive(true);
            }, (GameObject gameObject) =>
            {
                gameObject.gameObject.SetActive(false);
                gameObject.transform.SetParent(m_inactiveObjectsContainer ? m_inactiveObjectsContainer : null);
            }, (GameObject gameObject) =>
            {

            });

            m_poolDictionary.Add(id, pool);
            return pool;
        }

        /// <summary>
        /// Spawns a gameObject based on registered Spawn Info.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameObject Get(string id)
        {
            Pool<GameObject> pool;
            if (m_poolDictionary.TryGetValue(id, out pool))
            {
                var gameObject = pool.Get();
                m_activeObjects.Add(gameObject);
                m_activeObjectDictionary.Add(gameObject, id);

                return gameObject;
            }
            return null;
        }

        /// <summary>
        /// Disable a gameObject and put it inside a pool.
        /// </summary>
        /// <param name="gameObject"></param>
        public void Release(GameObject gameObject)
        {
            string id;
            if (m_activeObjectDictionary.TryGetValue(gameObject, out id))
            {
                gameObject.transform.SetParent(m_inactiveObjectsContainer ? null : m_inactiveObjectsContainer);
                m_activeObjects.Remove(gameObject);
                m_activeObjectDictionary.Remove(gameObject);

                m_poolDictionary[id].Recycle(gameObject);
            }
        }
    }
}