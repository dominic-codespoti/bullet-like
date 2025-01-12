using System;
using UnityEngine;
using Events;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Player Settings")]
        public float Damage { get; private set; } = 10f;
        public float Speed { get; private set; } = 5f;

        private float MaxHealth { get; set; } = 100f;

        public float CurrentHealth { get; private set; } = 100f;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
        }

        private void Start()
        {
            EventManager.Subscribe<OnItemPickupEvent>(ModifyStats, this.Id());
        }

        private void ModifyStats(OnItemPickupEvent evt)
        {
            switch (evt.Item.passiveItem.statToUpgrade)
            {
                case "health":
                    MaxHealth += evt.Item.passiveItem.upgradeValue;
                    Heal(evt.Item.passiveItem.upgradeValue);
                    break;
                case "damage":
                    Damage += evt.Item.passiveItem.upgradeValue;
                    break;
                case "speed":
                    Speed += evt.Item.passiveItem.upgradeValue;
                    break;
                default:
                    return;
            }
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // Publish health change and damage taken using EventManager
            EventManager.Publish(new HealthChangedEvent(CurrentHealth), this.Id());
            EventManager.Publish(new DamageTakenEvent(damage), this.Id());

            // Check if the player is dead
            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            EventManager.Publish(new PlayerDeathEvent(), this.Id());
            Debug.Log("Player has died.");
        }

        private void Heal(float amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
            EventManager.Publish(new HealthChangedEvent(CurrentHealth), this.Id());
        }
    }

    public class HealthChangedEvent : BaseEvent
    {
        public float CurrentHealth { get; }

        public HealthChangedEvent(float currentHealth)
        {
            CurrentHealth = currentHealth;
        }
    }
}
