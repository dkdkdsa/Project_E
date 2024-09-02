using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct CollisionData : IComponentData
{

    public Tag ignoreTag;

    public CollisionData(Tag ignoreTag)
    {
        this.ignoreTag = ignoreTag;
    }

}
