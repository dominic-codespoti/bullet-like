using System.Collections;
using UnityEngine;

namespace BulletLike.Enemy
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

      private GameObject _player;
      private int currentWaveIndex = 0;

      private void Start()
      {
          StartCoroutine(SpawnWaves());
          _player = GameObject.FindGameObjectWithTag("Player");
      }

      private IEnumerator SpawnWaves()
      {
          while (currentWaveIndex < waves.Length)
          {
              Wave wave = waves[currentWaveIndex];
              yield return StartCoroutine(SpawnSingleWave(wave));

              currentWaveIndex++;
              if (currentWaveIndex < waves.Length)
              {
                  yield return new WaitForSeconds(timeBetweenWaves);
              }
          }
      }

      private IEnumerator SpawnSingleWave(Wave wave)
      {
          if (!_player || _player.transform.position.sqrMagnitude > wave.spawnRadius * wave.spawnRadius)
          {
              yield break;
          }

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
