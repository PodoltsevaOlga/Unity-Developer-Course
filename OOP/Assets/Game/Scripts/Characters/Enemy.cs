using System;
using System.Collections.Generic;
using Game.Characters.CharacterComponents;
using Game.Utils;
using UnityEngine;

namespace Game.Characters
{
    public class Enemy : CharacterShip, IPoolableObject
    {
        public CharacterShip Target { get; private set; }
        public override Faction CharacterFaction => Faction.Enemy;
        
        private List<IResetComponent> resetComponents = new();
        public new event Action<Enemy> OnDead;
        
        private bool canMakeActions;

        protected override void OnAwake()
        {
            var resetComps = GetComponents<IResetComponent>();
            if (resetComps != null)
            {
                resetComponents.AddRange(resetComps);
            }
            resetComponents.Add(health);
            resetComponents.Add(weapon);
            
            base.OnDead += (character) => { (character as Enemy)?.OnDead?.Invoke((Enemy)character); };
        }

        public void ForbidActions()
        {
            canMakeActions = false;
        }

        public void Setup(CharacterShip targetCharacterShip, Vector2 destinationPosition)
        {
            Target = targetCharacterShip;
            movementAgent.SetDestination(destinationPosition);
            canMakeActions = true;
        }

        protected override void OnFixedUpdate()
        {
            if (!canMakeActions || Target == null || !Target.IsAlive)
            {
                return;
            }
            
            movementAgent.OnFixedUpdate();

            if (movementAgent.IsReachedDestination(out var _))
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
    }
}