using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters
{
    public class Enemy : Character
    {
        public Character Target { get; private set; }

        private List<IResetComponent> resetComponents = new();
        public new event Action<Enemy> OnDead;

        private void Awake()
        {
            var resetComps = GetComponents<IResetComponent>();
            if (resetComps != null)
            {
                resetComponents.AddRange(resetComps);
            }
        }

        public void OnSpawn()
        {
            foreach (var resetComponent in resetComponents)
            {
                resetComponent.ResetValues();
            }
            this.Weapon.ResetValues();
        }

        public void Setup(Character targetCharacter, Vector2 destinationPosition)
        {
            Target = targetCharacter;
            movementAgent.SetDestination(destinationPosition);
        }

        protected override void OnFixedUpdate()
        {
            if (Target == null || !Target.IsAlive)
            {
                return;
            }

            if (movementAgent.IsReachedDestination(out var _))
            {
                this.Weapon.TryToFire();
            }
        }
    }
}