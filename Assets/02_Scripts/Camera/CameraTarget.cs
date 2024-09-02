using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{

    private EntityManager _manager;
    private Entity _target;
    private bool _haveTarget;

    private void Awake()
    {

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

    }

    public void SetTarget(Entity target)
    {

        _target = target;
        _haveTarget = true;

    }

    private void LateUpdate()
    {

        if (!_haveTarget ||  !_manager.IsEnabled(_target)) return;

        var pos = _manager.GetComponentData<LocalToWorld>(_target).Position;
        transform.position = pos;

    }

    public void ReleaseTarget()
    {

        _haveTarget = false;

    }
}
