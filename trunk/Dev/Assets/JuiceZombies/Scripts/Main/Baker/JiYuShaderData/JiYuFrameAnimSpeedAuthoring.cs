using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class JiYuFrameAnimSpeedAuthoring : MonoBehaviour
    {
        class JiYuFrameAnimSpeedAuthoringBaker : Baker<JiYuFrameAnimSpeedAuthoring>
        {
            public override void Bake(JiYuFrameAnimSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuFrameAnimSpeed
                {
                    value = 1
                });
            }
        }
    }

    /// <summary>
    /// ���ٶȸ�Ϊ����һ������ʱ�� /s
    /// </summary>
    [MaterialProperty("_JiYuFrameAnimSpeed")]
    public struct JiYuFrameAnimSpeed : IComponentData
    {
        public float value;
    }
}