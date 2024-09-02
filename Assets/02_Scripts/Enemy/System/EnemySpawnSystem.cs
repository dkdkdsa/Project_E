using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public partial struct EnemySpawnSystem : ISystem
{

    private Random _random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        _random = Random.CreateFromIndex((uint)SystemAPI.Time.ElapsedTime);

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        SpawningEnemy(ref state);

    }

    [BurstCompile]
    private void SpawningEnemy(ref SystemState state)
    {

        var buf = SystemAPI.GetSingletonBuffer<CreateEntityBuffer>();

        var job = new EnemySpawnJob
        {

            buffer = buf,
            random = _random,
            dt = SystemAPI.Time.DeltaTime,

        };

        state.Dependency = job.Schedule(state.Dependency);

    }

    [BurstCompile]
    public partial struct EnemySpawnJob : IJobEntity
    {

        public Random random;
        public DynamicBuffer<CreateEntityBuffer> buffer;
        public float dt;

        public void Execute(ref EnemySpawnData data)
        {

            if (data.currentTime > 0)
            {

                data.currentTime -= dt;
                return;

            }

            var target = random.NextInt(10, 30);
            for (int i = 0; i < target; i++)
            {

                float2 dir = random.NextFloat2Direction() * data.mapDistance;
                float3 pos = new float3(dir.x, 1, dir.y);
                buffer.Add(new()
                {

                    position = pos,
                    rotation = quaternion.identity,
                    prefab = data.enemyPrefab,

                });

            }

            data.currentTime = data.waveTime;
            data.waveTime -= data.waveTimeDecrease;
            data.waveTime = math.clamp(data.waveTime, data.minWaveTime, 100);

        }

    }

}
