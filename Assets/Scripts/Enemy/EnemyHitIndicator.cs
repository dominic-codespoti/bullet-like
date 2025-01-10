using System.Collections;
using Events;
using UnityEngine;

namespace Enemy
{
    public class EnemyHitIndicator : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float flashDuration = 0.2f;

        [SerializeField] private Color flashColor = Color.red;

        public void Awake()
        {
            EventManager.Subscribe<DamageTakenEvent>(FlashOnDamage, this.Id());
        }

        private void FlashOnDamage(DamageTakenEvent damageTakenEvent)
        {
            StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            var renderer = GetComponent<Renderer>();
            var originalColor = renderer.material.color;
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            renderer.material.color = originalColor;
        }
    }
}