using System;
using UnityEngine;
using TowerDefense.Data;
using TowerDefense.Entities;

namespace TowerDefense.Entities
{
    /// <summary>
    /// The player's base. Listens for enemies reaching it, takes damage,
    /// and fires an event when destroyed.
    ///
    /// Intentionally thin — it is a data holder and event relay.
    /// Game state transitions live in GameManager.
    /// </summary>
    public class PlayerBase : MonoBehaviour
    {
        // ── Events ────────────────────────────────────────────────────────────
        /// <summary>Fired when base HP changes. Passes (currentHP, maxHP).</summary>
        public static event Action<float, float> OnHealthChanged;

        /// <summary>Fired once when base HP reaches zero.</summary>
        public static event Action OnBaseDestroyed;

        // ── Inspector ─────────────────────────────────────────────────────────
        [SerializeField] private GameConfig _config;

        // ── Runtime state ─────────────────────────────────────────────────────
        private float _currentHealth;
        private bool _isDestroyed;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Awake()
        {
            if (_config == null)
            {
                Debug.LogError("[PlayerBase] GameConfig not assigned.");
                return;
            }
            _currentHealth = _config.BaseMaxHealth;
        }

        private void OnEnable()  => Enemy.OnEnemyReachedBase += HandleEnemyReachedBase;
        private void OnDisable() => Enemy.OnEnemyReachedBase -= HandleEnemyReachedBase;

        // ── Handlers ─────────────────────────────────────────────────────────
        private void HandleEnemyReachedBase(Enemy enemy)
        {
            if (_isDestroyed) return;
            TakeDamage(enemy.Data.BaseDamage);
        }

        private void TakeDamage(float amount)
        {
            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            OnHealthChanged?.Invoke(_currentHealth, _config.BaseMaxHealth);

            if (_currentHealth <= 0f)
                DestroyBase();
        }

        private void DestroyBase()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;
            OnBaseDestroyed?.Invoke();
        }
    }
}
