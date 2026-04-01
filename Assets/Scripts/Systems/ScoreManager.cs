using System;
using UnityEngine;
using TowerDefense.Entities;

namespace TowerDefense.Systems
{
    /// <summary>
    /// Tracks the player's score.
    /// Subscribes to Enemy.OnEnemyDied — does not need a reference to any enemy.
    /// HUDView subscribes to OnScoreChanged to update the display.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        // ── Events ────────────────────────────────────────────────────────────
        /// <summary>Fires whenever the score changes, passing the new total.</summary>
        public static event Action<int> OnScoreChanged;

        // ── Runtime state ─────────────────────────────────────────────────────
        private int _score;

        public int Score => _score;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void OnEnable()  => Enemy.OnEnemyDied += HandleEnemyDied;
        private void OnDisable() => Enemy.OnEnemyDied -= HandleEnemyDied;

        // ── Handlers ─────────────────────────────────────────────────────────
        private void HandleEnemyDied(Enemy enemy)
        {
            _score += enemy.Data.ScoreValue;
            OnScoreChanged?.Invoke(_score);
        }

        public void ResetScore()
        {
            _score = 0;
            OnScoreChanged?.Invoke(_score);
        }
    }
}
