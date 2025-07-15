using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class TimeToDieAuthoring : MonoBehaviour
    {
        public float time;

        public class SwitchSceneAuthoringBaker : Baker<TimeToDieAuthoring>
        {
            public override void Bake(TimeToDieAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TimeToDieData
                {
                    duration = authoring.time
                });
            }
        }
    }

    public struct TimeToDieData : IComponentData
    {
        //public int type;
        public float duration;
        
        //public float dissolveTime;
    }
}