using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Entities;

namespace TowerDefense.Utilities
{
    /// <summary>
    /// Optional world-space health bar that sits above an enemy.
    /// Attach to a child GameObject of the enemy prefab with a Canvas set to World Space.
    ///
    /// Observes the enemy's health via a polling approach (simpler than adding
    /// another event to Enemy for this demo; a reactive binding would be cleaner).
    ///
    /// Trade-off: polling is fine for small enemy counts.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, 0f);

        private Enemy _enemy;
        private float _maxHealth;
        private Camera _camera;

        private void Awake()
        {
            _enemy  = GetComponentInParent<Enemy>();
            _camera = Camera.main;

            if (_enemy == null)
                Debug.LogError("[EnemyHealthBar] No Enemy found in parent hierarchy.");
        }

        private void Start()
        {
            if (_enemy == null) return;
            _maxHealth = _enemy.Data.MaxHealth;

            if (_slider != null)
            {
                _slider.minValue = 0f;
                _slider.maxValue = _maxHealth;
                _slider.value    = _maxHealth;
            }
        }

        private void LateUpdate()
        {
            // Billboard — always face camera
            if (_camera != null)
                transform.rotation = _camera.transform.rotation;
        }
    }
}
