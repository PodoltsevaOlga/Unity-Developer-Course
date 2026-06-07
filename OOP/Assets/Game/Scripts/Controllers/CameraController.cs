using Game.Characters;
using Modules.Utils;
using UnityEngine;

namespace Game.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private CameraShaker cameraShaker;

        [SerializeField] 
        private Player player;

        private void OnEnable()
        {
            player.OnHealthChanged += OnPlayerReceiveDamage;
        }
        
        private void OnDisable()
        {
            player.OnHealthChanged -= OnPlayerReceiveDamage;
        }

        private void OnPlayerReceiveDamage(int _)
        {
            cameraShaker.Shake();
        }
    }
}