using UnityEngine;
using System.Collections.Generic;

namespace FPSGame.Systems
{
    /// <summary>
    /// Object pooler for efficient object reuse (bullets, effects, etc.).
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [Header("Pools")]
        [SerializeField] private List<Pool> pools;
        
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        /// <summary>
        /// Spawns an object from the pool.
        /// </summary>
        /// <param name="tag">Pool tag</param>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <returns>Spawned GameObject</returns>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist!");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="obj">Object to return</param>
        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }

        /// <summary>
        /// Creates a new pool at runtime.
        /// </summary>
        public void CreatePool(string tag, GameObject prefab, int size)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} already exists!");
                return;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(tag, objectPool);
        }
    }
}
