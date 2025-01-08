using Player;
using UnityEngine;

namespace Loot
{
    [CreateAssetMenu(menuName = "Loot/PassiveItem")]
    public class PassiveItem : ScriptableObject
    {
        public string itemName;
        public string statToUpgrade;
        public float upgradeValue;
        public Sprite icon;
    }
}