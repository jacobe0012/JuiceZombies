using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public struct HybridEventData : IComponentData
    {
        public bool disAble;

       
        public int type;

        public float4 args;

        /// <summary>
        /// 帮派boss的entity
        /// </summary>
        public Entity bossEntity;

        /// <summary>
        /// 字符串参数
        /// </summary>
        public FixedString128Bytes strArgs;
        
        /// <summary>
        /// go
        /// </summary>
        public UnityObjectRef<GameObject> go;
    }
}