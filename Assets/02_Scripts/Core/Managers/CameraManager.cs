using System;
using Unity.Entities;

public class CameraManager : MonoSingleton<CameraManager>
{

    public void SetTarget(Entity target)
    {

        FindObjectOfType<CameraTarget>().SetTarget(target);

    }

    public void ReleaseTarget()
    {

        FindObjectOfType<CameraTarget>().ReleaseTarget();

    }

}