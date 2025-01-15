using Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace HUD
{
    public class StatsHUD : MonoBehaviour
    {
        private ProgressBar healthBar;
        private PlayerController playerController;

        private void Start()
        {
            playerController = FindFirstObjectByType<PlayerController>();
            var root = GetComponent<UIDocument>().rootVisualElement;
            healthBar = new ProgressBar(root.Q("health-bar"), new StyleColor(Color.red));
        }

        private void Update()
        {
            if (playerController != null)
            {
                healthBar.SetProgress(min: 0, max: playerController.MaxHealth, value: playerController.CurrentHealth);
            }
        }
    }
}