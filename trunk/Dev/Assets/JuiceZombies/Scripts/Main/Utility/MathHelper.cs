//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-24 12:25:25
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

namespace Main
{
    public static class MathHelper
    {
        #region const

        public readonly static float3 picForward = math.up();
        public const int MaxNum = 99999999;

        #endregion


        #region Random

        private const int A = 16807;
        private const int M = 2147483647;
        private const int Q = 127773;
        private const int R = 2836;


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SelectRandomItem(NativeHashMap<int, int> items, Unity.Mathematics.Random random)
        {
            float randomValue = random.NextFloat(0, 100); // 生成随机值
            float weightSum = 0;
            foreach (var item in items)
            {
                weightSum += item.Value / 10000f;

                // 检查随机值是否在当前物品的权重范围内
                if (randomValue <= weightSum)
                {
                    return item.Key; // 返回选中的物品ID
                }
            }

            return -1;
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRandomSeed()
        {
            return Random.Range(1, int.MaxValue);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRandomBySeed(this ref int seed)
        {
            var hi = seed / Q;
            var lo = seed % Q;
            seed = A * lo - R * hi;
            if (seed <= 0) seed += M;
            return seed * 1.0f / M;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRandomSeedBySeed(this ref int seed)
        {
            return (int)(int.MaxValue * seed.GetRandomBySeed());
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRandomRange(this ref int seed, int min, int max)
        {
            return (int)math.floor(seed.GetRandomBySeed() * (max - min) + min);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRandomRange(this ref int seed, float min, float max)
        {
            return seed.GetRandomBySeed() * (max - min) + min;
        }

        #endregion

        #region float2

        /// <summary>
        /// 返回圆环上的一点
        /// </summary>
        /// <param name="centerPos"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetPointOnCircle(float3 centerPos, float radius, float angle)
        {
            // 将角度转换为弧度
            float angleRadians = math.radians(angle);

            // 计算点的坐标
            float x = centerPos.x + radius * math.sin(angleRadians);
            float y = centerPos.y + radius * math.cos(angleRadians);

            // 返回计算得到的点（y坐标保持为圆心的y坐标）
            return new float3(x, y, 0);
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float VectorAngleUnsign(float3 from, float3 to)
        {
            float num = (float)math.sqrt((double)(math.length(from) * math.length(from)) * (double)(math.length(to) *
                math.length(to)));
            return (double)num < 1.00000000362749E-15
                ? 0.0f
                : (float)math.acos((double)math.clamp(math.dot(from, to) / num, -1f, 1f)) * 57.29578f;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float VectorAngleSign(float3 from, float3 to)
        {
            float angle;
            float3 cross = math.cross(from, to);
            angle = VectorAngleUnsign(from, to);
            return cross.y > 0 ? -angle : angle;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ToFloat3(this float2 v)
        {
            return new float3(v.x, 0, v.y);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 ToFloat2(this float3 v)
        {
            return new float2(v.x, v.z);
        }

        #endregion

        #region float

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckMax(this ref float f, float max)
        {
            if (f > max) f = max;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckMin(this ref float f, float min)
        {
            if (f < min) f = min;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetMax(this ref float f, float other)
        {
            if (other > f) f = other;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetMin(this ref float f, float other)
        {
            if (other < f) f = other;
        }

        #endregion

        #region int

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckMax(this ref int i, int max)
        {
            if (i > max) i = max;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckMin(this ref int i, int min)
        {
            if (i < min) i = min;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetMax(this ref int i, int other)
        {
            if (other > i) i = other;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetMin(this ref int i, int other)
        {
            if (other < i) i = other;
        }

        #endregion

        #region toEuler

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Qua2Euler(quaternion q, math.RotationOrder order = math.RotationOrder.Default)
        {
            const float epsilon = 1e-6f;

            //prepare the data
            var qv = q.value;
            var d1 = qv * qv.wwww * new float4(2.0f); //xw, yw, zw, ww
            var d2 = qv * qv.yzxw * new float4(2.0f); //xy, yz, zx, ww
            var d3 = qv * qv;
            var euler = new float3(0.0f);

            const float CUTOFF = (1.0f - 2.0f * epsilon) * (1.0f - 2.0f * epsilon);

            switch (order)
            {
                case math.RotationOrder.ZYX:
                {
                    var y1 = d2.z + d1.y;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = -d2.x + d1.z;
                        var x2 = d3.x + d3.w - d3.y - d3.z;
                        var z1 = -d2.y + d1.x;
                        var z2 = d3.z + d3.w - d3.y - d3.x;
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), math.atan2(z1, z2));
                    }
                    else //zxz
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.z, d1.y, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z); //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.ZXY:
                {
                    var y1 = d2.y - d1.x;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = d2.x + d1.z;
                        var x2 = d3.y + d3.w - d3.x - d3.z;
                        var z1 = d2.z + d1.y;
                        var z2 = d3.z + d3.w - d3.x - d3.y;
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), math.atan2(z1, z2));
                    }
                    else //zxz
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.z, d1.y, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z); //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.YXZ:
                {
                    var y1 = d2.y + d1.x;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = -d2.z + d1.y;
                        var x2 = d3.z + d3.w - d3.x - d3.y;
                        var z1 = -d2.x + d1.z;
                        var z2 = d3.y + d3.w - d3.z - d3.x;
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), math.atan2(z1, z2));
                    }
                    else //yzy
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.x, d1.z, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z); //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.YZX:
                {
                    var y1 = d2.x - d1.z;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = d2.z + d1.y;
                        var x2 = d3.x + d3.w - d3.z - d3.y;
                        var z1 = d2.y + d1.x;
                        var z2 = d3.y + d3.w - d3.x - d3.z;
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), math.atan2(z1, z2));
                    }
                    else //yxy
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.x, d1.z, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z); //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.XZY:
                {
                    var y1 = d2.x + d1.z;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = -d2.y + d1.x;
                        var x2 = d3.y + d3.w - d3.z - d3.x;
                        var z1 = -d2.z + d1.y;
                        var z2 = d3.x + d3.w - d3.y - d3.z;
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), math.atan2(z1, z2));
                    }
                    else //xyx
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.x, d1.z, d2.z, d1.y);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z); //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.XYZ:
                {
                    var y1 = d2.z - d1.y;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = d2.y + d1.x;
                        var x2 = d3.z + d3.w - d3.y - d3.x;
                        var z1 = d2.x + d1.z;
                        var z2 = d3.x + d3.w - d3.y - d3.z;
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), math.atan2(z1, z2));
                    }
                    else //xzx
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.z, d1.y, d2.x, d1.z);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z); //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), 0.0f);
                    }

                    break;
                }
            }

            return EulerReorderBack(euler, order);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float3 EulerReorderBack(float3 euler, math.RotationOrder order)
        {
            switch (order)
            {
                case math.RotationOrder.XZY:
                    return euler.xzy;
                case math.RotationOrder.YZX:
                    return euler.yzx;
                case math.RotationOrder.YXZ:
                    return euler.yxz;
                case math.RotationOrder.ZXY:
                    return euler.zxy;
                case math.RotationOrder.ZYX:
                    return euler.zyx;
                case math.RotationOrder.XYZ:
                    return euler.xyz;
                default:
                    return euler;
            }
        }


        /// <summary>
        ///   <para>Calculates the signed angle between vectors from and to in relation to axis.</para>
        /// </summary>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        /// <param name="axis">A vector around which the other vectors are rotated.</param>
        /// <returns>
        ///   <para>Returns the signed angle between from and to in degrees.</para>
        /// </returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(float3 from, float3 to, float3 axis = default)
        {
            //项目的默认z轴正方向
            axis = new float3(0, 0, -1);
            float num1 = Angle(from, to);
            float num2 = (float)((double)from.y * (double)to.z - (double)from.z * (double)to.y);
            float num3 = (float)((double)from.z * (double)to.x - (double)from.x * (double)to.z);
            float num4 = (float)((double)from.x * (double)to.y - (double)from.y * (double)to.x);
            float num5 = Sign((float)((double)axis.x * (double)num2 + (double)axis.y * (double)num3 +
                                      (double)axis.z * (double)num4));
            return num1 * num5;
        }


        /// <summary>
        ///   <para>Calculates the angle between vectors from and.</para>
        /// </summary>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        /// <returns>
        ///   <para>The angle in degrees between the two vectors.</para>
        /// </returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(float3 from, float3 to)
        {
            float num = (float)math.sqrt((double)from.SqrMagnitude() * (double)to.SqrMagnitude());
            return (double)num < 1.0000000036274937E-15
                ? 0.0f
                : (float)math.acos((double)math.clamp(math.dot(from, to) / num, -1f, 1f)) * 57.29578f;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(this float3 flo3)
        {
            return (float)((double)flo3.x * (double)flo3.x + (double)flo3.y * (double)flo3.y +
                           (double)flo3.z * (double)flo3.z);
        }

        /// <summary>
        ///   <para>Returns the sign of f.</para>
        /// </summary>
        /// <param name="f"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sign(float f) => (double)f >= 0.0 ? 1f : -1f;

        #endregion

        #region damageNumber

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ConvertNumberisKOrM(int number)
        {
            float result = number;

            if (number >= 1000 && number < 1000000)
            {
                result = number / 1000f;
                result = math.round(result * 100) / 100f;
            }
            else if (number >= 1000000)
            {
                result = number / 1000000f;
                result = math.round(result * 100) / 100f;
            }

            return result;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x4 isKOrM(int sumNumber)
        {
            return new int2x4
            {
                c0 = default,
                c1 = default,
                c2 = default,
                c3 = default
            };
        }


        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSortAlgorithm(int[] arr, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(arr, low, high);

                QuickSortAlgorithm(arr, low, pivotIndex - 1);
                QuickSortAlgorithm(arr, pivotIndex + 1, high);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            int i = low - 1;
            for (int j = low; j <= high - 1; j++)
            {
                if (arr[j] < pivot)
                {
                    i++;
                    Swap(arr, i, j);
                }
            }

            Swap(arr, i + 1, high);
            return i + 1;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(int[] arr, int i, int j)
        {
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        #endregion

        #region Rotation

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEqualUsingDot(float dot) => (double)dot > 0.9999989867210388;

        /// <summary>
        ///   <para>Returns the angle in degrees between two rotations a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(quaternion a, quaternion b)
        {
            float num = math.min(math.abs(math.dot(a, b)), 1f);
            return IsEqualUsingDot(num) ? 0.0f : (float)((double)math.acos(num) * 2.0 * 57.295780181884766);
        }

        /// <summary>
        /// 绕着某个点旋转
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="pos">绝对位置</param>
        /// <param name="angle">弧度</param>
        /// <param name="up"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LocalTransform RotateAround(LocalTransform trans, float3 pos, float angle, float3 up = default)
        {
            if (up.x == default && up.y == default && up.z == default)
            {
                up = new float3(0, 0, 1);
            }

            //pos = pos - trans.Position;
            quaternion q = quaternion.AxisAngle(up, angle);

            trans.Position = math.mul(q, (trans.Position - pos)) + pos;
            trans.Rotation = math.mul(trans.Rotation, q);
            return trans;
        }

        /// <summary>
        /// 向某个方向看2d实现 2d平面为xy轴
        /// </summary>
        /// <param name="targetVector">某个方向</param>
        /// <returns>四元数</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion LookRotation2D(float3 targetVector)
        {
            return quaternion.LookRotation(math.forward(),
                targetVector);
        }


        /// <summary>
        ///   <para>Reflects a vector off the plane defined by a normal.</para>
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Reflect(float3 inDirection, float3 inNormal)
        {
            float num = -2f * math.dot(inNormal, inDirection);
            return new float3(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y,
                num * inNormal.z + inDirection.z);
        }

        /// <summary>
        /// 根据方向算出Quaternion 弃用 新api:LookRotation2D
        /// </summary>
        /// <param name="dir">方向</param>
        /// <returns>quaternion</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion DirToQuan(float3 dir, float3 orginalDir = default)
        {
            var bool3 = orginalDir - float3.zero < math.EPSILON;
            if (bool3.x && bool3.y && bool3.z)
            {
                orginalDir = MathHelper.picForward;
            }

            float needAngel = MathHelper.SignedAngle(math.normalize(dir),
                orginalDir);
            return quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
        }

        /// <summary>
        /// 获得当前四元数的朝向向量
        /// </summary>
        /// <param name="q">四元数</param>
        /// <returns>朝向向量</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Forward(quaternion q)
        {
            return math.normalizesafe(math.mul(q, MathHelper.picForward));
        }


        /// <summary>
        /// 向量旋转
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 RotateVector(float2 dir, float degree)
        {
            // 将角度转换为弧度

            float angleInRadians = math.radians(degree);

            // 计算新的 x 和 y 坐标

            float newX = dir.x * math.cos(angleInRadians) - dir.y * math.sin(angleInRadians);
            float newY = dir.x * math.sin(angleInRadians) + dir.y * math.cos(angleInRadians);


            return math.normalizesafe(new float2(newX, newY));
        }

        /// <summary>
        /// 向量旋转
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RotateVector(float3 dir, float degree)
        {
            // 将角度转换为弧度

            float angleInRadians = math.radians(degree);

            // 计算新的 x 和 y 坐标

            float newX = dir.x * math.cos(angleInRadians) - dir.y * math.sin(angleInRadians);
            float newY = dir.x * math.sin(angleInRadians) + dir.y * math.cos(angleInRadians);


            return math.normalizesafe(new float3(newX, newY, 0));
        }

        #endregion

        #region Vector

        /// <summary>
        /// 拿到两个点连线的任意比例点
        /// </summary>
        /// <param name="startPos">起始点算起</param>
        /// <param name="endPos"></param>
        /// <param name="ratios"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Get2PointRatiosPoint(float3 startPos, float3 endPos, float ratios)
        {
            ratios = math.clamp(ratios, 0f, 1f);
            return (1 - ratios) * startPos + ratios * endPos;
        }

        #endregion

        #region Rec

        public struct Rectangle
        {
            public float2 center; // 矩形的中心点
            public float width; // 矩形的宽度
            public float height; // 矩形的高度

            /// <summary>
            /// 判断某个点是否在矩形内
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public bool Contains(float2 point)
            {
                float minX = center.x - width / 2;
                float maxX = center.x + width / 2;
                float minY = center.y - height / 2;
                float maxY = center.y + height / 2;

                return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
            }
        }

        #endregion

        #region Tween

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EaseOutQuad(float t, float3 b, float3 c, float d)
        {
            var x = t / d; //x值
            var y = -x * x + 2 * x; //y值
            return b + c * y;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EaseInQuad(float t, float3 b, float3 c, float d)
        {
            t = t / d; // 将 t 标准化到 [0, 1] 的范围
            return c * t * t + b; // 使用缓入公式
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInQuad(float t, float b, float c, float d)
        {
            t = t / d; // 将 t 标准化到 [0, 1] 的范围
            return c * t * t + b; // 使用缓入公式
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EaseInOutQuad(float t, float3 b, float3 c, float d)
        {
            t /= (d / 2);
            if (t < 1)
            {
                return (c / 2) * (t * t) + b;
            }

            return (-c / 2) * ((-t) * (t - 2) - 1) + b;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EaseOutInQuad(float t, float3 b, float3 c, float d)
        {
            t /= d / 2;

            if (t < 1)
            {
                return b + c / 2 * (1 - (1 - t) * (1 - t));
            }

            t--;
            return b + c / 2 * (t * t) + c / 2 + (c / 2 * t);
        }

        #endregion
    }
}