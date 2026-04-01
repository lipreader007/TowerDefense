using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Systems;
using TowerDefense.Entities;

namespace TowerDefense.UI
{
    /// <summary>
    /// Dumb view layer. Subscribes to system events and updates UI elements.
    /// Contains zero business logic — if a designer changes the score formula,
    /// nothing in this file changes.
    ///
    /// Trade-off: direct event subscription instead of a ViewModel/Presenter.
    /// Fine for this scope; a larger project would use MVP or a reactive binding system.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TMP_Text _scoreText;

        [Header("Base Health")]
        [SerializeField] private TMP_Text _baseHealthText;
        [SerializeField] private Slider _baseHealthSlider;

        [Header("Game Over Panel")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private Button _restartButton;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Awake()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnEnable()
        {
            ScoreManager.OnScoreChanged    += HandleScoreChanged;
            PlayerBase.OnHealthChanged     += HandleHealthChanged;
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreChanged    -= HandleScoreChanged;
            PlayerBase.OnHealthChanged     -= HandleHealthChanged;
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void Start()
        {
            UpdateScoreDisplay(0);
        }

        // ── Handlers ─────────────────────────────────────────────────────────
        private void HandleScoreChanged(int score)     => UpdateScoreDisplay(score);
        private void HandleHealthChanged(float current, float max) => UpdateHealthDisplay(current, max);
        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
                ShowGameOver();
        }

        // ── Display helpers ───────────────────────────────────────────────────
        private void UpdateScoreDisplay(int score)
        {
            if (_scoreText != null)
                _scoreText.text = $"Score: {score}";
        }

        private void UpdateHealthDisplay(float current, float max)
        {
            if (_baseHealthText != null)
                _baseHealthText.text = $"Base: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";

            if (_baseHealthSlider != null)
            {
                _baseHealthSlider.maxValue = max;
                _baseHealthSlider.value    = current;
            }
        }

        private void ShowGameOver()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);

            if (_finalScoreText != null && GameManager.Instance != null)
            {
                // Fetch current score from the ScoreManager if accessible
                var sm = FindObjectOfType<ScoreManager>();
                int finalScore = sm != null ? sm.Score : 0;
                _finalScoreText.text = $"Final Score: {finalScore}";
            }
        }

        private void OnRestartClicked()
        {
            GameManager.Instance?.RestartGame();
        }
    }
}
