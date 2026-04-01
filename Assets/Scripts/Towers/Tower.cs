using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Data;
using TowerDefense.Entities;
using TowerDefense.Interfaces;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Core tower component. Owns:
    ///   - Enemy detection within range (Physics.OverlapSphere each frame)
    ///   - Fire rate timer
    ///   - Delegation to ITowerStrategy for the actual attack
    ///
    /// Adding a new attack type requires only a new ITowerStrategy — this class never changes.
    /// </summary>
    public class Tower : MonoBehaviour
    {
        // ── Inspector ─────────────────────────────────────────────────────────
        [SerializeField] private TowerData _data;
        [SerializeField] private Transform _firePoint;

        // ── Runtime state ─────────────────────────────────────────────────────
        private ITowerStrategy _strategy;
        private float _nextFireTime;
        private readonly List<Enemy> _enemiesInRange = new();

        // ── Public accessors (used by strategies) ─────────────────────────────
        public TowerData Data       => _data;
        public Transform FirePoint  => _firePoint != null ? _firePoint : transform;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Awake()
        {
            _strategy = CreateStrategy(_data.Type);
            if (_strategy == null)
                Debug.LogError($"[Tower] No strategy found for type {_data.Type}");
        }

        private void Update()
        {
            if (Time.time < _nextFireTime) return;

            RefreshEnemiesInRange();

            if (_enemiesInRange.Count == 0) return;

            _strategy.Execute(_enemiesInRange, this);
            _nextFireTime = Time.time + 1f / _data.FireRate;
        }

        // ── Targeting ─────────────────────────────────────────────────────────
        private void RefreshEnemiesInRange()
        {
            _enemiesInRange.Clear();

            // Trade-off: OverlapSphere every frame is fine for a small scene.
            // With more enemies, use a SpatialGrid or subscribe to enter/exit events.
            Collider[] hits = Physics.OverlapSphere(transform.position, _data.Range);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Enemy>(out var enemy) && !enemy.IsDead)
                    _enemiesInRange.Add(enemy);
            }

            // Sort: enemies closest to base (highest PathProgress) first
            _enemiesInRange.Sort((a, b) => b.PathProgress.CompareTo(a.PathProgress));
        }

        // ── Strategy factory ──────────────────────────────────────────────────
        /// <summary>
        /// Constructs the correct strategy from the TowerType enum.
        /// Centralised here so Tower.cs stays clean and strategies stay stateless.
        /// </summary>
        private static ITowerStrategy CreateStrategy(TowerType type)
        {
            return type switch
            {
                TowerType.AOE         => new AOETowerStrategy(),
                TowerType.MultiTarget => new MultiTargetTowerStrategy(),
                _                     => null
            };
        }

        // ── Gizmos ────────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            if (_data == null) return;
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, _data.Range);

            if (_data.Type == TowerType.AOE)
            {
                Gizmos.color = new Color(1f, 0.3f, 0f, 0.2f);
                Gizmos.DrawWireSphere(transform.position, _data.SplashRadius);
            }
        }
    }
}
