using System;
using Events;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public float Health { get; private set; } = 100f;
        public float Damage { get; private set; } = 10f;
        public float Speed { get; private set; } = 5f;

        public void Start()
        {
            EventManager.Subscribe<OnItemPickupEvent>(ModifyStats, this.Id());
        }

        private void ModifyStats(OnItemPickupEvent evt)
        {
            switch (evt.Item.passiveItem.statToUpgrade)
            {
                case "health":
                    Health += evt.Item.passiveItem.upgradeValue;
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
    }
}