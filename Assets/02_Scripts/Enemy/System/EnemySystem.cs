using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct EnemySystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        TargetSetup(ref state);

    }

    [BurstCompile]
    private void TargetSetup(ref SystemState state)
    {

        float3 playerPos = float3.zero;

        foreach((_, var trm, var mass) in SystemAPI.Query<RefRO<Player>, RefRO<LocalToWorld>, RefRW<PhysicsMass>>())
        {

            playerPos = trm.ValueRO.Position;

        }

        var job = new TargetSetupJob
        {

            dt = SystemAPI.Time.DeltaTime,
            playerPos = playerPos,

        };

        state.Dependency = job.Schedule(state.Dependency);

    }

    [BurstCompile]
    public partial struct TargetSetupJob : IJobEntity
    {

        [ReadOnly] public float dt;
        [ReadOnly] public float3 playerPos;

        public void Execute(ref Enemy enemy, ref NavAgent agent, ref PhysicsMass mass, ref PhysicsVelocity vel)
        {

            if (agent.inJob || agent.needFind) return;

            mass.InverseInertia.x = 0;
            mass.InverseInertia.z = 0;
            vel.Angular = float3.zero;

            if (enemy.currentTime > 0)
            {

                enemy.currentTime -= dt;
                return;

            }

            agent.targetPoint = playerPos;
            agent.needFind = true;
            agent.havePath = false;
            enemy.currentTime = 0.5f;

        }

    }

}