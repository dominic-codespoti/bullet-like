using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Loot
{
    public class LootTable : MonoBehaviour
    {
        [System.Serializable]
        public class Loot
        {
            public PassiveItem passiveItem;
            public float dropChance;
        }

        [SerializeField] private Loot[] lootTable;

        public void Start()
        {
            EventManager.Subscribe<EnemyKilledEvent>(DropLoot, this.Id());
        }

        private void DropLoot(EnemyKilledEvent evt)
        {
            var position = evt.Position;
            foreach (var loot in lootTable)
            {
                if (!(Random.value * 100f <= loot.dropChance)) continue;
                var pickupPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var pickup = pickupPrefab.AddComponent<PickupItem>();
                pickupPrefab.AddComponent<SphereCollider>().isTrigger = true;
                pickupPrefab.AddComponent<Rigidbody>().useGravity = true;
                pickup.passiveItem = loot.passiveItem;

                pickupPrefab.transform.position = position;
                return;
            }
        }
    }
}