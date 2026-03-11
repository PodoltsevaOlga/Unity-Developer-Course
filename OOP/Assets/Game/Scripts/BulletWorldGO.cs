using System;
using System.Collections.Generic;
using Game.Characters;
using Game.Projectiles;
using Modules.Utils;
using UnityEngine;

namespace Game
{
    // +
    public sealed class BulletWorldGO : MonoBehaviour
    {
        [SerializeField]
        private ProjectileConfig _prefab;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private BulletViewConfig _configView;

        [SerializeField]
        private TransformBounds _levelBounds;

        private readonly Stack<ProjectileConfig> _pool = new();
        private readonly List<ProjectileConfig> _bullets = new();

        private void Awake()
        {
            for (var i = 0; i < 10; i++)
            {
                ProjectileConfig bullet = Instantiate(_prefab, _container);
                bullet.gameObject.SetActive(false);
                _pool.Push(bullet);
            }
        }

        private void FixedUpdate()
        {
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                ProjectileConfig bullet = _bullets[i];
                Vector3 moveStep = bullet.direction * bullet.speed * Time.fixedDeltaTime;
                bullet.transform.position += moveStep;

                if (!_levelBounds.InBounds(bullet.transform.position))
                {
                    _bullets.RemoveAt(i);

                    bullet.OnTriggerEntered -= this.OnTriggerEntered;
                    bullet.gameObject.SetActive(false);
                    _pool.Push(bullet);
                }
            }
        }

        public void Spawn(Vector2 position, Vector2 direction, float speed, int damage, Faction team)
        {
            if (_pool.TryPop(out ProjectileConfig bullet))
                bullet.gameObject.SetActive(true);
            else
                bullet = Instantiate(_prefab, _container);
            
            bullet.direction = direction;
            bullet.speed = speed;
            bullet.Damage = damage;
            bullet.OwnerFaction = team;

            bullet.transform.position = position;
            bullet.transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);
            bullet.gameObject.layer = team switch
            {
                Faction.None => LayerMask.NameToLayer("Default"),
                Faction.Player => LayerMask.NameToLayer("PlayerBullet"),
                Faction.Enemy => LayerMask.NameToLayer("EnemyBullet"),
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };

            if (team == Faction.Player)
            {
                bullet.blueVFX.SetActive(true);
                bullet.redVFX.SetActive(false);
            }
            else
            {
                bullet.blueVFX.SetActive(false);
                bullet.redVFX.SetActive(true);
            }

            bullet.OnTriggerEntered += this.OnTriggerEntered;
            _bullets.Add(bullet);
        }

        private void OnTriggerEntered(ProjectileConfig bullet, Collider2D other)
        {
            if (!other.TryGetComponent(out ShipController ship)) 
                return;

            if (bullet.OwnerFaction == Faction.Player && ship is Enemy ||
                bullet.OwnerFaction == Faction.Enemy && ship is PlayerShip)
            {
                // Deal damage to target:
                if (bullet.Damage > 0)
                {
                    ship.currentHealth = Mathf.Clamp(ship.currentHealth - bullet.Damage, 0, ship.config.Health);
                    ship.NotifyAboutHealthChanged(ship.currentHealth);
 
                    if (ship.currentHealth <= 0)
                    {
                        ship.NotifyAboutDead();
                        ship.gameObject.SetActive(false);
                    }
                }

                bullet.OnTriggerEntered -= this.OnTriggerEntered;

                _bullets.Remove(bullet);

                bullet.gameObject.SetActive(false);
                _pool.Push(bullet);

                // Explosion Vfx
                GameObject prefab = _configView.ExplosionVFX;
                Instantiate(prefab, bullet.transform.position, prefab.transform.rotation);
            }
        }
    }
}