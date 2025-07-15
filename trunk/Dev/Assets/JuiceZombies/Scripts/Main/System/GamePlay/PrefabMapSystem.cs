//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-20 16:02:32
//---------------------------------------------------------------------

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(HandleInputSystem))]
    public partial class PrefabMapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<GlobalConfigData>();
            RequireForUpdate<PrefabTempBuffer>();
            RequireForUpdate(SystemAPI.QueryBuilder().WithNone<WorldBlackBoardTag>().Build());
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var sbe = SystemAPI.GetSingletonEntity<GlobalConfigData>();

            //Mapping Prefabs
            var prefabTempEntity = SystemAPI.GetSingletonEntity<PrefabTempBuffer>();
            var buffer = SystemAPI.GetSingletonBuffer<PrefabTempBuffer>();

            var nativePrefabMap = new NativeHashMap<FixedString128Bytes, Entity>(buffer.Length, Allocator.Persistent);

            foreach (var bakeData in buffer)
            {
                nativePrefabMap.TryAdd(bakeData.name, bakeData.entity);
                //Debug.LogError($"=={bakeData.name}==");
            }

            //special_effect_skill_10011001
            // if (nativePrefabMap.TryGetValue("special_effect_skill_10011001", out var hotShaderPrefab))
            // {
            //     Debug.Log($"预生成特效 special_effect_skill_10011001");
            //     var hotShaderIns = EntityManager.Instantiate(hotShaderPrefab);
            //     EntityManager.SetComponentData(hotShaderIns, new LocalTransform
            //     {
            //         Position = new float3(9999, 1111, -5555),
            //         Scale = 0.01f,
            //         Rotation = default
            //     });
            //     EntityManager.AddComponentData(hotShaderIns, new TimeToDieData
            //     {
            //         duration = 3
            //     });
            //     EntityManager.AddComponentData(hotShaderIns, new JiYuFrameAnimLoop()
            //     {
            //         value = 2
            //     });
            // }


            EntityManager.AddComponentData(sbe, new PrefabMapData
            {
                prefabHashMap = nativePrefabMap
            });
            EntityManager.RemoveComponent<PrefabTempBuffer>(prefabTempEntity);
            
            Debug.Log($"PrefabMapSystem");
            Enabled = false;
        }
    }
}