using UnityEngine;

namespace FPSGame.Utilities
{
    /// <summary>
    /// Generic singleton base class for MonoBehaviour objects.
    /// Ensures only one instance exists and persists across scenes if needed.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static readonly object lockObject = new object();
        private static bool applicationIsQuitting = false;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed. Returning null.");
                    return null;
                }

                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            instance = singletonObject.AddComponent<T>();
                            Debug.Log($"[Singleton] Created new instance of {typeof(T)}");
                        }
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Override this to prevent destruction on scene load.
        /// </summary>
        protected virtual bool DontDestroyOnLoad => true;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                
                if (DontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} found. Destroying.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                applicationIsQuitting = true;
            }
        }
    }
}
