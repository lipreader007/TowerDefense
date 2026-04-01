using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Entities;

namespace TowerDefense.Towers.Strategies
{
    /// <summary>
    /// AOE strategy: selects the highest-priority enemy (closest to base),
    /// fires a single projectile whose impact callback splashes all enemies
    /// within SplashRadius.
    ///
    /// Splash uses a simple multiplier for edge damage — not a full falloff curve.
    /// Trade-off noted in TDD; a proper falloff curve is a one-line change to OnHit.
    /// </summary>
    public class AOETowerStrategy : ITowerStrategy
    {
        public void Execute(List<Enemy> enemiesInRange, Tower owner)
        {
            // List is pre-sorted by PathProgress descending — index 0 is highest priority
            Enemy primaryTarget = enemiesInRange[0];
            if (primaryTarget == null) return;

            var data    = owner.Data;
            var prefab  = data.ProjectilePrefab;

            if (prefab == null)
            {
                Debug.LogWarning("[AOETowerStrategy] No ProjectilePrefab assigned on TowerData.");
                return;
            }

            GameObject go = Object.Instantiate(
                prefab,
                owner.FirePoint.position,
                Quaternion.identity);

            if (!go.TryGetComponent<Projectile>(out var projectile))
            {
                Debug.LogError("[AOETowerStrategy] ProjectilePrefab has no Projectile component.");
                Object.Destroy(go);
                return;
            }

            projectile.Initialise(
                primaryTarget.transform,
                data.ProjectileSpeed,
                data.Damage,
                (hitPos, dmg) => OnHit(hitPos, dmg, data.SplashRadius,
                                       data.SplashDamageMultiplier));
        }

        // ── Hit callback ──────────────────────────────────────────────────────
        private static void OnHit(Vector3 hitPosition, float damage,
                                  float splashRadius, float edgeMultiplier)
        {
            Collider[] hits = Physics.OverlapSphere(hitPosition, splashRadius);
            foreach (var col in hits)
            {
                if (!col.TryGetComponent<Enemy>(out var enemy)) continue;
                if (enemy.IsDead) continue;

                // Enemies at the centre receive full damage; edge receives reduced damage.
                float dist       = Vector3.Distance(hitPosition, enemy.transform.position);
                float t          = Mathf.Clamp01(dist / splashRadius);
                float finalDmg   = Mathf.Lerp(damage, damage * edgeMultiplier, t);

                enemy.TakeDamage(finalDmg);
            }
        }
    }
}
