using UnityEngine;
using TowerDefense.Interfaces;

namespace TowerDefense.Entities
{
    /// <summary>
    /// A fired projectile. Moves toward its assigned target and applies
    /// damage on arrival. Damage application is delegated to a callback
    /// set by the tower strategy, keeping AOE vs single-hit logic out of this class.
    ///
    /// Trade-off: No pooling — Instantiate/Destroy is used.
    /// With more time: IObjectPool&lt;Projectile&gt; from Unity.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        // ── Runtime state ─────────────────────────────────────────────────────
        private Transform _target;
        private float _speed;
        private float _damage;
        private System.Action<Vector3, float> _onHitCallback;
        private float _lifetime = 5f;   // safety destroy if target dies mid-flight
        private float _spawnTime;

        // ── Initialisation ────────────────────────────────────────────────────
        /// <summary>
        /// Called immediately after instantiation by the tower strategy.
        /// </summary>
        /// <param name="target">Transform to home toward.</param>
        /// <param name="speed">Travel speed in units/second.</param>
        /// <param name="damage">Damage value passed to the hit callback.</param>
        /// <param name="onHitCallback">
        ///   Invoked at the impact position with the damage value.
        ///   AOE strategy passes a splash handler; single-target passes a direct-hit handler.
        /// </param>
        public void Initialise(Transform target, float speed, float damage,
                               System.Action<Vector3, float> onHitCallback)
        {
            _target         = target;
            _speed          = speed;
            _damage         = damage;
            _onHitCallback  = onHitCallback;
            _spawnTime      = Time.time;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Update()
        {
            // Safety: destroy stale projectiles whose targets died mid-flight
            if (_target == null || Time.time - _spawnTime > _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            MoveTowardTarget();
        }

        private void MoveTowardTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(
                transform.position,
                _target.position,
                _speed * Time.deltaTime);

            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);

            if (Vector3.Distance(transform.position, _target.position) < 0.15f)
                Hit();
        }

        private void Hit()
        {
            _onHitCallback?.Invoke(transform.position, _damage);
            Destroy(gameObject);
        }
    }
}
