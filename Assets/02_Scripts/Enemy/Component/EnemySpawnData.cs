using Unity.Entities;

public struct EnemySpawnData : IComponentData
{

    public Entity enemyPrefab;
    public float mapDistance;
    public float waveTime;
    public float waveTimeDecrease;
    public float minWaveTime;
    public int currentWave;
    public float currentTime;

}

public readonly partial struct EnemySpawnDataAspect : IAspect
{

    public readonly Entity entity;
    public readonly RefRW<EnemySpawnData> data;

}
