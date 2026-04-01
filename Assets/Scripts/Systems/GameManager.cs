using System;
using UnityEngine;
using TowerDefense.Entities;

namespace TowerDefense.Systems
{
    /// <summary>
    /// Game states.
    /// </summary>
    public enum GameState
    {
        Playing,
        GameOver
    }

    /// <summary>
    /// Owns the game state machine.
    /// Listens to PlayerBase.OnBaseDestroyed and transitions to GameOver.
    /// HUDView listens to OnGameStateChanged.
    ///
    /// Does not own score or wave logic — those are separate managers
    /// following the single-responsibility principle.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ── Singleton ─────────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ── Events ────────────────────────────────────────────────────────────
        /// <summary>Fires whenever the game state changes.</summary>
        public static event Action<GameState> OnGameStateChanged;

        // ── Runtime state ─────────────────────────────────────────────────────
        private GameState _state = GameState.Playing;
        public GameState State => _state;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()  => PlayerBase.OnBaseDestroyed += HandleBaseDestroyed;
        private void OnDisable() => PlayerBase.OnBaseDestroyed -= HandleBaseDestroyed;

        // ── Handlers ─────────────────────────────────────────────────────────
        private void HandleBaseDestroyed()
        {
            TransitionTo(GameState.GameOver);
        }

        private void TransitionTo(GameState newState)
        {
            if (_state == newState) return;
            _state = newState;
            Debug.Log($"[GameManager] State → {_state}");
            OnGameStateChanged?.Invoke(_state);
        }

        // ── Public API ────────────────────────────────────────────────────────
        /// <summary>
        /// Called by the Restart button in the HUD.
        /// Trade-off: scene reload is simple and reliable for a demo.
        /// A proper implementation would reset all systems without reloading.
        /// </summary>
        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
