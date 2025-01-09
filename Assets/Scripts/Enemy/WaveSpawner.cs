using System.Collections;
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

        [Header("Waves Configuration")]
        [SerializeField] private Wave[] waves;
        [SerializeField] private float timeBetweenWaves = 5f;

        private BoxCollider spawnAreaCollider;
        private GameObject _player;
        private int currentWaveIndex = 0;
        private bool isPlayerInside = false;
        private bool isSpawning = false;

        private Coroutine spawningCoroutine;

        private void Awake()
        {
            // Ensure the BoxCollider is present and configured as a trigger
            spawnAreaCollider = GetComponent<BoxCollider>();
            spawnAreaCollider.isTrigger = true;

            // Initialize BoxCollider to default size
            if (waves.Length > 0)
            {
                SetSpawnArea(waves[0].spawnAreaSize);
            }
            else
            {
                Debug.LogWarning("No waves configured for WaveSpawner.");
            }
        }

        private void Start()
        {
            // Find the player by tag
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogError("Player with tag 'Player' not found in the scene.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _player && !isPlayerInside)
            {
                isPlayerInside = true;
                if (!isSpawning && currentWaveIndex < waves.Length)
                {
                    spawningCoroutine = StartCoroutine(SpawnWaves());
                }
            }
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
                currentWaveIndex = 0; // Optionally reset wave index or handle as needed
            }
        }

        /// <summary>
        /// Coroutine to handle spawning multiple waves with delays.
        /// </summary>
        private IEnumerator SpawnWaves()
        {
            isSpawning = true;

            while (currentWaveIndex < waves.Length && isPlayerInside)
            {
                Wave currentWave = waves[currentWaveIndex];

                // Adjust the spawn area to match the current wave's requirements
                SetSpawnArea(currentWave.spawnAreaSize);

                yield return StartCoroutine(SpawnSingleWave(currentWave));

                currentWaveIndex++;

                if (currentWaveIndex < waves.Length && isPlayerInside)
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
            for (int i = 0; i < wave.enemiesToSpawn && isPlayerInside; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition(wave.spawnAreaSize);
                Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);
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

            // Get the bounds of the spawn area
            Bounds bounds = spawnAreaCollider.bounds;

            // Generate random positions within the bounds
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(randomX, randomY, randomZ);
        }
    }
}
