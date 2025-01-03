using BulletLike.Events;
using BulletLike.GameObjectExtensions;
using UnityEngine;

namespace BulletLike.Enemy
{
  [RequireComponent(typeof(Rigidbody), typeof(EnemyHitIndicator))]
  public class EnemyShooter : MonoBehaviour, IDamageable
  {
      [Header("Shooting Settings")]
      [SerializeField] private GameObject bulletPrefab;
      [SerializeField] private float shootInterval = 1f;
      [SerializeField] private int bulletCount = 10;
      [SerializeField] private float bulletSpeed = 10f;

      [Header("Movement Settings")]
      [Tooltip("Player Transform to seek and orbit. If left empty, will try to find an object tagged 'Player'.")]
      [SerializeField] private Transform player;

      [Tooltip("Distance at which the enemy stops approaching and begins orbiting.")]
      [SerializeField] private float orbitDistance = 5f;

      [Tooltip("Horizontal speed at which the enemy approaches the player.")]
      [SerializeField] private float approachSpeed = 3f;

      [Tooltip("Speed at which the enemy orbits the player once close.")]
      [SerializeField] private float orbitSpeed = 45f;

      [Header("Floating Settings")]
      [Tooltip("How high above the player's position the enemy should hover.")]
      [SerializeField] private float offsetAbovePlayer = 2f;

      [Tooltip("Amplitude of a bobbing motion above the offset (0 = no bob).")]
      [SerializeField] private float bobAmplitude = 0.5f;

      [Tooltip("Speed of the bobbing motion.")]
      [SerializeField] private float bobFrequency = 2f;

      [Header("Stats")]
      [SerializeField] private int maxHealth = 10;

      private Rigidbody _rb;
      private float _timer;
      private int _currentHealth;

      public void TakeDamage(float damage)
      {
          _currentHealth -= (int)damage;
          if (_currentHealth <= 0)
          {
              Destroy(gameObject);
          }

          Debug.Log($"Event {nameof(DamageTakenEvent)} published for {this.Id()}");
          EventManager.Publish(new DamageTakenEvent(damage), this.Id());
      }

      private void Awake()
      {
          _rb = GetComponent<Rigidbody>();
          _rb.useGravity = false;
          _currentHealth = maxHealth;
      }

      private void Start()
      {
          if (!player)
          {
              GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
              if (playerObj)
              {
                  player = playerObj.transform;
              }
          }
      }

      private void Update()
      {
          _timer += Time.deltaTime;
          if (_timer >= shootInterval)
          {
              Fire();
              _timer = 0f;
          }

          HandleMovement();
      }

      private void HandleMovement()
      {
          if (!player) return;

          Vector3 horizontalPos = new Vector3(transform.position.x, 0f, transform.position.z);
          Vector3 playerHorizPos = new Vector3(player.position.x, 0f, player.position.z);
          float distance = Vector3.Distance(horizontalPos, playerHorizPos);

          float desiredY = player.position.y + offsetAbovePlayer;
          if (bobAmplitude > 0f)
          {
              desiredY += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
          }

          if (distance > orbitDistance)
          {
              Vector3 direction = (playerHorizPos - horizontalPos).normalized;
              Vector3 velocity = direction * approachSpeed;
            
              _rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
          }
          else
          {
              _rb.linearVelocity = Vector3.zero;
              transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);
          }

          Vector3 currentPos = transform.position;
          currentPos.y = desiredY;
          transform.position = currentPos;

          Vector3 lookDir = (player.position - transform.position);
          lookDir.y = 0f;
          if (lookDir.sqrMagnitude > 0.001f)
          {
              Quaternion targetRot = Quaternion.LookRotation(lookDir);
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
