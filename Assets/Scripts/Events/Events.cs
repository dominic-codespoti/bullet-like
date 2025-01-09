using Loot;
using UnityEngine;

namespace Events
{
    public class BaseEvent
    {
    }

    public class DamageTakenEvent : BaseEvent
    {
        public DamageTakenEvent(float damage)
        {
            Damage = damage;
        }

        public float Damage { get; private set; }
    }
    
    public class EnemyKilledEvent : BaseEvent
    {
        public Vector3 Position { get; private set; }
        public EnemyKilledEvent(Vector3 position)
        {
            Position = position;
        }
    }
    
    public class OnItemPickupEvent : BaseEvent
    {
        public OnItemPickupEvent(PickupItem item)
        {
            Item = item;
        }

        public PickupItem Item { get; private set; }
    }
}