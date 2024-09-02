using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct AttackSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        AttackCoolDown(ref state);

    }

    [BurstCompile]
    private void AttackCoolDown(ref SystemState state)
    {

        var job = new AttackCoolDownJob
        {

            dt = SystemAPI.Time.DeltaTime,

        };

        job.ScheduleParallel();

    }

    [BurstCompile]
    public partial struct AttackCoolDownJob : IJobEntity
    {

        [ReadOnly] public float dt;

        public void Execute(ref Attack attack)
        {

            if (!attack.isCooldown) return;

            attack.currentTime -= dt;

            if(attack.currentTime <= 0)
            {

                attack.isCooldown = false;

            }

        }

    }

}
