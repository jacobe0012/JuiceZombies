//---------------------------------------------------------------------
// JiYuStudio
// Author: 迅捷蟹
// Time: 2023-08-30 14:18:51
//---------------------------------------------------------------------


using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Flow
{
    //[GenerateAuthoringComponent]   类似于Baker的功能,加上这个特性,允许在Mono里对这个组件赋值,然后系统会自动生成代码转成baker
    //网格组件,全局唯一一个,需要添加到某个实体上,基于该实体的组件去实现寻路
    public struct FlowFieldControllerData : IComponentData
    {
        public int2 gridSize;

        //正方体半边长
        public float cellRadius;
    }

    public class FlowFieldControllerDataAuthoring : MonoBehaviour
    {
        [SerializeField] public int2 _gridSize;
        [SerializeField] public float _cellRadius;

        public class FlowFieldControllerDataAuthoringBaker : Baker<FlowFieldControllerDataAuthoring>
        {
            public override void Bake(FlowFieldControllerDataAuthoring authoring)
            {
                var gridEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(gridEntity, new FlowFieldControllerData
                {
                    gridSize = authoring._gridSize,
                    cellRadius = authoring._cellRadius,
                });
            }
        }
    }
}