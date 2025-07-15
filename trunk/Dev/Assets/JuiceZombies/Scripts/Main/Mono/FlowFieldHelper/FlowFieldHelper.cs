//---------------------------------------------------------------------
// JiYuStudio
// Author: 迅捷蟹
// Time: 2023-08-30 15:00:10
//---------------------------------------------------------------------

using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace Flow
{
    public static class FlowFieldHelper
    {
        /// <summary>
        /// 目的是为了得到周围4个邻居节点的指向
        /// </summary>
        /// <param name="originIndex">二维单元格所在网格的索引下标</param>
        /// <param name="directions">方向,IEnumerable枚举其中的四个方向,实际上就是遍历四个Int2类型数据</param>
        /// <param name="gridSize">网格大小</param>
        /// <param name="results">返回一个所有邻居的方向</param>
        public static void GetNeighborIndices(int2 originIndex, IEnumerable<GridDirection> directions, int2 gridSize,
            ref NativeList<int2> results)
        {
            foreach (int2 curDirection in directions)
            {
                int2 neighborIndex = GetIndexAtRelativePosition(originIndex, curDirection, gridSize);

                if (neighborIndex.x >= 0)
                {
                    results.Add(neighborIndex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originPos">二维单元格所在网格的索引下标</param>
        /// <param name="relativePos">CardinalDirections中的四个方向</param>
        /// <param name="gridSize">网格大小</param>
        /// <returns></returns>
        public static int2 GetIndexAtRelativePosition(int2 originPos, int2 relativePos, int2 gridSize)
        {
            //坐标和,其实这里是向量和,为了邻居这个点的合成方向
            int2 finalPos = originPos + relativePos;
            if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
            {
                return new int2(-1, -1);
            }
            else
            {
                return finalPos;
            }
        }


        //二维数组变量转一维,将单元格在网格的二维索引x,y转到实际物理存储的一维索引上
        public static int ToFlatIndex(int2 index2D, int height)
        {
            return height * index2D.x + index2D.y;
        }

        //鼠标输入相关的
        public static int2 GetCellIndexFromWorldPos(float3 worldPos, int2 gridSize, float cellDiameter)
        {
            float percentX = worldPos.x / (gridSize.x * cellDiameter);
            float percentY = worldPos.z / (gridSize.y * cellDiameter);

            percentX = math.clamp(percentX, 0f, 1f);
            percentY = math.clamp(percentY, 0f, 1f);

            int2 cellIndex = new int2
            {
                x = math.clamp((int)math.floor((gridSize.x) * percentX), 0, gridSize.x - 1),
                y = math.clamp((int)math.floor((gridSize.y) * percentY), 0, gridSize.y - 1)
            };

            return cellIndex;
        }
    }
}