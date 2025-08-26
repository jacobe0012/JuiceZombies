//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-09-21 14:15:10
//---------------------------------------------------------------------

using Unity.Entities;
using UnityEngine;

namespace Main
{
    public struct BossTag : IComponentData
    {
    }

    public class BossTagAuthoring : MonoBehaviour
    {
        //[SerializeField] public GameObject hybridGO;

        //[SerializeField] public ChaStats ChaStats;
        //public int enemyID;

        public class BossTagBaker : Baker<BossTagAuthoring>
        {
            public override void Bake(BossTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);


                // AddComponentObject(entity, new BossHybridTempData
                // {
                //     animationPrefab = authoring.hybridGO
                // });

                AddComponent(entity, new BossTag
                {
                });
                //AddBuffer<BulletCastData>(entity);
            }
        }
    }
}