using UnityEngine;
using TowerDefense.Entities;

namespace TowerDefense.Utilities
{
    /// <summary>
    /// Renders a range indicator ring around a tower using a LineRenderer.
    /// Attach to the same GameObject as Tower.
    ///
    /// The ring is always visible (useful for demo/testing).
    /// With more time: show only on mouse-over or selection.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private int _segments = 40;
        [SerializeField] private float _yOffset = 0.05f;

        private LineRenderer _lineRenderer;
        private Tower _tower;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _tower        = GetComponent<Tower>();
        }

        private void Start()
        {
            if (_tower == null || _tower.Data == null) return;
            DrawCircle(_tower.Data.Range);
        }

        private void DrawCircle(float radius)
        {
            _lineRenderer.positionCount = _segments + 1;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.loop          = true;

            float angleStep = 360f / _segments;
            for (int i = 0; i <= _segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x     = Mathf.Cos(angle) * radius;
                float z     = Mathf.Sin(angle) * radius;
                _lineRenderer.SetPosition(i, new Vector3(x, _yOffset, z));
            }
        }
    }
}
