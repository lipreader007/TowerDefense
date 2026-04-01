using UnityEngine;

namespace TowerDefense.Data
{
    /// <summary>
    /// Enumerates the available tower attack strategies.
    /// Drives which ITowerStrategy implementation is used at runtime.
    /// </summary>
    public enum TowerType
    {
        AOE,
        MultiTarget
    }

    /// <summary>
    /// Pure data container for a tower type.
    /// Designers can create new tower variants by duplicating this asset.
    /// </summary>
    [CreateAssetMenu(fileName = "TowerData_New", menuName = "TowerDefense/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("Identity")]
        public string TowerName = "Tower";
        public TowerType Type = TowerType.AOE;

        [Header("Combat")]
        [Tooltip("Radius within which the tower detects and fires at enemies")]
        public float Range = 8f;

        [Tooltip("Shots fired per second")]
        public float FireRate = 1f;

        [Tooltip("Damage dealt per projectile hit")]
        public float Damage = 25f;

        [Header("AOE Settings (TowerType.AOE only)")]
        [Tooltip("Radius of the explosion splash around the impact point")]
        public float SplashRadius = 3f;

        [Tooltip("Damage at the edge of the splash (centre always receives full Damage)")]
        public float SplashDamageMultiplier = 0.5f;

        [Header("Multi-Target Settings (TowerType.MultiTarget only)")]
        [Tooltip("Maximum number of enemies targeted simultaneously")]
        public int MaxTargets = 3;

        [Header("Projectile")]
        [Tooltip("Projectile prefab spawned when the tower fires")]
        public GameObject ProjectilePrefab;

        [Tooltip("Speed the projectile travels toward its target (units per second)")]
        public float ProjectileSpeed = 10f;
    }
}
