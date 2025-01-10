using Events;
using UnityEngine;

namespace Loot
{
    public class LootDropper : MonoBehaviour
    {
        [System.Serializable]
        public class Loot
        {
            public PickupItem DroppedItem;
            public float DropChance;
        }

        [SerializeField] private Loot[] lootTable;

        private void Start()
        {
            EventManager.Subscribe<EnemyKilledEvent>(DropLoot, this.Id());
        }

        private void DropLoot(EnemyKilledEvent evt)
        {
            var position = evt.Position;

            foreach (var loot in lootTable)
            {
                if (!(Random.value * 100f <= loot.DropChance))
                {
                    continue;
                }

                var pickupPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var pickup = pickupPrefab.AddComponent<PickupItem>();
                pickupPrefab.AddComponent<SphereCollider>().isTrigger = true;
                pickupPrefab.AddComponent<Rigidbody>().useGravity = true;
                pickupPrefab.transform.position = position;
                pickup.passiveItem = loot.DroppedItem.passiveItem;
                return;
            }
        }
    }
}