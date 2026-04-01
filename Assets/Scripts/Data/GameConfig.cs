using UnityEngine;

namespace TowerDefense.Data
{
    /// <summary>
    /// Global game configuration values.
    /// One instance lives in Resources or is referenced by GameManager.
    /// Keeps magic numbers out of code and editable without a code change.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TowerDefense/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Player Base")]
        [Tooltip("Total HP the player base starts with")]
        public float BaseMaxHealth = 100f;

        [Header("Waves")]
        [Tooltip("Delay in seconds between enemy spawns within a wave")]
        public float SpawnInterval = 1.5f;

        [Tooltip("Delay in seconds between waves")]
        public float WaveCooldown = 5f;
    }
}
