using System;
using UnityEngine;

namespace Game.Characters
{
    public class Health : MonoBehaviour, IResetComponent
    {
        [SerializeField] 
        private int maxHealthPoints = 10;
        private int currentHealthPoints;

        public int CurrentHealthPoints => currentHealthPoints;

        private void Awake()
        {
            ResetValues();
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