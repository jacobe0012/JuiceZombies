//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-30 12:09:01
//---------------------------------------------------------------------

using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public class DropsDataAuthoring : MonoBehaviour
    {
        //[SerializeField] public DropsData DropsData;


        class DropsDataBaker : Baker<DropsDataAuthoring>
        {
            public override void Bake(DropsDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DropsData>(entity);
            }
        }
    }


    [Serializable]
    public struct DropsData : IComponentData
    {
        public int id;

        //贝塞尔曲线data
        [HideInInspector] public float3 point0;
        [HideInInspector] public float3 point1;
        [HideInInspector] public float3 point2;
        [HideInInspector] public float3 point3;

        /// <summary>
        /// 是否已经播完动画
        /// </summary>
        [HideInInspector] public bool isDropAnimed;

        [HideInInspector] public bool isLooting;
        [HideInInspector] public float lootingAniDuration;
        [HideInInspector] public float dropAnimedElpase;
        [HideInInspector] public float3 dropPoint0;
        [HideInInspector] public float3 dropPoint1;
        [HideInInspector] public float3 dropPoint2;

        [HideInInspector] public int tick;
    }
}