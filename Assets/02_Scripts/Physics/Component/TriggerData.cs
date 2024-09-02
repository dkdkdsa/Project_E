using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TriggerData : IComponentData
{

    public Tag ignoreTag;

    public TriggerData(Tag ignoreTag)
    {
        this.ignoreTag = ignoreTag;
    }
}
