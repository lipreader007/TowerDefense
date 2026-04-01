using UnityEngine;

namespace TowerDefense.Entities
{
    /// <summary>
    /// Holds the ordered list of waypoints that enemies follow.
    /// One Path exists in the scene; enemies query it for their next destination.
    /// Trade-off: waypoint array instead of NavMesh — sufficient for this demo,
    /// easy to replace with a proper pathfinding solution later.
    /// </summary>
    public class Path : MonoBehaviour
    {
        [SerializeField] private Transform[] _waypoints;

        /// <summary>Total number of waypoints on this path.</summary>
        public int WaypointCount => _waypoints.Length;

        /// <summary>Returns the world position of waypoint at index.</summary>
        public Vector3 GetWaypoint(int index)
        {
            if (index < 0 || index >= _waypoints.Length)
            {
                Debug.LogError($"[Path] Waypoint index {index} out of range.");
                return Vector3.zero;
            }
            return _waypoints[index].position;
        }

        /// <summary>
        /// Normalized progress [0..1] for an enemy that has reached waypoint index.
        /// Used by towers to prioritise enemies closest to the base.
        /// </summary>
        public float GetProgress(int waypointIndex)
        {
            if (_waypoints.Length <= 1) return 0f;
            return (float)waypointIndex / (_waypoints.Length - 1);
        }

        private void OnDrawGizmos()
        {
            if (_waypoints == null || _waypoints.Length < 2) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _waypoints.Length - 1; i++)
            {
                if (_waypoints[i] != null && _waypoints[i + 1] != null)
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
            }

            Gizmos.color = Color.red;
            foreach (var wp in _waypoints)
            {
                if (wp != null)
                    Gizmos.DrawSphere(wp.position, 0.2f);
            }
        }
    }
}
