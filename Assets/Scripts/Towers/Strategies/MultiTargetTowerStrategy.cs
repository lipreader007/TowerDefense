using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Entities;
using TowerDefense.Interfaces;

namespace TowerDefense.Towers.Strategies
{
    /// <summary>
    /// Multi-target strategy: fires one direct-damage projectile at each of the
    /// top N enemies in range (N = TowerData.MaxTargets).
    ///
    /// Each projectile deals full damage with no splash — visually distinct from AOE.
    /// </summary>
    public class MultiTargetTowerStrategy : ITowerStrategy
    {
        public void Execute(List<Enemy> enemiesInRange, Tower owner)
        {
            var data    = owner.Data;
            var prefab  = data.ProjectilePrefab;

            if (prefab == null)
            {
                Debug.LogWarning("[MultiTargetTowerStrategy] No ProjectilePrefab assigned on TowerData.");
                return;
            }

            int count = Mathf.Min(data.MaxTargets, enemiesInRange.Count);

            for (int i = 0; i < count; i++)
            {
                Enemy target = enemiesInRange[i];
                if (target == null || target.IsDead) continue;

                GameObject go = Object.Instantiate(
                    prefab,
                    owner.FirePoint.position,
                    Quaternion.identity);

                if (!go.TryGetComponent<Projectile>(out var projectile))
                {
                    Debug.LogError("[MultiTargetTowerStrategy] ProjectilePrefab has no Projectile component.");
                    Object.Destroy(go);
                    continue;
                }

                // Capture loop variable for the lambda
                Enemy captured = target;
                float dmg      = data.Damage;

                projectile.Initialise(
                    captured.transform,
                    data.ProjectileSpeed,
                    dmg,
                    (hitPos, d) => OnHit(hitPos, d, captured));
            }
        }

        // ── Hit callback ──────────────────────────────────────────────────────
        private static void OnHit(Vector3 hitPosition, float damage, Enemy target)
        {
            if (target == null || target.IsDead) return;
            target.TakeDamage(damage);
        }
    }
}
