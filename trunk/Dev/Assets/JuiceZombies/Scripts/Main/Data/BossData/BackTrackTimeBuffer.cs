//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2024-11-08 11:51:46
//---------------------------------------------------------------------


using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    /// <summary>
    /// boss技能 时间回溯buffer
    /// </summary>
    public struct BackTrackTimeBuffer : IBufferElementData
    {
        public float3 position;
    }
}