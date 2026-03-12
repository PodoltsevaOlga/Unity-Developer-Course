using Game.Characters;
using UnityEngine;

namespace Game.Controllers
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] 
        private Player player;

        [SerializeField] 
        private KeyCode fireButton = KeyCode.Space;
        
        private void Update()
        {
            if (!player.IsAlive)
            {
                return;
            }
            
            if (Input.GetKeyDown(fireButton)) 
                player.RequestFire();
            
            float dx = Input.GetAxisRaw("Horizontal");
            float dy = Input.GetAxisRaw("Vertical");
            var direction = new Vector2(dx, dy);
            player.SetDirection(direction);
        }
    }
}