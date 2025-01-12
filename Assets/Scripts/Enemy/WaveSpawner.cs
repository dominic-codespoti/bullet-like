using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletLike.Enemy
{
    [RequireComponent(typeof(BoxCollider))]
    public class WaveSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class Wave
        {
            [Tooltip("Prefab of the enemy to spawn.")]
            public GameObject enemyPrefab;

            [Tooltip("Number of enemies to spawn in this wave.")]
            public int enemiesToSpawn;

            [Tooltip("Seconds to wait between spawning each enemy.")]
            public float secondsBetweenSpawns;

            [Tooltip("Size of the spawn area for this wave (width, height, depth).")]
            public Vector3 spawnAreaSize;
        }

        private enum WaveState
        {
            NotStarted,
            InProgress,
            Completed
        }

        [Header("Waves Configuration")]
        [SerializeField] private Wave[] waves;
        [SerializeField] private float timeBetweenWaves = 5f;

        private BoxCollider spawnAreaCollider;
        private GameObject _player;
        private bool isPlayerInside = false;
        private bool isSpawning = false;

        private Dictionary<int, WaveState> waveStates;
        private Coroutine spawningCoroutine;

        private int totalEnemiesSpawned = 0; // Tracks total enemies spawned

        private void Awake()
        {
            spawnAreaCollider = GetComponent<BoxCollider>();
            spawnAreaCollider.isTrigger = true;

            if (waves.Length > 0)
            {
                SetSpawnArea(waves[0].spawnAreaSize);
            }

            // Initialize wave states
            waveStates = new Dictionary<int, WaveState>();
            for (int i = 0; i < waves.Length; i++)
            {
                waveStates[i] = WaveState.NotStarted;
            }
        }

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == _player && isPlayerInside)
            {
                isPlayerInside = false;
                if (spawningCoroutine != null)
                {
                    StopCoroutine(spawningCoroutine);
                    spawningCoroutine = null;
                }
                isSpawning = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _player && !isPlayerInside)
            {
                isPlayerInside = true;

                if (!isSpawning && waveStates.ContainsValue(WaveState.NotStarted))
                {
                    spawningCoroutine = StartCoroutine(SpawnWaves());
                }
            }
        }

        /// <summary>
        /// Coroutine to handle spawning multiple waves with delays.
        /// </summary>
        private IEnumerator SpawnWaves()
        {
            isSpawning = true;

            for (int i = 0; i < waves.Length; i++)
            {
                if (!isPlayerInside || waveStates[i] != WaveState.NotStarted)
                {
                    continue; // Skip waves that are already in progress or completed
                }

                Wave currentWave = waves[i];
                waveStates[i] = WaveState.InProgress;

                SetSpawnArea(currentWave.spawnAreaSize);

                yield return StartCoroutine(SpawnSingleWave(currentWave));

                waveStates[i] = WaveState.Completed;
                Debug.Log($"Finished wave {i + 1}/{waves.Length}.");

                if (i < waves.Length - 1)
                {
                    yield return new WaitForSeconds(timeBetweenWaves);
                }
            }

            isSpawning = false;
        }

        /// <summary>
        /// Coroutine to spawn a single wave of enemies.
        /// </summary>
        /// <param name="wave">The wave configuration.</param>
        private IEnumerator SpawnSingleWave(Wave wave)
        {
            for (int i = 0; i < wave.enemiesToSpawn; i++)
            {
                if (!isPlayerInside)
                {
                    yield break;
                }

                Vector3 spawnPos = GetRandomSpawnPosition(wave.spawnAreaSize);
                Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);

                totalEnemiesSpawned++;

                yield return new WaitForSeconds(wave.secondsBetweenSpawns);
            }
        }

        /// <summary>
        /// Sets the BoxCollider's size to define the spawn area.
        /// </summary>
        /// <param name="spawnAreaSize">The desired size of the spawn area (width, height, depth).</param>
        private void SetSpawnArea(Vector3 spawnAreaSize)
        {
            if (spawnAreaCollider == null)
            {
                Debug.LogError("Spawn Area Collider is missing.");
                return;
            }

            spawnAreaCollider.size = spawnAreaSize;
            spawnAreaCollider.center = Vector3.zero; // Ensure the collider is centered on the spawner
        }

        /// <summary>
        /// Calculates a random spawn position within the spawn area collider.
        /// </summary>
        /// <param name="spawnAreaSize">The size of the spawn area (width, height, depth).</param>
        /// <returns>A random position within the spawn area.</returns>
        private Vector3 GetRandomSpawnPosition(Vector3 spawnAreaSize)
        {
            if (spawnAreaCollider == null)
            {
                Debug.LogError("Spawn Area Collider is missing.");
                return transform.position;
            }

            Bounds bounds = spawnAreaCollider.bounds;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(randomX, randomY, randomZ);
        }
    }
}
