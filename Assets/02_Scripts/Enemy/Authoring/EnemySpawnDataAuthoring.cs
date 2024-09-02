using Unity.Entities;
using UnityEngine;

public class EnemySpawnDataAuthoring : MonoBehaviour
{

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _mapDistance;
    [SerializeField] private float _waveTime;
    [SerializeField] private float _waveTimeDecrease;
    [SerializeField] private float _minWaveTime;


    private class EnemySpawnDataBaker : Baker<EnemySpawnDataAuthoring>
    {
        public override void Bake(EnemySpawnDataAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.None);
            var data = new EnemySpawnData
            {

                mapDistance = authoring._mapDistance,
                waveTime = authoring._waveTime,
                waveTimeDecrease = authoring._waveTimeDecrease,
                minWaveTime = authoring._minWaveTime,
                enemyPrefab = GetEntity(authoring._enemyPrefab, TransformUsageFlags.Dynamic),
                currentWave = 0,

            };

            AddComponent(entity, data);

        }
    }
}