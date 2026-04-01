using UnityEngine;
using TowerDefense.Interfaces;

namespace TowerDefense.Systems
{
    /// <summary>
    /// Handles all player input.
    /// Currently: left-click → raycast → damage IDamageable.
    ///
    /// Kept separate from Enemy so new click behaviours (tower placement, UI)
    /// can be added without touching entity code.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Damage dealt to an enemy on a single click")]
        private float _clickDamage = 20f;

        [SerializeField]
        [Tooltip("Layer mask for enemy colliders — keep this filtered for performance")]
        private LayerMask _enemyLayerMask;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
                Debug.LogError("[InputManager] No main camera found.");
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                HandleClick();
        }

        private void HandleClick()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _enemyLayerMask))
                return;

            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(_clickDamage);
        }
    }
}
