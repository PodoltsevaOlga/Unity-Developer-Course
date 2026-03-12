using System;
using Game.Characters;
using Modules.UI;
using UnityEngine;

namespace Game.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] 
        private HealthView healthView;

        [SerializeField] 
        private Player player;

        private void Start()
        {
            healthView.SetHealth(player.CurrentHealth, player.MaxHealth);
        }

        private void OnEnable()
        {
            player.OnHealthChanged += ChangeHealthInView;
        }

        private void OnDisable()
        {
            player.OnHealthChanged -= ChangeHealthInView;
        }

        private void ChangeHealthInView(int newValue)
        {
            healthView.SetHealth(newValue, player.MaxHealth);
        }
    }
}