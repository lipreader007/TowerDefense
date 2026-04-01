using System;
using UnityEngine;
using TowerDefense.Data;
using TowerDefense.Interfaces;

namespace TowerDefense.Entities
{
    /// <summary>
    /// Core enemy entity. Owns:
    ///   - Movement along the shared Path
    ///   - Current health and damage reception
    ///   - Hit / death VFX spawning
    ///   - Events consumed by ScoreManager and GameManager
    ///
    /// Does NOT know about scoring, game state, or towers — those systems
    /// listen to this class's events instead of being coupled to it.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        // ── Events ────────────────────────────────────────────────────────────
        /// <summary>Fired when this enemy reaches the player base.</summary>
        public static event Action<Enemy> OnEnemyReachedBase;

        /// <summary>Fired when this enemy's health drops to zero.</summary>
        public static event Action<Enemy> OnEnemyDied;

        // ── Inspector ─────────────────────────────────────────────────────────
        [SerializeField] private EnemyData _data;

        // ── Runtime state ─────────────────────────────────────────────────────
        private float _currentHealth;
        private int _waypointIndex;
        private Path _path;
        private bool _isDead;

        // ── Public accessors ──────────────────────────────────────────────────
        public EnemyData Data => _data;
        public bool IsDead => _isDead;

        /// <summary>
        /// Normalised path progress [0..1]. Towers use this to prioritise
        /// enemies that are closest to the base.
        /// </summary>
        public float PathProgress => _path != null ? _path.GetProgress(_waypointIndex) : 0f;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Awake()
        {
            _path = FindObjectOfType<Path>();
            if (_path == null)
                Debug.LogError("[Enemy] No Path found in scene.");
        }

        private void Start()
        {
            _currentHealth = _data.MaxHealth;
            _waypointIndex = 0;

            if (_path != null)
                transform.position = _path.GetWaypoint(0);
        }

        private void Update()
        {
            if (_isDead) return;
            MoveAlongPath();
        }

        // ── Movement ──────────────────────────────────────────────────────────
        private void MoveAlongPath()
        {
            if (_path == null || _waypointIndex >= _path.WaypointCount) return;

            Vector3 target = _path.GetWaypoint(_waypointIndex);
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                _data.MoveSpeed * Time.deltaTime);

            // Face movement direction
            Vector3 direction = target - transform.position;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);

            if (Vector3.Distance(transform.position, target) < 0.05f)
                AdvanceWaypoint();
        }

        private void AdvanceWaypoint()
        {
            _waypointIndex++;
            if (_waypointIndex >= _path.WaypointCount)
                ReachBase();
        }

        private void ReachBase()
        {
            OnEnemyReachedBase?.Invoke(this);
            // Enemy is consumed on arrival — no VFX, just disappears
            Destroy(gameObject);
        }

        // ── IDamageable ───────────────────────────────────────────────────────
        public void TakeDamage(float amount)
        {
            if (_isDead) return;

            _currentHealth -= amount;
            SpawnVFX(_data.HitVFXPrefab);

            if (_currentHealth <= 0f)
                Die();
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            SpawnVFX(_data.DeathVFXPrefab);
            OnEnemyDied?.Invoke(this);
            Destroy(gameObject);
        }

        // ── VFX ───────────────────────────────────────────────────────────────
        private void SpawnVFX(GameObject prefab)
        {
            if (prefab == null) return;
            GameObject vfx = Instantiate(prefab, transform.position, Quaternion.identity);
            Destroy(vfx, _data.VFXLifetime);
        }

        // ── Gizmos ────────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            // Show health bar approximation in scene view
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
        }
    }
}
