using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public struct WeaponData : IComponentData
    {
        public int tick;
        public float3 offset;
        public float scale;
        public quaternion rotation;
        public int weaponId;
        public bool enableEnemyWeapon;

        public float3 rotateOffset;

        //public bool isPlayingAnim;
        public float attackTime;
        public float curAttackTime;

        /// <summary>
        /// 循环间隔 
        /// </summary>
        public float interval;

        /// <summary>
        /// 当前循环间隔 
        /// </summary>
        public float curInterval;

        /// <summary>
        /// 循环次数
        /// </summary>
        public int repeatTimes;

        /// <summary>
        /// 当前循环次数
        /// </summary>
        public int curRepeatTimes;

        //public float totalDistance;
        /// <summary>
        /// 玩家为死亡动画时间 普通boss为普通技能施法动画时间
        /// </summary>
        public float secondAnimTime;
    }


    public class WeaponDataAuthoring : MonoBehaviour
    {
        private float3 offset;
        private float scale;

        private quaternion rotation;
        private float attackTime;
        private float totalDistance;

        [Header("旋转点偏移")] [SerializeField] public float2 rotateOffset;

        public class WeaponDataBaker : Baker<WeaponDataAuthoring>
        {
            public override void Bake(WeaponDataAuthoring authoring)
            {
                // float3 rotateOffset = default;
                // Debug.LogError(authoring.transform.childCount);

                // var child = authoring.transform.GetChild(0).GetComponent<Transform>();
                // if (child != null)
                // {
                //     Debug.LogError(child.name);
                //     rotateOffset = child.position;
                //     GameObject.DestroyImmediate(child.gameObject);
                // }


                // AddComponent(entity, new WeaponData
                // {
                //     offset = tempTransform.position,
                //     scale = tempTransform.localScale,
                //     rotation = tempTransform.rotation,
                //     attackTime = authoring.attackTime,
                //     curAttackTime = 0,
                //     totalDistance = authoring.totalDistance,
                // });

                var entity = GetEntity(TransformUsageFlags.Dynamic);


                AddComponent(entity, new WeaponData
                {
                    offset = authoring.offset,
                    scale = authoring.scale,
                    rotation = authoring.rotation,
                    attackTime = authoring.attackTime,
                    curAttackTime = 0,
                    //totalDistance = authoring.totalDistance,
                    rotateOffset = new float3(authoring.rotateOffset, 0)
                });

                //AddComponent<Flip>(entity);
            }
        }
    }
}