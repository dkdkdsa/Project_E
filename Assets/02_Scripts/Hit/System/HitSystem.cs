using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct HitSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {

        CheckDie(ref state);
        CheckHit(ref state);

    }

    private void CheckDie(ref SystemState state)
    {

        var buf = SystemAPI.GetSingletonBuffer<DestroyEntityBuffer>();

        foreach(var item in SystemAPI.Query<HpDieAspect>())
        {

            if(item.hp.ValueRO.hp <= 0)
                buf.Add(new() { entity = item.entity });

        }

    }

    private void CheckHit(ref SystemState state)
    {

        var job = new HitCheckJob
        {

            dt = SystemAPI.Time.DeltaTime,

        };

        state.Dependency = job.ScheduleParallel(state.Dependency);

    }

    [BurstCompile]
    public partial struct HitCheckJob : IJobEntity
    {

        public float dt;

        public void Execute(
            in DynamicBuffer<PhysicsEventBuffer> buffer,
            ref DynamicBuffer<FeedbackBuffer> feedbackBuf,
            ref Hit hit,
            ref Hp hp)
        {

            if (!CheckBuffer(in buffer, in hit))
                return;

            if (hit.isInvincibility)
            {

                hit.currentTime -= dt;

                if (hit.currentTime <= 0)
                    hit.isInvincibility = false;

                return;

            }

            hit.currentTime = hit.invincibilityTime;
            hit.isInvincibility = true;
            hp.hp -= 1;

            feedbackBuf.Add(new FeedbackBuffer { type = FeedbackType.Blink });

        }

        private readonly bool CheckBuffer(in DynamicBuffer<PhysicsEventBuffer> buffer, in Hit hit)
        {

            foreach (var tag in buffer)
            {

                if (hit.isCollison && tag.eventType != PhysicsEventType.Collision)
                    continue;

                if ((tag.entityTag & hit.hitEventTag) == hit.hitEventTag)
                    return true;

            }

            return false;

        }

    }

    public readonly partial struct HpDieAspect : IAspect
    {

        public readonly Entity entity;
        public readonly RefRO<Hp> hp;

    }

}
