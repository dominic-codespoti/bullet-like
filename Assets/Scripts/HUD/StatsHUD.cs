using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD
{
    public class StatsHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI speedText;

        private PlayerController playerController;

        private void Start()
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            healthText.text = $"Health: {playerController.CurrentHealth}";
            damageText.text = $"Damage: {playerController.Damage}";
            speedText.text = $"Speed: {playerController.Speed}";
        }
    }
}