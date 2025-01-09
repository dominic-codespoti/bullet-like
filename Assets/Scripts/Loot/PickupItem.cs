using Events;
using Player;
using UnityEngine;

namespace Loot
{
    public class PickupItem : MonoBehaviour
    {
        public PassiveItem passiveItem;

        private void OnCollisionEnter(Collision other)
        {
            var firstPersonController = other.gameObject.GetComponent<FirstPersonController>();
            if (firstPersonController)
            {
                EventManager.Publish(new OnItemPickupEvent(this), this.Id());
                Destroy(gameObject);
            }
        }
    }
}