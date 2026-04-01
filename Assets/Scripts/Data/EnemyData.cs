using UnityEngine;

namespace TowerDefense.Data
{
    /// <summary>
    /// Pure data container for an enemy type.
    /// Designers duplicate this asset to create new enemy variants - no code changes required.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData_New", menuName = "TowerDefense/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name used in debug and future UI")]
        public string EnemyName = "Enemy";

        [Header("Stats")]
        [Tooltip("Total hit points")]
        public float MaxHealth = 100f;

        [Tooltip("Movement speed along the path (units per second)")]
        public float MoveSpeed = 3f;

        [Tooltip("Damage dealt to the player base on arrival")]
        public float BaseDamage = 10f;

        [Tooltip("Score awarded to the player on kill")]
        public int ScoreValue = 10;

        [Header("Visual Feedback")]
        [Tooltip("Particle prefab instantiated at the enemy position on hit")]
        public GameObject HitVFXPrefab;

        [Tooltip("Particle prefab instantiated at the enemy position on death")]
        public GameObject DeathVFXPrefab;

        [Tooltip("How long the VFX GameObject lives before being destroyed (seconds)")]
        public float VFXLifetime = 2f;
    }
}
