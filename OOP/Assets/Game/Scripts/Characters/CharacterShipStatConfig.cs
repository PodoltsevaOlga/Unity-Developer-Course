using UnityEngine;

namespace Game.Characters
{
    [CreateAssetMenu(menuName = "Game/New CharacterShipStatConfig")]
    public class CharacterShipStatConfig : ScriptableObject
    {
        [Header("Health")]
        [field: SerializeField]
        public int MaxHealth { get; private set; }
        
        [Header("Movement")]
        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        [field: SerializeField]
        public float StoppingDistance { get; private set; }= 0.25f;
    }
}