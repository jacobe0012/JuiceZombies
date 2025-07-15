//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2023-08-10 16:56:00
//---------------------------------------------------------------------

using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class BoardDataAuthoring : MonoBehaviour
    {
        public BoardTypeEnum type;

        public class BoardDataAuthoringBaker : Baker<BoardDataAuthoring>
        {
            public override void Bake(BoardDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ObstacleTag>(entity);
                AddComponent(entity, new BoardData
                {
                    type = authoring.type
                });
                AddComponent(entity, new TargetData
                {
                    tick = 0,
                    BelongsTo = (uint)BuffHelper.TargetEnum.Obstacle,
                    AttackWith = 0
                });
            }
        }
    }

    public enum BoardTypeEnum
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    public struct BoardData : IComponentData
    {
        /// <summary>
        /// 0 1 2 3 上下左右
        /// </summary>
        public BoardTypeEnum type;
    }
}