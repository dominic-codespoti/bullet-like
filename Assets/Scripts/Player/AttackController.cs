using Projectiles;
using UnityEngine;

namespace Player
{
    public class AttackController : MonoBehaviour
    {
        [Header("Prefabs")] [Tooltip("Main projectile to use. If null, we use the fallback.")] [SerializeField]
        private Projectile projectilePrefab;

        [Header("Settings")] [Tooltip("Time between shots.")] [SerializeField]
        private float fireRate = 0.2f;

        [Tooltip("Distance in front of the camera to spawn the projectile.")] [SerializeField]
        private float spawnDistance = 2f;

        private float fireTimer;

        private void Update()
        {
            fireTimer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0) && fireTimer >= fireRate)
            {
                FireProjectile();
                fireTimer = 0f;
            }
        }

        private void FireProjectile()
        {
            var prefabToUse = projectilePrefab;
            if (!prefabToUse)
            {
                Debug.LogWarning("No primary or fallback projectile assigned. Unable to fire.");
                return;
            }

            var cam = Camera.main;
            if (!cam)
            {
                Debug.LogWarning("No MainCamera found. Unable to fire.");
                return;
            }

            var spawnPosition = cam.transform.position + cam.transform.forward * spawnDistance;
            var spawnRotation = cam.transform.rotation;

            var obj = Instantiate(prefabToUse, spawnPosition, spawnRotation);
            obj.SetDamage(10f);
        }
    }
}