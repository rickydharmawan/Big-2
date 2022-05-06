using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonderednow
{
    /// <summary>
    /// Simple and quick pooling system.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// var pool = new Pool<GameObject>(
    ///     () => {
    ///         return new GameObject();
    ///     },
    ///     (GameObject go) => {
    ///         go.transform.localScale = Vector3.one;
    ///     },
    ///     (GameObject go) => {
    ///         go.transform.localScale = Vector3.zero;
    ///     },
    ///     (GameObject go) => {
    ///         Destroy(go);
    ///     });
    /// ]]>
    /// </code>
    /// </example>
    /// <typeparam name="T"></typeparam>
    public sealed class Pool<T>
    {
        public delegate T ConstructorDelegate();
        public delegate void OnGetDelegate(T item);
        public delegate void OnRecycleDelegate(T item);
        public delegate void OnDisposeDelegate(T item);

        //delegates for manipulating reusable object.
        private ConstructorDelegate m_constructor;
        private OnGetDelegate m_onGet;
        private OnRecycleDelegate m_onRecycle;
        private OnDisposeDelegate m_onDispose;
        
        /// <summary>
        /// The recycle bin
        /// </summary>
        private Stack<T> m_bin;

        /// <summary>
        /// The recycle bin property.
        /// </summary>
        public Stack<T> Bin
        {
            get
            {
                return m_bin;
            }
        }

        /// <summary>
        /// See example for delegates info.
        /// </summary>
        /// <param name="constructor">Delegate for object creation.</param>
        /// <param name="onGet">Delegate for reusable object initialization.</param>
        /// <param name="onRecycle">Delegate that called before the object recycled.</param>
        /// <param name="onDispose">Delegate to dispose an object.</param>
        public Pool(ConstructorDelegate constructor, OnGetDelegate onGet, OnRecycleDelegate onRecycle, OnDisposeDelegate onDispose)
        {
            m_bin = new Stack<T>();
            m_constructor = constructor;
            m_onGet = onGet;
            m_onRecycle = onRecycle;
            m_onDispose = onDispose;
        }

        /// <summary>
        /// Instantiates a bulk of objects to fill the recycle bin.
        /// </summary>
        /// <param name="count">Number of objects to be created.</param>
        public void Fill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var item = m_constructor();
                m_onRecycle(item);
                m_bin.Push(item);
            }
        }
        
        /// <summary>
        /// Retrieves a reuseable object or create a new object if the recycle bin is empty.
        /// </summary>
        /// <remarks>
        /// If an object is created, OnRecycle will be called and followed by OnGet.
        /// If reusable object exists, only OnGet will be called.
        /// </remarks>
        /// <returns>Reusable object.</returns>
        public T Get()
        {
            T item = default(T);
            if (m_bin.Count == 0)
            {
                item = m_constructor();
                m_onRecycle(item);
            }
            else
            {
                item = m_bin.Pop();
            }
            m_onGet(item);
            return item;
        }
        
        /// <summary>
        /// Puts an object to recycle bin
        /// </summary>
        /// <param name="item">Object to be recycled.</param>
        public void Recycle(T item)
        {
            m_onRecycle(item);
            m_bin.Push(item);
        }
        
        /// <summary>
        /// Removes all objects from recycle bin.
        /// </summary>
        public void Dispose()
        {
            while (m_bin.Count > 0)
            {
                var item = m_bin.Pop();
                m_onDispose(item);
            }
        }
    }
}