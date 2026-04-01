using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Data;
using TowerDefense.Entities;

namespace TowerDefense.Systems
{
    /// <summary>
    /// Controls enemy wave spawning.
    /// Reads WaveConfig ScriptableObjects; designers build wave lists without code.
    ///
    /// Trade-off: basic sequential wave loop for the demo.
    /// With more time: event-driven wave progression, difficulty scaling,
    /// and per-wave spawn interval overrides.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<WaveConfig> _waves;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameConfig _config;

        [Header("Enemy Prefabs")]
        [Tooltip("Map EnemyData to the prefab that represents it in the scene.\n" +
                 "Order must match the EnemyData assets used in WaveConfigs.")]
        [SerializeField] private List<EnemyPrefabEntry> _enemyPrefabs;

        private int _currentWaveIndex;
        private bool _isSpawning;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Start()
        {
            if (_waves == null || _waves.Count == 0)
            {
                Debug.LogWarning("[WaveManager] No waves configured.");
                return;
            }
            StartCoroutine(RunWaves());
        }

        // ── Wave loop ─────────────────────────────────────────────────────────
        private IEnumerator RunWaves()
        {
            foreach (WaveConfig wave in _waves)
            {
                Debug.Log($"[WaveManager] Starting {wave.WaveName}");
                yield return StartCoroutine(SpawnWave(wave));
                yield return new WaitForSeconds(_config.WaveCooldown);
            }
            Debug.Log("[WaveManager] All waves complete.");
        }

        private IEnumerator SpawnWave(WaveConfig wave)
        {
            foreach (SpawnEntry entry in wave.Entries)
            {
                for (int i = 0; i < entry.Count; i++)
                {
                    SpawnEnemy(entry.EnemyType);
                    yield return new WaitForSeconds(_config.SpawnInterval);
                }
            }
        }

        private void SpawnEnemy(EnemyData data)
        {
            GameObject prefab = GetPrefabForData(data);
            if (prefab == null)
            {
                Debug.LogError($"[WaveManager] No prefab mapped for EnemyData '{data.EnemyName}'.");
                return;
            }

            Vector3 pos = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
            Instantiate(prefab, pos, Quaternion.identity);
        }

        private GameObject GetPrefabForData(EnemyData data)
        {
            foreach (var entry in _enemyPrefabs)
            {
                if (entry.Data == data) return entry.Prefab;
            }
            return null;
        }
    }

    /// <summary>
    /// Simple struct mapping an EnemyData asset to its scene prefab.
    /// Serialised inline on WaveManager so the designer sees it in the inspector.
    /// </summary>
    [System.Serializable]
    public class EnemyPrefabEntry
    {
        public EnemyData Data;
        public GameObject Prefab;
    }
}
