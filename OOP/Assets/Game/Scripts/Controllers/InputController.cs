using Game.Characters;
using UnityEngine;

namespace Game.Controllers
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] 
        private Player player;
        
        private void Update()
        {
            if (!player.IsAlive)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Space)) 
                player.RequestFire();
            
            float dx = Input.GetAxisRaw("Horizontal");
            float dy = Input.GetAxisRaw("Vertical");
            var direction = new Vector2(dx, dy);
            player.SetDirection(direction);
        }
    }
}