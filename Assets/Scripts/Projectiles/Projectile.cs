using UnityEngine;

namespace Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float lifeTime = 5f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, lifeTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            IDamageable damageable = collision.collider.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}