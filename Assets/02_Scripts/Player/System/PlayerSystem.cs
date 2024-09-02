using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerSystem : SystemBase, InputSetting.IPlayerActions
{

    private NativeList<CreateEntityBuffer> _buffer;
    private InputSetting _input;
    private Vector2 _mousePos;
    private Vector3 _moveVec;
    private bool _isAttack;

    protected override void OnCreate()
    {

        _input = new InputSetting();
        _input.Player.SetCallbacks(this);
        _input.Player.Enable();
        _buffer = new NativeList<CreateEntityBuffer>(Allocator.Persistent);

    }

    protected override void OnStartRunning()
    {

        Entities
            .WithoutBurst()
            .WithPresent<Player>()
            .ForEach((Entity e, ref PhysicsMass mass) =>
            {

                CameraManager.Instance.SetTarget(e);

                mass.InverseInertia.x = 0;
                mass.InverseInertia.z = 0;

            }).Run();

    }

    protected override void OnUpdate()
    {

        Move();
        Rotate();
        Attack();
        CheckDie();

    }

    private void CheckDie()
    {

        int count = 0;

        Entities
            .WithPresent<Player>()
            .ForEach(() =>
            {

                count++;

            }).Run();

        if(count == 0)
        {

            CameraManager.Instance.ReleaseTarget();

        }

    }

    private void Attack()
    {

        if (!_isAttack) return;

        var single = SystemAPI.GetSingletonBuffer<CreateEntityBuffer>();

        Entities
            .WithoutBurst()
            .ForEach((ref Attack atk, in Player player, in LocalTransform trm) =>
            {

                if (atk.isCooldown) return;

                single.Add(new CreateEntityBuffer
                {
                    position = trm.Position,
                    rotation = trm.Rotation,
                    prefab = player.bulletPrefab
                });

                atk.isCooldown = true;
                atk.currentTime = atk.attackCooldown;

            }).Run();
    }

    private void Rotate()
    {

        var y = Camera.main.transform.position.y;
        float3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, y));

        Entities
            .WithPresent<Player>()
            .ForEach
            ((ref LocalTransform trm) =>
            {

                Vector3 dir = math.normalize(mousePos - trm.Position);
                dir.y = 0;

                var rot = Quaternion.LookRotation(dir);

                trm.Rotation = rot;

            }).Run();

    }

    private void Move()
    {

        float3 vec = _moveVec.normalized;
        var dt = SystemAPI.Time.DeltaTime;

        Entities
            .ForEach(
            (ref LocalTransform trm,
            in Player player) =>
            {

                var force = new float3(vec.x, 0, vec.y) * dt * player.speed;
                trm.Position += force;

            }).Run();


    }

    protected override void OnDestroy()
    {

        _input.Dispose();
        _buffer.Dispose();

    }

    public void OnMousePos(InputAction.CallbackContext context)
    {

        _mousePos = context.ReadValue<Vector2>();

    }

    public void OnMove(InputAction.CallbackContext context)
    {

        _moveVec = context.ReadValue<Vector2>();

    }

    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.performed)
            _isAttack = true;
        else if (context.canceled)
            _isAttack = false;

    }
}