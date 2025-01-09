using Events;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody), typeof(EnemyHitIndicator))]
    public class EnemyShooter : MonoBehaviour, IDamageable
    {
        [Header("Shooting Settings")] [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField] private float shootInterval = 1f;
        [SerializeField] private int bulletCount = 10;
        [SerializeField] private float bulletSpeed = 10f;

        [Header("Movement Settings")]
        [Tooltip("Player Transform to seek and orbit. If left empty, will try to find an object tagged 'Player'.")]
        [SerializeField]
        private Transform player;

        [Tooltip("Distance at which the enemy stops approaching and begins orbiting.")] [SerializeField]
        private float orbitDistance = 5f;

        [Tooltip("Horizontal speed at which the enemy approaches the player.")] [SerializeField]
        private float approachSpeed = 3f;

        [Tooltip("Speed at which the enemy orbits the player once close.")] [SerializeField]
        private float orbitSpeed = 45f;

        [Header("Floating Settings")]
        [Tooltip("How high above the player's position the enemy should hover.")]
        [SerializeField]
        private float offsetAbovePlayer = 2f;

        [Tooltip("Amplitude of a bobbing motion above the offset (0 = no bob).")] [SerializeField]
        private float bobAmplitude = 0.5f;

        [Tooltip("Speed of the bobbing motion.")] [SerializeField]
        private float bobFrequency = 2f;

        [Header("Stats")] [SerializeField] private int maxHealth = 10;

        private int currentHealth;

        private Rigidbody rb;
        private float timer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            currentHealth = maxHealth;
        }

        private void Start()
        {
            if (!player)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj) player = playerObj.transform;
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                Fire();
                timer = 0f;
            }

            HandleMovement();
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= (int)damage;
            if (currentHealth <= 0)
            {
                EventManager.Publish(new EnemyKilledEvent(transform.position), this.Id());
                Destroy(gameObject);
            }

            Debug.Log($"Event {nameof(DamageTakenEvent)} published for {this.Id()}");
            EventManager.Publish(new DamageTakenEvent(damage), this.Id());
        }

        private void HandleMovement()
        {
            if (!player) return;

            var horizontalPos = new Vector3(transform.position.x, 0f, transform.position.z);
            var playerHorizPos = new Vector3(player.position.x, 0f, player.position.z);
            var distance = Vector3.Distance(horizontalPos, playerHorizPos);

            var desiredY = player.position.y + offsetAbovePlayer;
            if (bobAmplitude > 0f) desiredY += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

            if (distance > orbitDistance)
            {
                var direction = (playerHorizPos - horizontalPos).normalized;
                var velocity = direction * approachSpeed;

                rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);
            }

            var currentPos = transform.position;
            currentPos.y = desiredY;
            transform.position = currentPos;

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
        }
    }
}