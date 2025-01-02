using System.Collections;
using UnityEngine;

namespace BulletLike
{
  public class WaveSpawner : MonoBehaviour
  {
      [System.Serializable]
      public class Wave
      {
          public GameObject enemyPrefab;
          public int count;
          public float spawnInterval;
          public float spawnRadius = 10f;
      }

      [SerializeField] private Wave[] waves;
      [SerializeField] private float timeBetweenWaves = 5f;

      private int _currentWaveIndex = 0;

      private void Start()
      {
          StartCoroutine(SpawnWaves());
      }

      private IEnumerator SpawnWaves()
      {
          while (_currentWaveIndex < waves.Length)
          {
              Wave wave = waves[_currentWaveIndex];
              yield return StartCoroutine(SpawnSingleWave(wave));

              _currentWaveIndex++;
              if (_currentWaveIndex < waves.Length)
              {
                  yield return new WaitForSeconds(timeBetweenWaves);
              }
          }
      }

      private IEnumerator SpawnSingleWave(Wave wave)
      {
          for (int i = 0; i < wave.count; i++)
          {
              // Pick a random point in a sphere around this spawner
              Vector3 randomOffset = Random.insideUnitSphere * wave.spawnRadius;
            
              // If you want them at the same Y as the spawner (i.e., on a flat plane), do:
              randomOffset.y = 0f;

              // The final spawn position
              Vector3 spawnPos = transform.position + randomOffset;

              // Spawn the enemy
              Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);

              yield return new WaitForSeconds(wave.spawnInterval);
          }
      }
  }
}
