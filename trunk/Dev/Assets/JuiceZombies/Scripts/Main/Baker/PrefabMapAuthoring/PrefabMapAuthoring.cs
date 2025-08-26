//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-20 15:48:52
//---------------------------------------------------------------------

using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Main
{
    //预制件映射baker
    public class PrefabMapAuthoring : MonoBehaviour
    {
        [SerializeField] public List<GameObject> prefabs;


        public class PrefabMapAuthoringBaker : Baker<PrefabMapAuthoring>
        {
            public override void Bake(PrefabMapAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<PrefabTempBuffer>(entity);
                var tempDic = new Dictionary<string, GameObject>();

                // 从后往前遍历
                for (int i = authoring.prefabs.Count - 1; i >= 0; i--)
                {
                    if (authoring.prefabs[i])
                    {
                        if (tempDic.ContainsKey(authoring.prefabs[i].name))
                        {
                            // 如果名字已经存在于 HashSet 中，则从列表中删除
                            authoring.prefabs.RemoveAt(i);
                        }
                        else
                        {
                            // 如果名字不在 HashSet 中，则将其添加进去
                            tempDic.Add(authoring.prefabs[i].name, authoring.prefabs[i]);
                        }
                    }
                }

                tempDic.Clear();
                foreach (var prefab in authoring.prefabs)
                {
                    if (prefab)
                    {
                        //Debug.LogError($"{prefab.name}");
                        buffer.Add(new PrefabTempBuffer
                        {
                            name = prefab.name,
                            entity = GetEntity(prefab, TransformUsageFlags.Dynamic)
                        });
                    }
                }
            }
        }
    }
}

public struct PrefabTempBuffer : IBufferElementData
{
    public FixedString128Bytes name;
    public Entity entity;
}