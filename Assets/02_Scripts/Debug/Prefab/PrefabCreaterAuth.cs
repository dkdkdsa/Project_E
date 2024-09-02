using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PrefabCreaterAuth : MonoBehaviour
{

    [SerializeField] private GameObject _prefab;

    private class PrefabBaker : Baker<PrefabCreaterAuth>
    {
        public override void Bake(PrefabCreaterAuth authoring)
        {

            var entity = GetEntity(TransformUsageFlags.None);
            var ins = GetEntity(authoring._prefab, TransformUsageFlags.None);

            var compo = new PrefabCreater
            {

                prefab = ins,

            };

            AddComponent(entity, compo);

        }

    }

}
