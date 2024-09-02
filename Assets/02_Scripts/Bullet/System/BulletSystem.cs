using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct BulletSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        DespawnBullet(ref state);
        MoveBullet(ref state);
        SetupLifeTime(ref state);

    }

    private void DespawnBullet(ref SystemState state)
    {

        var buf = SystemAPI.GetSingletonBuffer<DestroyEntityBuffer>();

        foreach(var item in SystemAPI.Query<DestroyBulletAspect>())
        {

            if(item.bullet.ValueRO.lifeTime <= 0)
            {

                buf.Add(new() { entity = item.entity });

            }

        }

    }

    [BurstCompile]
    private void SetupLifeTime(ref SystemState state)
    {

        var job = new BulletLifeTimeJob
        {

            dt = SystemAPI.Time.DeltaTime

        };

        state.Dependency = job.ScheduleParallel(state.Dependency);

    }

    [BurstCompile]
    private void MoveBullet(ref SystemState state)
    {

        var job = new BulletMovementJob();
        state.Dependency = job.ScheduleParallel(state.Dependency);

    }

    [BurstCompile]
    public partial struct BulletMovementJob : IJobEntity
    {

        public void Execute(
            ref PhysicsVelocity vel,
            in LocalTransform trm,
            in Bullet bullet)
        {

            vel.Linear = trm.Forward() * bullet.speed;

        }

    }

    [BurstCompile]
    public partial struct BulletLifeTimeJob : IJobEntity
    {

        public float dt;

        public void Execute(ref Bullet blt)
        {

            blt.lifeTime -= dt;

        }

    }

    public readonly partial struct DestroyBulletAspect : IAspect
    {

        public readonly Entity entity;
        public readonly RefRO<Bullet> bullet;

    }

}