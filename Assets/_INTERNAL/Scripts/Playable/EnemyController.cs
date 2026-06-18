using Entity;
using Playable.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playable
{
    public class EnemyController : MonoBehaviour
    {
        // TODO: Контроллер врагов (перемещение/передача в PlayableController событий, связанных с Enemy)
        [Header("Enemy Prefabs Setup")]
        [SerializeField] private Enemy _baseEnemyPrefab;
        [SerializeField] private Enemy _bossEnemyPrefab;
        [SerializeField] private HealthBarView _healthBarViewPrefab;

        [Space(5), Header("Enemy Count Settings")]
        [SerializeField] private int _orcEnemyCount = 6;
        [SerializeField] private int _bossEnemyCount = 1;

        [Space(5), Header("Enemy Waypoints")]
        [SerializeField] private List<Transform> _waypoints = new();

        [Space(5), Header("Enemy Spawn Settings")]
        [SerializeField] private float _spawnDelay = 0.25f;

        [Space(5), Header("Health Bar Settings")]
        [SerializeField] private RectTransform _canvasTransform;

        private readonly List<Enemy> _spawnedEnemy = new();
        private Coroutine _spawnRoutine;

        public event Action<Enemy> OnEnemySpawned;
        public event Action<Enemy> OnEnemyDied;
        public event Action OnPathEnded;

        public void StartEnemySpawn()
        {
            _spawnRoutine = StartCoroutine(EnemySpawnWithDelay());
        }

        public void StopGame()
        {
            for (int i = 0; i < _spawnedEnemy.Count; i++)
                _spawnedEnemy[i].Stop();
        }

        private void SpawnEnemy(bool isBossAvailable = false)
        {
            Transform startPoint = _waypoints[0];

            if (!isBossAvailable)
            {
                HealthBarView healthBarView = Instantiate(_healthBarViewPrefab);
                Enemy enemy = Instantiate(_baseEnemyPrefab, startPoint.position, Quaternion.identity);
                healthBarView.Init(enemy.HpAnchorPoint, _canvasTransform);
                enemy.SetHealthBar(healthBarView);
                enemy.InitializePath(_waypoints);
                enemy.OnEnemyDied += HandleEnemyDeath;
                enemy.OnPathEnded += HandlePathEnded;
                _spawnedEnemy.Add(enemy);
                OnEnemySpawned?.Invoke(enemy);
            }
            else
            {
                HealthBarView healthBarView = Instantiate(_healthBarViewPrefab);
                Enemy enemy = Instantiate(_bossEnemyPrefab, startPoint.position, Quaternion.identity);
                healthBarView.Init(enemy.HpAnchorPoint, _canvasTransform);
                enemy.SetHealthBar(healthBarView);
                enemy.InitializePath(_waypoints);
                enemy.OnEnemyDied += HandleEnemyDeath;
                enemy.OnPathEnded += HandlePathEnded;
                _spawnedEnemy.Add(enemy);
                OnEnemySpawned?.Invoke(enemy);
            }
        }

        private void HandlePathEnded() => OnPathEnded?.Invoke();

        private void HandleEnemyDeath(Enemy enemy)
        {
            enemy.OnEnemyDied -= HandleEnemyDeath;
            enemy.OnPathEnded -= HandlePathEnded;

            enemy.gameObject.SetActive(false);

            _spawnedEnemy.Remove(enemy);

            Destroy(enemy.gameObject);
            OnEnemyDied?.Invoke(enemy);
        }

        private IEnumerator EnemySpawnWithDelay()
        {
            int allEnemiesCount = _orcEnemyCount + _bossEnemyCount;

            while (allEnemiesCount > 0)
            {
                SpawnEnemy();

                if(allEnemiesCount == 1)
                {
                    yield return new WaitForSeconds(_spawnDelay);
                    SpawnEnemy(true);
                }

                yield return new WaitForSeconds(_spawnDelay);

                allEnemiesCount--;
            }

            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }
}