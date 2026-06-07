using System;
using System.Collections.Generic;
using Game.Characters.CharacterComponents;
using Game.Projectiles;
using Game.Utils;
using UnityEngine;

namespace Game.Characters
{
    public class Enemy : CharacterShip, IPoolableObject
    {
        public CharacterShip Target { get; private set; }
        public override Faction CharacterFaction => Faction.Enemy;
        
        private List<IResetComponent> resetComponents = new();
        public event Action<Enemy> OnEnemyDead;
        
        private bool canMakeActions;

        private DestinationComponent destinationComponent;

        protected override void OnAwake()
        {
            var resetComps = GetComponents<IResetComponent>();
            if (resetComps != null)
            {
                resetComponents.AddRange(resetComps);
            }

            destinationComponent = new(statConfiguration.StoppingDistance);
            resetComponents.Add(health);
            resetComponents.Add(weapon);
            resetComponents.Add(destinationComponent);
        }

        public void ForbidActions()
        {
            canMakeActions = false;
        }

        public void Setup(CharacterShip targetCharacterShip, Vector2 destinationPosition,
            ProjectileSpawner projectileSpawner)
        {
            Target = targetCharacterShip;
            destinationComponent.SetDestination(destinationPosition);
            canMakeActions = true;
            weapon.SetProjectileSpawner(projectileSpawner);
        }

        private void FixedUpdate()
        {
            if (!canMakeActions || Target == null || !Target.IsAlive)
            {
                return;
            }

            var currMove = destinationComponent.CalculateMovementVector(transform.position);
            movementAgent.SetDirection(currMove);
            movementAgent.MoveOnFixedUpdate();

            if (destinationComponent.IsReachedDestination(transform.position))
            {
                weapon.TryToFire(Target.transform.position);
            }
        }

        public void OnActivate()
        {
            foreach (var resetComponent in resetComponents)
            {
                resetComponent.ResetValues();
            }
        }

        protected override void OnDying()
        {
            base.OnDying();
            OnEnemyDead?.Invoke(this);
        }
    }
}
