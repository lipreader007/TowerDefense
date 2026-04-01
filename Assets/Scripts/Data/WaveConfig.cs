using System;
using UnityEngine;

namespace TowerDefense.Data
{
    /// <summary>
    /// Defines a single spawn entry within a wave.
    /// </summary>
    [Serializable]
    public class SpawnEntry
    {
        [Tooltip("Which enemy type to spawn")]
        public EnemyData EnemyType;

        [Tooltip("How many of this type to spawn in this wave")]
        public int Count = 5;
    }

    /// <summary>
    /// Defines a full wave: which enemies spawn and how many.
    /// A designer adds waves by creating assets and building a list in WaveManager - no code needed.
    /// </summary>
    [CreateAssetMenu(fileName = "WaveConfig_01", menuName = "TowerDefense/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Tooltip("Label shown in debug logs")]
        public string WaveName = "Wave 1";

        [Tooltip("List of enemy types and counts to spawn in this wave")]
        public SpawnEntry[] Entries;
    }
}
