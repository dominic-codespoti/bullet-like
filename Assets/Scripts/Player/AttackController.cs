using BulletLike.Projectiles;
using UnityEngine;

namespace BulletLike.Player
{
  public class AttackController : MonoBehaviour
  {
      [Header("Prefabs")]
      [Tooltip("Main projectile to use. If null, we use the fallback.")]
      [SerializeField] private Projectile projectilePrefab;
    
      [Header("Settings")]
      [Tooltip("Time between shots.")]
      [SerializeField] private float fireRate = 0.2f;
    
      [Tooltip("Distance in front of the camera to spawn the projectile.")]
      [SerializeField] private float spawnDistance = 2f;

      private float _fireTimer;

      private void Update()
      {
          _fireTimer += Time.deltaTime;

          if (Input.GetMouseButtonDown(0) && _fireTimer >= fireRate)
          {
              FireProjectile();
              _fireTimer = 0f;
          }
      }

      private void FireProjectile()
      {
          Projectile prefabToUse = projectilePrefab;
          if (!prefabToUse)
          {
              Debug.LogWarning("No primary or fallback projectile assigned. Unable to fire.");
              return;
          }

          Camera cam = Camera.main;
          if (!cam)
          {
              Debug.LogWarning("No MainCamera found. Unable to fire.");
              return;
          }

          Vector3 spawnPosition = cam.transform.position + cam.transform.forward * spawnDistance;
          Quaternion spawnRotation = cam.transform.rotation;

          Instantiate(prefabToUse, spawnPosition, spawnRotation);
      }
  }
}
