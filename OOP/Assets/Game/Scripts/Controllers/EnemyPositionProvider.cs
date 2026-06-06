using System;
using Modules.Utils;
using UnityEngine;

namespace Game.Controllers
{
    [Serializable]
    public class EnemyPositionProvider
    {
        [SerializeField]
        private Transform[] spawnPositions;
        
        [SerializeField]
        private Transform[] attackPositions;
        
        private int currentSpawnPositionIndex;
        private int currentAttackPositionIndex;

        public void Reset()
        {
            ShuffleSpawnPositions();
            ShuffleAttackPositions();
        }

        private void ShuffleSpawnPositions()
        {
            spawnPositions.Shuffle();
            currentSpawnPositionIndex = 0;
        }
        
        private void ShuffleAttackPositions()
        {
            attackPositions.Shuffle();
            currentAttackPositionIndex = 0;
        }
        
        public Vector3 NextSpawnPosition()
        {
            if (currentSpawnPositionIndex >= spawnPositions.Length)
            {
                ShuffleSpawnPositions();
            }

            return spawnPositions[currentSpawnPositionIndex++].position;
        }
        
        public Vector3 NextAttackPosition()
        {
            if (currentAttackPositionIndex >= attackPositions.Length)
            {
                ShuffleAttackPositions();
            }

            return attackPositions[currentAttackPositionIndex++].position;
        }
    }
}