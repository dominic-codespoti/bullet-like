using Events;
using Player;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody), typeof(EnemyHitIndicator))]
    public class EnemyShooter : MonoBehaviour, IDamageable
    {
        [Header("Shooting Settings")]
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField] private float shootInterval = 1f;
        [SerializeField] private float bulletSpeed = 10f;
        
        [Header("Contact Damage Settings")]
        [SerializeField] private float contactDamage = 10f;

        [Header("Movement Settings")]
        [Tooltip("Player Transform to seek and orbit. If left empty, will try to find an object tagged 'Player'.")]
        [SerializeField]
        private Transform player;

        [Tooltip("Distance at which the enemy stops approaching and begins orbiting.")]
        [SerializeField]
        private float orbitDistance = 5f;

        [Tooltip("Horizontal speed at which the enemy approaches the player.")]
        [SerializeField]
        private float approachSpeed = 3f;

        [Tooltip("Speed at which the enemy orbits the player once close.")]
        [SerializeField]
        private float orbitSpeed = 45f;

        [Header("Floating Settings")]
        [Tooltip("How high above the enemy's initial position the enemy should hover.")]
        [SerializeField]
        private float offsetAboveInitial = 2f;

        [Tooltip("Amplitude of a bobbing motion above the offset (0 = no bob).")]
        [SerializeField]
        private float bobAmplitude = 0.5f;

        [Tooltip("Speed of the bobbing motion.")]
        [SerializeField]
        private float bobFrequency = 2f;

        [Header("Stats")]
        [SerializeField] private int maxHealth = 10;

        private int currentHealth;

        private Rigidbody rb;
        private float timer;
        private float initialY;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            currentHealth = maxHealth;
            initialY = transform.position.y;
            shootInterval += Random.Range(-0.5f, 0.5f);
        }

        private void Start()
        {
            if (!player)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj) player = playerObj.transform;
            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                Fire();
                timer = 0f;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= (int)damage;
            if (currentHealth <= 0)
            {
                EventManager.Publish(new EnemyKilledEvent(transform.position), this.Id());
                Destroy(gameObject);
            }

            EventManager.Publish(new DamageTakenEvent(damage), this.Id());
        }

        private void HandleMovement()
        {
            if (!player) return;

            var horizontalPos = new Vector3(rb.position.x, 0f, rb.position.z);
            var playerHorizPos = new Vector3(player.position.x, 0f, player.position.z);
            var distance = Vector3.Distance(horizontalPos, playerHorizPos);

            var desiredY = initialY + offsetAboveInitial;
            if (bobAmplitude > 0f) desiredY += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

            Vector3 targetPosition = rb.position;

            if (distance > orbitDistance)
            {
                // Approach the player
                var direction = (playerHorizPos - horizontalPos).normalized;
                var velocity = direction * approachSpeed * Time.fixedDeltaTime;
                targetPosition += new Vector3(velocity.x, 0f, velocity.z);
            }
            else
            {
                // Orbit the player
                transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.fixedDeltaTime);
            }

            // Apply floating height
            targetPosition.y = desiredY;

            // Move the Rigidbody
            rb.MovePosition(targetPosition);

            // Smoothly rotate to face the player
            var lookDir = player.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                var targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            }
        }

        private void Fire()
        {
            var directionToPlayer = (player.position - transform.position).normalized;
            var startingPosition = transform.position + directionToPlayer * 2f;
            var bullet = Instantiate(bulletPrefab, startingPosition, Quaternion.identity);
            bullet.transform.forward = directionToPlayer;

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = directionToPlayer * bulletSpeed;
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            var playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(contactDamage);
            }
        }
    }
}
