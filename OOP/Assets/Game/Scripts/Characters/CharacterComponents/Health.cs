using System;
using UnityEngine;

namespace Game.Characters.CharacterComponents
{
    public class Health : IResetComponent
    {
        [SerializeField] 
        private int maxHealthPoints = 5;
        private int currentHealthPoints;

        public int CurrentHealthPoints => currentHealthPoints;
        public int MaxHealthPoints => maxHealthPoints;

        public Health(int _maxHealthPoints)
        {
            maxHealthPoints = _maxHealthPoints;
            currentHealthPoints = maxHealthPoints;
        }
        
        public void ResetValues()
        {
            currentHealthPoints = maxHealthPoints;
        }

        public bool ApplyDamage(int hitPoints)
        {
            int previousHealthPoints = currentHealthPoints;
            currentHealthPoints = Math.Max(0, currentHealthPoints - hitPoints);
            return previousHealthPoints != currentHealthPoints;
        }
    }
}