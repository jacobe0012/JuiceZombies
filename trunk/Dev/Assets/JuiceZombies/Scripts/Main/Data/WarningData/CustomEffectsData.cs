//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: 2023-10-25 10:38:54
//---------------------------------------------------------------------


using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Main
{
    // public struct CustomEffectsData : IComponentData
    // {
    //     /// <summary>
    //     /// x:angle  y:time  z:speed  w:cycle
    //     /// 当伤害范围显示没有动态变填充的时候speed为0，
    //     /// 当伤害范围显示有且仅有一次扩散填充的时候speed大于0且cycle为0，
    //     /// 当伤害范围显示有持续的扩散填充时speed和cycle都大于0
    //     /// </summary>
    //     public float4 warningData;
    //
    //     /// <summary>
    //     /// x:持续时间  y:生成时间  z:扇形的角度
    //     /// </summary>
    //     public float3 monsterGeneralAttackData;
    // }

    [MaterialProperty("_MonsterGeneralAttackData")]
    public struct MonsterGeneralAttackData : IComponentData
    {
        /// <summary>
        /// x:持续时间  y:生成时间  z:扇形的角度
        /// </summary>
        public float3 Data;
    }
}