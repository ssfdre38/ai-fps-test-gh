using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FPSGame.Utilities;

namespace FPSGame.Systems
{
    /// <summary>
    /// Manages enemy wave spawning with increasing difficulty.
    /// </summary>
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private int startingWave = 1;
        [SerializeField] private float timeBetweenWaves = 10f;
        [SerializeField] private int baseEnemiesPerWave = 5;
        [SerializeField] private float difficultyScaling = 1.2f;
        
        [Header("Enemy Spawning")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnDelay = 0.5f;
        
        [Header("Advanced Settings")]
        [SerializeField] private int maxEnemiesAlive = 20;
        [SerializeField] private bool autoStartWaves = true;

        private int currentWave;
        private int enemiesAlive;
        private int enemiesToSpawn;
        private bool isSpawning;
        private List<GameObject> activeEnemies = new List<GameObject>();

        // Events
        public delegate void WaveStartedHandler(int waveNumber);
        public event WaveStartedHandler OnWaveStarted;
        
        public delegate void WaveCompletedHandler(int waveNumber);
        public event WaveCompletedHandler OnWaveCompleted;
        
        public delegate void EnemySpawnedHandler(GameObject enemy);
        public event EnemySpawnedHandler OnEnemySpawned;

        /// <summary>
        /// Gets the current wave number.
        /// </summary>
        public int CurrentWave => currentWave;

        /// <summary>
        /// Gets the number of enemies currently alive.
        /// </summary>
        public int EnemiesAlive => enemiesAlive;

        private void Start()
        {
            currentWave = startingWave - 1;
            
            if (autoStartWaves)
            {
                StartCoroutine(WaveController());
            }
        }

        private void Update()
        {
            // Clean up null references
            activeEnemies.RemoveAll(e => e == null);
            enemiesAlive = activeEnemies.Count;
        }

        /// <summary>
        /// Main wave controller coroutine.
        /// </summary>
        private IEnumerator WaveController()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                
                if (!isSpawning)
                {
                    StartNextWave();
                    yield return StartCoroutine(SpawnWave());
                }
            }
        }

        /// <summary>
        /// Starts the next wave.
        /// </summary>
        public void StartNextWave()
        {
            currentWave++;
            enemiesToSpawn = CalculateEnemiesForWave(currentWave);
            
            OnWaveStarted?.Invoke(currentWave);
            
            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncrementWave();
            }
        }

        /// <summary>
        /// Spawns enemies for the current wave.
        /// </summary>
        private IEnumerator SpawnWave()
        {
            isSpawning = true;
            
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Don't spawn if max enemies alive is reached
                while (enemiesAlive >= maxEnemiesAlive)
                {
                    yield return new WaitForSeconds(1f);
                }
                
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }
            
            isSpawning = false;
            
            // Wait for all enemies to be defeated
            while (enemiesAlive > 0)
            {
                yield return new WaitForSeconds(1f);
            }
            
            OnWaveCompleted?.Invoke(currentWave);
        }

        /// <summary>
        /// Spawns a single enemy.
        /// </summary>
        private void SpawnEnemy()
        {
            if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
            {
                Debug.LogWarning("No enemy prefabs or spawn points assigned!");
                return;
            }

            // Select random enemy prefab
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            
            // Select random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Spawn enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            activeEnemies.Add(enemy);
            enemiesAlive++;
            
            // Subscribe to enemy death
            var enemyHealth = enemy.GetComponent<AI.EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath += () => OnEnemyDeath(enemy);
            }
            
            OnEnemySpawned?.Invoke(enemy);
        }

        /// <summary>
        /// Calculates the number of enemies for a given wave.
        /// </summary>
        private int CalculateEnemiesForWave(int waveNumber)
        {
            return Mathf.RoundToInt(baseEnemiesPerWave * Mathf.Pow(difficultyScaling, waveNumber - 1));
        }

        /// <summary>
        /// Called when an enemy dies.
        /// </summary>
        private void OnEnemyDeath(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
            enemiesAlive--;
            
            // Award score
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(100);
            }
        }

        /// <summary>
        /// Manually triggers the next wave.
        /// </summary>
        public void TriggerNextWave()
        {
            if (!isSpawning)
            {
                StopAllCoroutines();
                StartCoroutine(ManualWaveStart());
            }
        }

        private IEnumerator ManualWaveStart()
        {
            StartNextWave();
            yield return StartCoroutine(SpawnWave());
        }

        /// <summary>
        /// Stops spawning and clears all enemies.
        /// </summary>
        public void StopSpawning()
        {
            StopAllCoroutines();
            isSpawning = false;
            
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            
            activeEnemies.Clear();
            enemiesAlive = 0;
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnPoints == null)
                return;

            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 1f);
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.forward * 2f);
                }
            }
        }
    }
}
