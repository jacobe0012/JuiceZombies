//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    [DisallowMultipleComponent]
    public class JiYuSortAuthoring : MonoBehaviour
    {
        [SerializeField] public JiYuLayer layer;
        [SerializeField] public int sortIndex;

        public class JiYuSortAuthoringBaker : Baker<JiYuSortAuthoring>
        {
            public override void Bake(JiYuSortAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                int sortIndex = math.clamp(authoring.sortIndex, 0,
                    UnityHelper.SortingIndexCount);
                int sortLayer = math.clamp((int)authoring.layer, 0,
                    UnityHelper.LayerCount);

// #if UNITY_STANDALONE
//                  
// int sortIndex = math.clamp(authoring.sortIndex, 0,
//                     UnityHelper.SortingIndexCount);
//                 int sortLayer = math.clamp((int)authoring.layer, 0,
//                     UnityHelper.LayerCount);
// #else
//
//
//                 int sortIndex = math.clamp(UnityHelper.SortingIndexCount - authoring.sortIndex, 0,
//                     UnityHelper.SortingIndexCount);
//                 int sortLayer = math.clamp(UnityHelper.LayerCount - (int)authoring.layer, 0,
//                     UnityHelper.LayerCount);
// #endif

                AddComponent(entity, new JiYuSort
                {
                    value = new int2(sortLayer, sortIndex)
                });
            }
        }
    }

    [Serializable]
    public enum JiYuLayer
    {
        Map = 0,
        Area = 1,
        Obstacle = 2,
        Main = 3,
        UpLayer0 = 4,
        UpLayer1 = 5,
        Top = 6,
        RuntimeUI = 7
    }

    [MaterialProperty("_JiYuSort")]
    public struct JiYuSort : IComponentData
    {
        public int2 value;
    }
}