using System.Collections;
using Events;
using Player;
using UnityEngine;

namespace Loot
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickupItem : MonoBehaviour
    {
        public PassiveItem passiveItem;

        [Header("Bobbing Settings")]
        [Tooltip("Height offset for bobbing.")]
        public float bobHeight = 0.125f;

        [Tooltip("Speed of the bobbing motion.")]
        public float bobSpeed = 1f;

        [Tooltip("Rotation speed of the pickup item.")]
        public float rotationSpeed = 45f;

        [Tooltip("Duration of the movement to initial position.")]
        public float moveDuration = 0.5f;

        private Vector3 initialPosition;
        private Rigidbody rb;
        private Collider collider;
        private bool isFloating = false;
        private float randomOffset;
        private float randomSpeed;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            collider = GetComponent<Collider>();

            randomOffset = Random.Range(0f, 2f * Mathf.PI);
            randomSpeed = bobSpeed * Random.Range(0.8f, 1.2f);
        }

        private void Update()
        {
            if (isFloating)
            {
                HandleFloating();
            }

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        private void HandleFloating()
        {
            float time = Time.time * randomSpeed + randomOffset;
            float newY = initialPosition.y + Mathf.Sin(time) * bobHeight;
            transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name.ToLower().Contains("floor"))
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                collider.isTrigger = true;

                initialPosition = collision.contacts[0].point + Vector3.up * bobHeight + (Vector3.up * bobHeight);
                StartCoroutine(MoveToInitialPosition());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var firstPersonController = other.gameObject.GetComponent<FirstPersonController>();
            if (firstPersonController)
            {
                EventManager.Publish(new OnItemPickupEvent(this), firstPersonController.Id());
                Destroy(gameObject);
            }
        }

        private IEnumerator MoveToInitialPosition()
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                rb.MovePosition(Vector3.Lerp(startPosition, initialPosition, elapsedTime / moveDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rb.MovePosition(initialPosition);
            isFloating = true;
        }
    }
}