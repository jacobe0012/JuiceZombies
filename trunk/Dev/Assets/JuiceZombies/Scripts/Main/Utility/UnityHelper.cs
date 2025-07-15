//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-08-24 12:25:25
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using cfg.blobstruct;
using FMOD.Studio;
using FMODUnity;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Main
{
    public static class UnityHelper
    {
        #region MonsterConst

        //public const float LittleMonsterScale = 6f;
        //public const float BossScale = 10f;

        /// <summary>
        /// 随机掉落范围
        /// </summary>
        public const float DropRadius = 10f;

        #endregion

        #region BulletConst

        public const float OnStayCoolDown = 0.2f;

        #endregion

        #region ColliderConst

        public const int HitBackMaxCount = 2;
        public const int MaxEnemyCount = 1200;

        public const float MaxRefreshEnemyInterval = 60;
        // public const float ShortAttackRange = 20f;
        // public const float LongAttackRange = 60f;

        #endregion

        #region ShaderConst

        public const int LayerCount = 8;
        public const int SortingIndexCount = 10;

        public const string sortingGlobalData = "_sortingGlobalData";
        public const float PerLayerOffset = 1f / LayerCount;
        public const float PerSortingIndexOffset = PerLayerOffset / SortingIndexCount;

        #endregion

        #region Text

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string DeleteEmtypStr(this string str)
        {
            return Regex.Replace(str, @"[\p{C}\s]+", "");
        }

        #endregion

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            var tx = v.x;
            var ty = v.y;
            v.x = cos * tx - sin * ty;
            v.y = sin * tx + cos * ty;
            return v;
        }

        #region Color

        /// <summary>
        /// 普通颜色转hdr颜色
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="intensity">强度值</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color TurnHDRColor(Color baseColor, float intensity)
        {
            return baseColor * Mathf.Pow(2f, intensity);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color HexRGB2Color(string hexRGB)
        {
            Color color;
            string trimmedString = hexRGB.Trim();

            if (trimmedString.Length > 0 && trimmedString[0] != '#')
            {
                hexRGB = $"#{hexRGB}";
            }

            ColorUtility.TryParseHtmlString(hexRGB, out color);
            return color;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Color2HexRGB(Color color)
        {
            int red = (int)math.round(color.r * 255);
            int green = (int)math.round(color.g * 255);
            int blue = (int)math.round(color.b * 255);
            int alpha = (int)math.round(color.a * 255);

            string hex = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", red, green, blue, alpha);
            return hex;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 富文本改变文字大小
        /// </summary>
        /// <param name="input">文本</param>
        /// <param name="size">修改后的文本字体大小</param>
        public static string RichTextSize(string input, float size)
        {
            return "<size=" + size.ToString() + ">" + input + "</size>";
        }


        /// <summary>
        /// 富文本改变文字颜色
        /// </summary>
        /// <param name="input">文本</param>
        /// <param name="hex">修改后字体颜色</param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RichTextColor(string input, string hex)
        {
            string trimmedString = hex.Trim();

            if (trimmedString.Length > 0 && trimmedString[0] == '#')
            {
                input = "<color=" + hex + ">" + input + "</color>";
            }
            else
            {
                input = "<color=#" + hex + ">" + input + "</color>";
            }

            return input;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 富文本改变文字大小和颜色
        /// </summary>
        /// <param name="input">文本</param>
        /// <param name="size">修改后的文本字体大小</param>
        /// <param name="hex">修改后字体颜色</param>
        public static string RichTextSizeAndColor(string input, float size, string hex)
        {
            input = RichTextSize(input, size);
            input = RichTextColor(input, hex);
            return input;
        }

        #endregion

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ParticleSystemLength(Transform transform)
        {
            var pts = transform.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0f;
            foreach (var p in pts)
            {
                if (p.enableEmission)
                {
                    if (p.loop)
                    {
                        return -1f;
                    }

                    float dunration = 0f;
                    if (p.emissionRate <= 0)
                    {
                        dunration = p.startDelay + p.startLifetime;
                    }
                    else
                    {
                        dunration = p.startDelay + math.max(p.duration, p.startLifetime);
                    }

                    if (dunration > maxDuration)
                    {
                        maxDuration = dunration;
                    }
                }
            }

            return maxDuration;
        }

        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="args"></param>
        /// <param name="chaStats"></param>
        /// <param name="playerData"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandleBuffArgs(float3x4 args, ref ChaStats chaStats, ref PlayerData playerData)
        {
            if (args.c0.y == 202000)
            {
                chaStats.chaProperty.maxHp =
                    (int)(chaStats.chaProperty.maxHp * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 202020)
            {
                chaStats.chaProperty.hpRatios =
                    (int)(chaStats.chaProperty.hpRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 202030)
            {
                chaStats.chaProperty.hpAdd =
                    (int)(chaStats.chaProperty.hpAdd * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 202100)
            {
                chaStats.chaProperty.hpRecovery =
                    (int)(chaStats.chaProperty.hpRecovery * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 202120)
            {
                chaStats.chaProperty.hpRecoveryRatios =
                    (int)(chaStats.chaProperty.hpRecoveryRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 202130)
            {
                chaStats.chaProperty.hpRecoveryAdd =
                    (int)(chaStats.chaProperty.hpRecoveryAdd * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 203000)
            {
                chaStats.chaProperty.atk =
                    (int)(chaStats.chaProperty.atk * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 203020)
            {
                chaStats.chaProperty.atkRatios =
                    (int)(chaStats.chaProperty.atkRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 203030)
            {
                chaStats.chaProperty.atkAdd =
                    (int)(chaStats.chaProperty.atkAdd * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 204000)
            {
                chaStats.chaProperty.rebirthCount =
                    (int)(chaStats.chaProperty.rebirthCount * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 204010)
            {
                chaStats.chaProperty.rebirthCount1 =
                    (int)(chaStats.chaProperty.rebirthCount1 * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 205000)
            {
                chaStats.chaProperty.critical =
                    (int)(chaStats.chaProperty.critical * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 205100)
            {
                chaStats.chaProperty.criticalDamageRatios =
                    (int)(chaStats.chaProperty.criticalDamageRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 206120)
            {
                chaStats.chaProperty.damageRatios =
                    (int)(chaStats.chaProperty.damageRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 206130)
            {
                chaStats.chaProperty.damageAdd =
                    (int)(chaStats.chaProperty.damageAdd * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 206220)
            {
                chaStats.chaProperty.reduceHurtRatios =
                    (int)(chaStats.chaProperty.reduceHurtRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 206230)
            {
                chaStats.chaProperty.reduceHurtAdd =
                    (int)(chaStats.chaProperty.reduceHurtAdd * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 207000)
            {
                chaStats.chaProperty.maxMoveSpeed =
                    (int)(chaStats.chaProperty.maxMoveSpeed * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 207020)
            {
                chaStats.chaProperty.maxMoveSpeedRatios =
                    (int)(chaStats.chaProperty.maxMoveSpeedRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 207100)
            {
                chaStats.chaProperty.speedRecoveryTime =
                    (int)(chaStats.chaProperty.speedRecoveryTime * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 208000)
            {
                chaStats.chaProperty.mass =
                    (int)(chaStats.chaProperty.mass * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 208020)
            {
                chaStats.chaProperty.massRatios =
                    (int)(chaStats.chaProperty.massRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 209000)
            {
                chaStats.chaProperty.pushForce =
                    (int)(chaStats.chaProperty.pushForce * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 209020)
            {
                chaStats.chaProperty.pushForceRatios =
                    (int)(chaStats.chaProperty.pushForceRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 210000)
            {
                chaStats.chaProperty.reduceHitBackRatios =
                    (int)(chaStats.chaProperty.reduceHitBackRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            // if (args.c0.y == 218100)
            // {
            //     chaStats.chaProperty.collDamagePlus =
            //         (int)(chaStats.chaProperty.collDamagePlus * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }
            //
            // if (args.c0.y == 218200)
            // {
            //     chaStats.chaProperty.continuousCollDamagePlus =
            //         (int)(chaStats.chaProperty.continuousCollDamagePlus * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }
            //
            // if (args.c0.y == 211000)
            // {
            //     chaStats.chaProperty.dodge =
            //         (int)(chaStats.chaProperty.dodge * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }
            //
            // if (args.c0.y == 212000)
            // {
            //     chaStats.chaProperty.shieldCount =
            //         (int)(chaStats.chaProperty.shieldCount * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }
            //
            // if (args.c0.y == 213000)
            // {
            //     chaStats.chaProperty.coolDown =
            //         (int)(chaStats.chaProperty.coolDown * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }
            //
            // if (args.c0.y == 215000)
            // {
            //     chaStats.chaProperty.skillRangeRatios =
            //         (int)(chaStats.chaProperty.skillRangeRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }


            if (args.c0.y == 201100)
            {
                playerData.playerData.level =
                    (int)(playerData.playerData.level * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201200)
            {
                playerData.playerData.exp =
                    (int)(playerData.playerData.exp * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201220)
            {
                playerData.playerData.expRatios =
                    (int)(playerData.playerData.expRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201300)
            {
                playerData.playerData.gold =
                    (int)(playerData.playerData.gold * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201320)
            {
                playerData.playerData.goldRatios =
                    (int)(playerData.playerData.goldRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201400)
            {
                playerData.playerData.paper =
                    (int)(playerData.playerData.paper * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201420)
            {
                playerData.playerData.paperRatios =
                    (int)(playerData.playerData.paperRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201500)
            {
                playerData.playerData.equip =
                    (int)(playerData.playerData.equip * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201520)
            {
                playerData.playerData.equipRatios =
                    (int)(playerData.playerData.equipRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201700)
            {
                playerData.playerData.killEnemy =
                    (int)(playerData.playerData.killEnemy * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            if (args.c0.y == 201600)
            {
                playerData.playerData.pickUpRadiusRatios =
                    (int)(playerData.playerData.pickUpRadiusRatios * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            }

            // if (args.c0.y == 214000)
            // {
            //     chaStats.chaProperty.skillRefreshCount =
            //         (int)(chaStats.chaProperty.skillRefreshCount * (args.c0.x / 10000f) * (args.c0.z / 10000f));
            // }
        }

        /// <summary>
        /// 输出玩家结算金币，生存时间，杀敌数
        /// </summary>
        /// <param name="playerGold"></param>
        /// <param name="liveTime"></param>
        /// <param name="killEnemies"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OutPlayerGoldAndLiveTime(out GameTimeData gameTimeData, out PlayerData playerData,
            out ChaStats chaStats)
        {
            gameTimeData = default;
            playerData = default;
            chaStats = default;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            var wbeQuery = entityManager.CreateEntityQuery(typeof(GameTimeData));
            if (!entityQuery.IsEmpty)
            {
                playerData = entityQuery.ToComponentDataArray<PlayerData>(Allocator.Temp)[0];

                chaStats = entityQuery.ToComponentDataArray<ChaStats>(Allocator.Temp)[0];
            }

            if (!wbeQuery.IsEmpty)
            {
                gameTimeData = wbeQuery.ToComponentDataArray<GameTimeData>(Allocator.Temp)[0];
                //liveTime = gameTimeData.unScaledTime.elapsedTime;
            }
        }

        #region DamageNumber

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetDamageNumPos(ComponentLookup<LocalTransform> cdfeLocalTransform, Entity caster,
            Entity carrier)
        {
            var casterTrans = cdfeLocalTransform[caster];
            var carrierTrans = cdfeLocalTransform[carrier];
            var damNumDir = math.normalizesafe(casterTrans.Position -
                                               carrierTrans.Position);
            return carrierTrans.Position + 0.5f * carrierTrans.Scale * damNumDir;
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x2 FormatDamage(long damage)
        {
            FixedString128Bytes output = default;
            float4x2 result = new float4x2();
            int renderStrCount = 5;
            // 确定如何分割和编码
            if (damage < 10000)
            {
                // 1-4位数字，直接显示每一位
                FillDigits(ref result, (int)damage, 1);
            }
            else if (damage < 1000000)
            {
                // 5-6位数字，显示为千单位，保留至整数位
                int valueInK = (int)(damage / 1000); // 千单位取整

                FillDigits(ref result, valueInK, 2);
                result.c1.x = 11; // 'k' 表示为 10.0f
            }
            else if (damage < 10000000)
            {
                FillDigits(ref result, 0, 3);
                long millions = damage / 10000;
                long temp = millions;
                var gewei = temp % 10;
                temp /= 10;
                var shiwei = temp % 10; // 十位
                temp /= 10;
                var baiwei = temp % 10; // 十位

                result.c1.x = 12;
                output = $"{baiwei}.{shiwei}{gewei}M";

                result.c0.w = gewei;
                result.c0.z = shiwei;
                result.c0.y = 10;
                result.c0.x = baiwei;
                if (gewei == 0 && shiwei == 0)
                {
                    output = $"{baiwei}M";
                    result.c0.w = baiwei;
                }
                else if (gewei == 0 && shiwei != 0)
                {
                    output = $"{baiwei}.{shiwei}M";

                    result.c0.w = shiwei;
                    result.c0.z = 10;
                    result.c0.y = baiwei;
                }
                else if (gewei != 0 && shiwei == 0)
                {
                    output = $"{baiwei}.{shiwei}{gewei}M";

                    result.c0.w = gewei;
                    result.c0.z = shiwei;
                    result.c0.y = 10;
                    result.c0.x = baiwei;
                }

                //Debug.LogError($"{output}");
                // result.c0.x = integerPart / 10; // 百万位
                // result.c0.y = 12.0f; // '.' 表示为 12.0f
                // result.c0.z = integerPart % 10; // 百万的个位
                // result.c0.w = fractionalPart; // 百万小数第一位
                // result.c1.x = 11.0f; // 'M' 表示为 11.0f
            }
            else if (damage < 100000000)
            {
                FillDigits(ref result, 0, 3);
                long millions = damage / 100000;
                long temp = millions;
                var gewei = temp % 10;
                temp /= 10;
                var shiwei = temp % 10; // 十位
                temp /= 10;
                var baiwei = temp % 10; // 百位


                result.c1.x = 12;
                output = $"{baiwei}{shiwei}.{gewei}M";


                if (gewei == 0)
                {
                    output = $"{baiwei}{shiwei}M";

                    result.c0.w = shiwei;
                    result.c0.z = baiwei;
                }
                else
                {
                    result.c0.w = gewei;
                    result.c0.z = 10;
                    result.c0.y = shiwei;
                    result.c0.x = baiwei;
                }


                //Debug.LogError($"{output}");
            }
            else if (damage < 10000000000)
            {
                FillDigits(ref result, 0, 3);

                if (GetDigitCount(damage) == 9)
                {
                    long millions = damage / 1000000;
                    long temp = millions;
                    var gewei = temp % 10;
                    temp /= 10;
                    var shiwei = temp % 10; // 十位
                    temp /= 10;
                    var baiwei = temp % 10; // 百位
                    temp /= 10;
                    var qianwei = temp % 10; // 百位

                    result.c0.w = gewei;
                    result.c0.z = shiwei;
                    result.c0.y = baiwei;
                    //result.c0.x = qianwei;
                }
                else if (GetDigitCount(damage) == 10)
                {
                    long millions = damage / 1000000;
                    long temp = millions;
                    var gewei = temp % 10;
                    temp /= 10;
                    var shiwei = temp % 10; // 十位
                    temp /= 10;
                    var baiwei = temp % 10; // 百位
                    temp /= 10;
                    var qianwei = temp % 10; // 百位

                    result.c0.w = gewei;
                    result.c0.z = shiwei;
                    result.c0.y = baiwei;
                    result.c0.x = qianwei;
                }


                result.c1.x = 12;
            }
            else
            {
                result.c0.w = 9;
                result.c0.z = 9;
                result.c0.y = 9;
                result.c0.x = 9;
                result.c1.x = 12;
            }

            var tempList = new NativeList<int>(5, Allocator.Temp);

            if (math.abs(result.c0.x - 14) < math.EPSILON)
            {
                renderStrCount--;
            }
            else
            {
                tempList.Add((int)result.c0.x);
            }

            if (math.abs(result.c0.y - 14) < math.EPSILON)
            {
                renderStrCount--;
            }
            else
            {
                tempList.Add((int)result.c0.y);
            }

            if (math.abs(result.c0.z - 15) < math.EPSILON)
            {
                renderStrCount--;
            }
            else
            {
                tempList.Add((int)result.c0.z);
            }

            if (math.abs(result.c0.w - 14) < math.EPSILON)
            {
                renderStrCount--;
            }
            else
            {
                tempList.Add((int)result.c0.w);
            }

            if (math.abs(result.c1.x - 14) < math.EPSILON)
            {
                renderStrCount--;
            }
            else
            {
                tempList.Add((int)result.c1.x);
            }


            if (renderStrCount == 1)
            {
                FillDigits(ref result, 0, 3);
                result.c0.z = tempList[0]; // 一个值，放在 c0.z
            }
            else if (renderStrCount == 2)
            {
                FillDigits(ref result, 0, 3);
                result.c0.y = tempList[0];
                result.c0.z = tempList[1]; // 两个值，放在 c0.y 和 c0.w
            }
            else if (renderStrCount == 3)
            {
                FillDigits(ref result, 0, 3);
                result.c0.y = tempList[0];
                result.c0.z = tempList[1];
                result.c0.w = tempList[2]; // 三个值，放在 c0.y, c0.z, c0.w
            }
            // else if (renderStrCount == 4)
            // {
            //     centeredValues[0].x = tempList[0];
            //     centeredValues[0].y = tempList[1];
            //     centeredValues[0].w = tempList[2];
            //     centeredValues[1].x = tempList[3]; // 四个值，放在 c0.x, c0.y, c0.w, c1.x
            // }

            return result;
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillDigits(ref float4x2 result, long value, int type)
        {
            var digitCount = GetDigitCount(value);
            //当伤害数值为1-4位时，显示完整的伤害数字，如“9638”。
            if (type == 1)
            {
                // 从高位到低位依次提取每一位，并直接填入 result.c0 的相应位置

                result.c1.x = value % 10; // 个位
                value /= 10;
                result.c0.w = value % 10; // 十位
                value /= 10;
                result.c0.z = value % 10; // 百位
                value /= 10;
                result.c0.y = value % 10; // 千位

                result.c0.x = 14; // 万位

                switch (digitCount)
                {
                    case 1:
                        result.c0.w = 14;
                        result.c0.z = 14;
                        result.c0.y = 14;
                        break;
                    case 2:

                        result.c0.z = 14;
                        result.c0.y = 14;
                        break;
                    case 3:

                        result.c0.y = 14;
                        break;
                }

                //Debug.LogError($"{result}");
            }
            //当伤害数值为5-6位时，伤害值/1000，向下取整保留至小数点前第4位，后接k，如“156k”。
            else if (type == 2)
            {
                // 从高位到低位依次提取每一位，并直接填入 result.c0 的相应位置

                result.c0.w = value % 10; // 个位
                value /= 10;
                result.c0.z = value % 10; // 十位
                value /= 10;
                result.c0.y = value % 10; // 百位
                // value /= 10;
                // result.c0.y = value % 10; // 千位

                result.c0.x = 14; // 万位

                switch (digitCount)
                {
                    case 1:
                        //result.c0.w = 15;
                        result.c0.z = 14;
                        result.c0.y = 14;
                        break;
                    case 2:

                        //result.c0.z = 15;
                        result.c0.y = 14;
                        break;
                    // case 3:
                    //
                    //     result.c0.y = 15;
                    //     break;
                }
            }
            else if (type == 3)
            {
                result.c0.x = 14;
                result.c0.y = 14;
                result.c0.z = 14;
                result.c0.w = 14;
                result.c1.x = 14;
            }
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetDigitCount(long number)
        {
            var number0 = math.abs(number);
            if (number0 < 10) return 1;
            if (number0 < 100) return 2;
            if (number0 < 1000) return 3;
            if (number0 < 10000) return 4;
            if (number0 < 100000) return 5;
            if (number0 < 1000000) return 6;
            if (number0 < 10000000) return 7;
            if (number0 < 100000000) return 8;
            if (number0 < 1000000000) return 9;
            if (number0 < int.MaxValue) return 10;
            return 10;
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity HandleDamageType(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey,
            ComponentLookup<ObstacleTag> cdfeObstacleData,
            ComponentLookup<EnemyData> cdfeEnemyData,
            ComponentLookup<PlayerData> cdfePlayerData, ComponentLookup<ChaStats> cdfeChaStats,
            PrefabMapData prefabMapData, Entity player,
            DamageInfo damageInfo, long sumDamage, bool isCritical, float eT, GlobalConfigData config,
            out Entity prefabEntity)
        {
            ref var constants = ref config.value.Value.configTbconstants.configTbconstants;
            int damage_floating_interval_1 = 0;
            int damage_floating_interval_2 = 0;
            int damage_floating_interval_3 = 0;
            for (int i = 0; i < constants.Length; i++)
            {
                if (constants[i].constantName == "damage_floating_interval_1")
                {
                    damage_floating_interval_1 = constants[i].constantValue;
                }

                if (constants[i].constantName == "damage_floating_interval_2")
                {
                    damage_floating_interval_2 = constants[i].constantValue;
                }

                if (constants[i].constantName == "damage_floating_interval_3")
                {
                    damage_floating_interval_3 = constants[i].constantValue;
                }
            }

            Entity damIns = Entity.Null;
            FixedString128Bytes prefatName = (FixedString128Bytes)$"JiYuDamageNumber";
            prefabEntity = Entity.Null;

            float4 color = new float4(255, 255, 255, 255);
            float4 color1 = new float4(255, 255, 255, 255);
            float4 color2 = new float4(255, 255, 255, 255);

            int colorType = 0;

            if (damageInfo.tags.directHeal)
            {
                sumDamage = math.abs(sumDamage);
                colorType = 1;
            }
            else
            {
                if (damageInfo.tags.seckillDamage)
                {
                    prefatName = (FixedString128Bytes)$"JiYuDamageNumber3";
                    prefabEntity = prefabMapData.prefabHashMap[prefatName];

                    damIns = ecb.Instantiate(sortKey, prefabEntity);
                    return damIns;
                }
                else if (isCritical)
                {
                    prefatName = (FixedString128Bytes)$"JiYuDamageNumber1";
                }

                if (cdfePlayerData.HasComponent(damageInfo.defender))
                {
                    colorType = 2;
                }
                else if (cdfeObstacleData.HasComponent(damageInfo.defender))
                {
                    colorType = 10;
                }
                else if (cdfeEnemyData.HasComponent(damageInfo.defender))
                {
                    int atk = cdfeChaStats[player].chaProperty.atk;
                    var ratios = (float)sumDamage / atk;

                    if (ratios >= 0 && ratios < damage_floating_interval_1 / 10000f)
                    {
                        colorType = 3;
                    }
                    else if (ratios >= damage_floating_interval_1 / 10000f &&
                             ratios < damage_floating_interval_2 / 10000f)
                    {
                        colorType = 4;
                    }
                    else if (ratios >= damage_floating_interval_2 / 10000f &&
                             ratios < damage_floating_interval_3 / 10000f)
                    {
                        colorType = 5;
                    }
                    else if (ratios >= damage_floating_interval_3 / 10000f)
                    {
                        colorType = 6;
                    }
                }
            }


            prefabEntity = prefabMapData.prefabHashMap[prefatName];

            damIns = ecb.Instantiate(sortKey, prefabEntity);

            switch (colorType)
            {
                case 1:

                    color1 = new float4(196, 229, 63, 255);
                    break;
                case 2:

                    color1 = new float4(255, 77, 101, 255);
                    break;
                case 3:

                    color1 = new float4(203, 202, 233, 255);
                    break;
                case 4:

                    color1 = new float4(254, 216, 93, 255);
                    break;
                case 5:

                    color1 = new float4(254, 130, 92, 255);
                    break;
                case 6:

                    color1 = new float4(255, 77, 101, 255);
                    break;
                case 10:

                    color1 = new float4(203, 202, 233, 255);
                    break;
            }

            color = math.remap(new float4(0, 0, 0, 0), new float4(255, 255, 255, 255), float4.zero,
                new float4(1, 1, 1, 1),
                color);
            color1 = math.remap(new float4(0, 0, 0, 0), new float4(255, 255, 255, 255), float4.zero,
                new float4(1, 1, 1, 1),
                color1);
            color2 = math.remap(new float4(0, 0, 0, 0), new float4(255, 255, 255, 255), float4.zero,
                new float4(1, 1, 1, 1),
                color2);

            //ecb.SetComponent(sortKey, damIns, new JiYuFrontColor() { value = color });
            ecb.SetComponent(sortKey, damIns, new JiYuFrontColor1() { value = color1 });
            //ecb.SetComponent(sortKey, damIns, new JiYuFrontColor2() { value = color2 });
            return damIns;
        }

        #endregion


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopTime()
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BeginTime()
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableTime(bool enable)
        {
            if (enable)
            {
                BeginTime();
            }
            else
            {
                StopTime();
            }
        }

        /// <summary>
        /// float转 分钟：秒数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToTimeFormat(float time)
        {
            //秒数取整
            int seconds = (int)time;
            //一小时为3600秒 秒数对3600取整即为小时
            int hour = seconds / 3600;
            //一分钟为60秒 秒数对3600取余再对60取整即为分钟
            int minute = seconds % 3600 / 60;
            //对3600取余再对60取余即为秒数
            seconds = seconds % 3600 % 60;
            //返回00:00:00时间格式
            return string.Format("{0:D2}:{1:D2}", minute, seconds);
        }


        /// <summary>
        /// float转小时分钟秒
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="seconds"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OutTime(float time, out int hour, out int minute, out int seconds)
        {
            //秒数取整
            seconds = (int)time;
            //一小时为3600秒 秒数对3600取整即为小时
            hour = seconds / 3600;
            //一分钟为60秒 秒数对3600取余再对60取整即为分钟
            minute = seconds % 3600 / 60;
            //对3600取余再对60取余即为秒数
            seconds = seconds % 3600 % 60;
            //返回00:00:00时间格式
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitProperty(ref PlayerData playerData, ref ChaStats chaStats, int id, float propertyValue)
        {
            var value = (int)propertyValue;
            if (id == 202000)
            {
                chaStats.chaProperty.maxHp = value;
                chaStats.chaProperty.defaultMaxHp = value;
            }


            if (id == 202010)
            {
                chaStats.chaProperty.defaultMaxHp = value;
            }


            if (id == 202020)
            {
                chaStats.chaProperty.hpRatios = value;
            }


            if (id == 202030)
            {
                chaStats.chaProperty.hpAdd = value;
            }


            if (id == 202040)
            {
                chaStats.chaProperty.curHpRatios = value;
            }


            if (id == 202100)
            {
                chaStats.chaProperty.hpRecovery = value;
                chaStats.chaProperty.defaultHpRecovery = value;
            }


            if (id == 202110)
            {
                chaStats.chaProperty.defaultHpRecovery = value;
            }


            if (id == 202120)
            {
                chaStats.chaProperty.hpRecoveryRatios = value;
            }


            if (id == 202130)
            {
                chaStats.chaProperty.hpRecoveryAdd = value;
            }


            if (id == 203000)
            {
                chaStats.chaProperty.atk = value;
                chaStats.chaProperty.defaultAtk = value;
            }


            if (id == 203010)
            {
                chaStats.chaProperty.defaultAtk = value;
            }


            if (id == 203020)
            {
                chaStats.chaProperty.atkRatios = value;
            }


            if (id == 203030)
            {
                chaStats.chaProperty.atkAdd = value;
            }


            if (id == 204000)
            {
                chaStats.chaProperty.rebirthCount = value;
            }

            if (id == 204010)
            {
                chaStats.chaProperty.rebirthCount1 = value;
            }

            if (id == 205000)
            {
                chaStats.chaProperty.critical = value;
            }


            if (id == 205010)
            {
                chaStats.chaProperty.tmpCritical = value;
            }


            if (id == 205100)
            {
                chaStats.chaProperty.criticalDamageRatios = value;
            }


            if (id == 206120)
            {
                chaStats.chaProperty.damageRatios = value;
            }


            if (id == 206130)
            {
                chaStats.chaProperty.damageAdd = value;
            }


            if (id == 206220)
            {
                chaStats.chaProperty.reduceHurtRatios = value;
            }


            if (id == 206230)
            {
                chaStats.chaProperty.reduceHurtAdd = value;
            }


            if (id == 206240)
            {
                chaStats.chaProperty.reduceBulletRatios = value;
            }


            if (id == 206250)
            {
                chaStats.chaProperty.changedFromPlayerDamage = value;
            }


            if (id == 207000)
            {
                chaStats.chaProperty.maxMoveSpeed = value;
                chaStats.chaProperty.defaultMaxMoveSpeed = value;
            }


            if (id == 207010)
            {
                chaStats.chaProperty.defaultMaxMoveSpeed = value;
            }


            if (id == 207020)
            {
                chaStats.chaProperty.maxMoveSpeedRatios = value;
            }


            if (id == 207030)
            {
                chaStats.chaProperty.maxMoveSpeedAdd = value;
            }


            if (id == 207100)
            {
                chaStats.chaProperty.speedRecoveryTime = value;
            }


            if (id == 208000)
            {
                chaStats.chaProperty.mass = value;
                chaStats.chaProperty.defaultMass = value;
            }


            if (id == 208010)
            {
                chaStats.chaProperty.defaultMass = value;
            }


            if (id == 208020)
            {
                chaStats.chaProperty.massRatios = value;
            }


            if (id == 209000)
            {
                chaStats.chaProperty.pushForce = value;
                chaStats.chaProperty.defaultPushForce = value;
            }


            if (id == 209010)
            {
                chaStats.chaProperty.defaultPushForce = value;
            }


            if (id == 209020)
            {
                chaStats.chaProperty.pushForceRatios = value;
            }


            if (id == 209030)
            {
                chaStats.chaProperty.pushForceAdd = value;
            }


            if (id == 210000)
            {
                chaStats.chaProperty.reduceHitBackRatios = value;
            }


            if (id == 211000)
            {
                chaStats.chaProperty.dodge = value;
            }


            if (id == 212000)
            {
                chaStats.chaProperty.shieldCount = value;
            }


            if (id == 213000)
            {
                chaStats.chaProperty.defaultcoolDown = value;
            }


            if (id == 215000)
            {
                chaStats.chaProperty.defaultBulletRangeRatios = value;
            }


            if (id == 218100)
            {
                chaStats.chaProperty.collideDamageRatios = value;
            }


            if (id == 218200)
            {
                chaStats.chaProperty.continuousCollideDamageRatios = value;
            }

            if (id == 219100)
            {
                playerData.playerData.superPushForceChance = value;
            }


            if (id == 219200)
            {
                playerData.playerData.maxPushForceChance = value;
            }


            if (id == 222100)
            {
                chaStats.chaProperty.scaleRatios = value;
            }


            if (id == 201100)
            {
                playerData.playerData.level = value;
            }


            if (id == 201200)
            {
                playerData.playerData.exp = value;
            }


            if (id == 201220)
            {
                playerData.playerData.expRatios = value;
            }


            if (id == 201300)
            {
                playerData.playerData.gold = value;
            }


            if (id == 201320)
            {
                playerData.playerData.goldRatios = value;
            }


            if (id == 201400)
            {
                playerData.playerData.paper = value;
            }


            if (id == 201420)
            {
                playerData.playerData.paperRatios = value;
            }


            if (id == 201500)
            {
                playerData.playerData.equip = value;
            }


            if (id == 201520)
            {
                playerData.playerData.equipRatios = value;
            }


            if (id == 201600)
            {
                playerData.playerData.pickUpRadiusRatios = value;
            }


            if (id == 201700)
            {
                playerData.playerData.killEnemy = value;
            }


            if (id == 202140)
            {
                playerData.playerData.propsRecoveryRatios = value;
            }


            if (id == 202150)
            {
                playerData.playerData.propsRecoveryAdd = value;
            }


            if (id == 214000)
            {
                playerData.playerData.skillFreeBuyCount = value;
            }


            if (id == 214100)
            {
                playerData.playerData.buySkillRatios = value;
            }


            if (id == 214200)
            {
                playerData.playerData.refreshShopRatios = value;
            }


            if (id == 214300)
            {
                playerData.playerData.skillRefreshCount = value;
            }

            if (id == 214700)
            {
                playerData.playerData.skillTempRefreshCount = value;
            }

            if (id == 214400)
            {
                playerData.playerData.skillWeightIncrease1 = value;
            }


            if (id == 214500)
            {
                playerData.playerData.skillWeightIncrease2 = value;
            }


            if (id == 214600)
            {
                playerData.playerData.skillWeightIncrease3 = value;
            }

            if (id == 220100)
            {
                playerData.playerData.normalMonsterDamageRatios = value;
            }


            if (id == 220200)
            {
                playerData.playerData.specialMonsterDamageRatios = value;
            }


            if (id == 220300)
            {
                playerData.playerData.bossMonsterDamageRatios = value;
            }


            if (id == 221100)
            {
                playerData.playerData.weaponSkillExtraCount = value;
            }
        }

        /// <summary>
        /// 拿属性 只读
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="chaStats"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetProperty(PlayerData playerData, ChaStats chaStats, int id)
        {
            if (id == 202000)
                return chaStats.chaProperty.maxHp;

            if (id == 202010)
                return chaStats.chaProperty.defaultMaxHp;

            if (id == 202020)
                return chaStats.chaProperty.hpRatios;

            if (id == 202030)
                return chaStats.chaProperty.hpAdd;

            if (id == 202040)
                return chaStats.chaProperty.curHpRatios;

            if (id == 202100)
                return chaStats.chaProperty.hpRecovery;

            if (id == 202110)
                return chaStats.chaProperty.defaultHpRecovery;

            if (id == 202120)
                return chaStats.chaProperty.hpRecoveryRatios;

            if (id == 202130)
                return chaStats.chaProperty.hpRecoveryAdd;

            if (id == 203000)
                return chaStats.chaProperty.atk;

            if (id == 203010)
                return chaStats.chaProperty.defaultAtk;

            if (id == 203020)
                return chaStats.chaProperty.atkRatios;

            if (id == 203030)
                return chaStats.chaProperty.atkAdd;

            if (id == 204000)
                return chaStats.chaProperty.rebirthCount;

            if (id == 204010)
                return chaStats.chaProperty.rebirthCount1;

            if (id == 205000)
                return chaStats.chaProperty.critical;

            if (id == 205010)
                return chaStats.chaProperty.tmpCritical;

            if (id == 205100)
                return chaStats.chaProperty.criticalDamageRatios;

            if (id == 206120)
                return chaStats.chaProperty.damageRatios;

            if (id == 206130)
                return chaStats.chaProperty.damageAdd;

            if (id == 206220)
                return chaStats.chaProperty.reduceHurtRatios;

            if (id == 206230)
                return chaStats.chaProperty.reduceHurtAdd;

            if (id == 206240)
                return chaStats.chaProperty.reduceBulletRatios;

            if (id == 206250)
                return chaStats.chaProperty.changedFromPlayerDamage;

            if (id == 207000)
                return chaStats.chaProperty.maxMoveSpeed;

            if (id == 207010)
                return chaStats.chaProperty.defaultMaxMoveSpeed;

            if (id == 207020)
                return chaStats.chaProperty.maxMoveSpeedRatios;

            if (id == 207030)
                return chaStats.chaProperty.maxMoveSpeedAdd;

            if (id == 207100)
                return chaStats.chaProperty.speedRecoveryTime;

            if (id == 208000)
                return chaStats.chaProperty.mass;

            if (id == 208010)
                return chaStats.chaProperty.defaultMass;

            if (id == 208020)
                return chaStats.chaProperty.massRatios;

            if (id == 209000)
                return chaStats.chaProperty.pushForce;

            if (id == 209010)
                return chaStats.chaProperty.defaultPushForce;

            if (id == 209020)
                return chaStats.chaProperty.pushForceRatios;

            if (id == 209030)
                return chaStats.chaProperty.pushForceAdd;

            if (id == 210000)
                return chaStats.chaProperty.reduceHitBackRatios;

            if (id == 211000)
                return chaStats.chaProperty.dodge;

            if (id == 212000)
                return chaStats.chaProperty.shieldCount;

            if (id == 213000)
                return chaStats.chaProperty.defaultcoolDown;

            if (id == 215000)
                return chaStats.chaProperty.defaultBulletRangeRatios;

            if (id == 218100)
                return chaStats.chaProperty.collideDamageRatios;

            if (id == 218200)
                return chaStats.chaProperty.continuousCollideDamageRatios;

            if (id == 219100)
                return playerData.playerData.superPushForceChance;

            if (id == 219200)
                return playerData.playerData.maxPushForceChance;

            if (id == 222100)
                return chaStats.chaProperty.scaleRatios;


            if (id == 201100)
                return playerData.playerData.level;


            if (id == 201200)
                return playerData.playerData.exp;


            if (id == 201220)
                return playerData.playerData.expRatios;


            if (id == 201300)
                return playerData.playerData.gold;


            if (id == 201320)
                return playerData.playerData.goldRatios;


            if (id == 201400)
                return playerData.playerData.paper;


            if (id == 201420)
                return playerData.playerData.paperRatios;


            if (id == 201500)
                return playerData.playerData.equip;


            if (id == 201520)
                return playerData.playerData.equipRatios;


            if (id == 201600)
                return playerData.playerData.pickUpRadiusRatios;


            if (id == 201700)
                return playerData.playerData.killEnemy;


            if (id == 202140)
                return playerData.playerData.propsRecoveryRatios;


            if (id == 202150)
                return playerData.playerData.propsRecoveryAdd;


            if (id == 214000)
                return playerData.playerData.skillFreeBuyCount;


            if (id == 214100)
                return playerData.playerData.buySkillRatios;


            if (id == 214200)
                return playerData.playerData.refreshShopRatios;


            if (id == 214300)
                return playerData.playerData.skillRefreshCount;
            if (id == 214700)
                return playerData.playerData.skillTempRefreshCount;
            if (id == 214400)
                return playerData.playerData.skillWeightIncrease1;

            if (id == 214500)
                return playerData.playerData.skillWeightIncrease2;

            if (id == 214600)
                return playerData.playerData.skillWeightIncrease3;

            if (id == 220100)
                return playerData.playerData.normalMonsterDamageRatios;


            if (id == 220200)
                return playerData.playerData.specialMonsterDamageRatios;


            if (id == 220300)
                return playerData.playerData.bossMonsterDamageRatios;


            if (id == 221100)
                return playerData.playerData.weaponSkillExtraCount;
            return default;
        }

        /// <summary>
        /// 改属性 加等
        /// </summary>
        /// <param name="chaStats"></param>
        /// <param name="playerData"></param>
        /// <param name="id"></param>
        /// <param name="propertyValue"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeProperty(ref ChaStats chaStats, ref PlayerData playerData, int id, float propertyValue)
        {
            var value = (int)propertyValue;
            if (id == 202000)
            {
                chaStats.chaProperty.maxHp += value;
            }


            if (id == 202010)
            {
                chaStats.chaProperty.defaultMaxHp += value;
            }


            if (id == 202020)
            {
                chaStats.chaProperty.hpRatios += value;
            }


            if (id == 202030)
            {
                chaStats.chaProperty.hpAdd += value;
            }


            if (id == 202040)
            {
                chaStats.chaProperty.curHpRatios += value;
            }


            if (id == 202100)
            {
                chaStats.chaProperty.hpRecovery += value;
            }


            if (id == 202110)
            {
                chaStats.chaProperty.defaultHpRecovery += value;
            }


            if (id == 202120)
            {
                chaStats.chaProperty.hpRecoveryRatios += value;
            }


            if (id == 202130)
            {
                chaStats.chaProperty.hpRecoveryAdd += value;
            }


            if (id == 203000)
            {
                chaStats.chaProperty.atk += value;
            }


            if (id == 203010)
            {
                chaStats.chaProperty.defaultAtk += value;
            }


            if (id == 203020)
            {
                chaStats.chaProperty.atkRatios += value;
            }


            if (id == 203030)
            {
                chaStats.chaProperty.atkAdd += value;
            }


            if (id == 204000)
            {
                chaStats.chaProperty.rebirthCount += value;
            }

            if (id == 204010)
            {
                chaStats.chaProperty.rebirthCount1 += value;
            }

            if (id == 205000)
            {
                chaStats.chaProperty.critical += value;
            }


            if (id == 205010)
            {
                chaStats.chaProperty.tmpCritical += value;
            }


            if (id == 205100)
            {
                chaStats.chaProperty.criticalDamageRatios += value;
            }


            if (id == 206120)
            {
                chaStats.chaProperty.damageRatios += value;
            }


            if (id == 206130)
            {
                chaStats.chaProperty.damageAdd += value;
            }


            if (id == 206220)
            {
                chaStats.chaProperty.reduceHurtRatios += value;
            }


            if (id == 206230)
            {
                chaStats.chaProperty.reduceHurtAdd += value;
            }


            if (id == 206240)
            {
                chaStats.chaProperty.reduceBulletRatios += value;
            }


            if (id == 206250)
            {
                chaStats.chaProperty.changedFromPlayerDamage += value;
            }


            if (id == 207000)
            {
                chaStats.chaProperty.maxMoveSpeed += value;
            }


            if (id == 207010)
            {
                chaStats.chaProperty.defaultMaxMoveSpeed += value;
            }


            if (id == 207020)
            {
                chaStats.chaProperty.maxMoveSpeedRatios += value;
            }


            if (id == 207030)
            {
                chaStats.chaProperty.maxMoveSpeedAdd += value;
            }


            if (id == 207100)
            {
                chaStats.chaProperty.speedRecoveryTime += value;
            }


            if (id == 208000)
            {
                chaStats.chaProperty.mass += value;
            }


            if (id == 208010)
            {
                chaStats.chaProperty.defaultMass += value;
            }


            if (id == 208020)
            {
                chaStats.chaProperty.massRatios += value;
            }


            if (id == 209000)
            {
                chaStats.chaProperty.pushForce += value;
            }


            if (id == 209010)
            {
                chaStats.chaProperty.defaultPushForce += value;
            }


            if (id == 209020)
            {
                chaStats.chaProperty.pushForceRatios += value;
            }


            if (id == 209030)
            {
                chaStats.chaProperty.pushForceAdd += value;
            }


            if (id == 210000)
            {
                chaStats.chaProperty.reduceHitBackRatios += value;
            }


            if (id == 211000)
            {
                chaStats.chaProperty.dodge += value;
            }


            if (id == 212000)
            {
                chaStats.chaProperty.shieldCount += value;
            }


            if (id == 213000)
            {
                chaStats.chaProperty.defaultcoolDown += value;
            }


            if (id == 215000)
            {
                chaStats.chaProperty.defaultBulletRangeRatios += value;
            }


            if (id == 218100)
            {
                chaStats.chaProperty.collideDamageRatios += value;
            }


            if (id == 218200)
            {
                chaStats.chaProperty.continuousCollideDamageRatios += value;
            }

            if (id == 219100)
            {
                playerData.playerData.superPushForceChance += value;
            }


            if (id == 219200)
            {
                playerData.playerData.maxPushForceChance += value;
            }

            if (id == 222100)
            {
                chaStats.chaProperty.scaleRatios += value;
            }


            if (id == 201100)
            {
                playerData.playerData.level += value;
            }


            if (id == 201200)
            {
                playerData.playerData.exp += value;
            }


            if (id == 201220)
            {
                playerData.playerData.expRatios += value;
            }


            if (id == 201300)
            {
                playerData.playerData.gold += value;
            }


            if (id == 201320)
            {
                playerData.playerData.goldRatios += value;
            }


            if (id == 201400)
            {
                playerData.playerData.paper += value;
            }


            if (id == 201420)
            {
                playerData.playerData.paperRatios += value;
            }


            if (id == 201500)
            {
                playerData.playerData.equip += value;
            }


            if (id == 201520)
            {
                playerData.playerData.equipRatios += value;
            }


            if (id == 201600)
            {
                playerData.playerData.pickUpRadiusRatios += value;
            }


            if (id == 201700)
            {
                playerData.playerData.killEnemy += value;
            }


            if (id == 202140)
            {
                playerData.playerData.propsRecoveryRatios += value;
            }


            if (id == 202150)
            {
                playerData.playerData.propsRecoveryAdd += value;
            }


            if (id == 214000)
            {
                playerData.playerData.skillFreeBuyCount += value;
            }


            if (id == 214100)
            {
                playerData.playerData.buySkillRatios += value;
            }


            if (id == 214200)
            {
                playerData.playerData.refreshShopRatios += value;
            }


            if (id == 214300)
            {
                playerData.playerData.skillRefreshCount += value;
            }

            if (id == 214700)
            {
                playerData.playerData.skillTempRefreshCount += value;
            }

            if (id == 214400)
            {
                playerData.playerData.skillWeightIncrease1 += value;
            }


            if (id == 214500)
            {
                playerData.playerData.skillWeightIncrease2 += value;
            }


            if (id == 214600)
            {
                playerData.playerData.skillWeightIncrease3 += value;
            }

            if (id == 220100)
            {
                playerData.playerData.normalMonsterDamageRatios += value;
            }


            if (id == 220200)
            {
                playerData.playerData.specialMonsterDamageRatios += value;
            }


            if (id == 220300)
            {
                playerData.playerData.bossMonsterDamageRatios += value;
            }


            if (id == 221100)
            {
                playerData.playerData.weaponSkillExtraCount += value;
            }

            chaStats.chaProperty.atk =
                (int)(chaStats.chaProperty.defaultAtk * (1 + chaStats.chaProperty.atkRatios / 10000f) +
                      chaStats.chaProperty.atkAdd);

            var oldMaxHp = chaStats.chaProperty.maxHp;
            chaStats.chaProperty.maxHp =
                (int)(chaStats.chaProperty.defaultMaxHp * (1 + chaStats.chaProperty.hpRatios / 10000f) +
                      chaStats.chaProperty.hpAdd);
            chaStats.chaResource.hp += (chaStats.chaProperty.maxHp - oldMaxHp);
            chaStats.chaResource.hp = math.min(chaStats.chaProperty.maxHp, chaStats.chaResource.hp);

            chaStats.chaProperty.hpRecovery =
                (int)(chaStats.chaProperty.defaultHpRecovery * (1 + chaStats.chaProperty.hpRecoveryRatios / 10000f) +
                      chaStats.chaProperty.hpRecoveryAdd);
            chaStats.chaProperty.maxMoveSpeed =
                (int)(chaStats.chaProperty.defaultMaxMoveSpeed *
                      (1 + chaStats.chaProperty.maxMoveSpeedRatios / 10000f) +
                      chaStats.chaProperty.maxMoveSpeedAdd);

            chaStats.chaProperty.pushForce =
                (int)(chaStats.chaProperty.defaultPushForce *
                      (1 + chaStats.chaProperty.pushForceRatios / 10000f) +
                      chaStats.chaProperty.pushForceAdd);

            chaStats.chaProperty.mass =
                (int)(chaStats.chaProperty.defaultMass *
                      (1 + chaStats.chaProperty.massRatios / 10000f));
        }

        public static void RemoveChangeProperty(ref ChaStats chaStats, ref PlayerData playerData, int id,
            float propertyValue)
        {
            ChangeProperty(ref chaStats, ref playerData, id, -propertyValue);
        }

        /// <summary>
        /// 改属性 加等
        /// </summary>
        /// <param name="cdfechaStats"></param>
        /// <param name="cdfeplayerData"></param>
        /// <param name="id"></param>
        /// <param name="propertyValue"></param>
        /// <param name="entity"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeProperty(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey,
            ref ComponentLookup<ChaStats> cdfeChaStats,
            ref ComponentLookup<PlayerData> cdfePlayerData,
            ref ComponentLookup<PhysicsMass> cdfePhysicsMass,
            ComponentLookup<LocalTransform> cdfeLocalTransform, int id,
            float propertyValue, Entity entity)
        {
            if (id == 0) return;
            var oldChaStats = cdfeChaStats[entity];
            var playerData = new PlayerData { };
            var chaStates = new ChaStats { };

            if (cdfePlayerData.HasComponent(entity))
            {
                playerData = cdfePlayerData[entity];
            }

            if (cdfeChaStats.HasComponent(entity))
            {
                chaStates = cdfeChaStats[entity];
            }


            ChangeProperty(ref chaStates, ref playerData, id, propertyValue);

            if (cdfePlayerData.HasComponent(entity))
            {
                cdfePlayerData[entity] = playerData;
            }

            if (cdfeChaStats.HasComponent(entity))
            {
                cdfeChaStats[entity] = chaStates;
            }

            if (cdfePhysicsMass.HasComponent(entity) && !cdfePlayerData.HasComponent(entity))
            {
                var physicsMasses = cdfePhysicsMass[entity];
                physicsMasses.InverseMass = 1f / chaStates.chaProperty.mass;
                cdfePhysicsMass[entity] = physicsMasses;
            }

            var tran = cdfeLocalTransform[entity];
            if (oldChaStats.chaProperty.scaleRatios != chaStates.chaProperty.scaleRatios)
            {
                ecb.AddComponent(sortKey, entity, new PushColliderData
                {
                    toBeSmall = chaStates.chaProperty.scaleRatios < oldChaStats.chaProperty.scaleRatios,
                    initScale = tran.Scale,
                    targetScale = tran.Scale / (oldChaStats.chaProperty.scaleRatios / 10000f) *
                                  (chaStates.chaProperty.scaleRatios / 10000f)
                });
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveChangeProperty(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey,
            ref ComponentLookup<ChaStats> cdfeChaStats,
            ref ComponentLookup<PlayerData> cdfePlayerData,
            ref ComponentLookup<PhysicsMass> cdfePhysicsMass,
            ComponentLookup<LocalTransform> cdfeLocalTransform, int id,
            float propertyValue, Entity entity)
        {
            if (id == 0) return;
            var oldChaStats = cdfeChaStats[entity];
            var playerData = new PlayerData { };
            var chaStates = new ChaStats { };

            if (cdfePlayerData.HasComponent(entity))
            {
                playerData = cdfePlayerData[entity];
            }

            if (cdfeChaStats.HasComponent(entity))
            {
                chaStates = cdfeChaStats[entity];
            }


            RemoveChangeProperty(ref chaStates, ref playerData, id, propertyValue);

            if (cdfePlayerData.HasComponent(entity))
            {
                cdfePlayerData[entity] = playerData;
            }

            if (cdfeChaStats.HasComponent(entity))
            {
                cdfeChaStats[entity] = chaStates;
            }

            if (cdfePhysicsMass.HasComponent(entity) && !cdfePlayerData.HasComponent(entity))
            {
                var physicsMasses = cdfePhysicsMass[entity];
                physicsMasses.InverseMass = 1f / chaStates.chaProperty.mass;
                cdfePhysicsMass[entity] = physicsMasses;
            }


            var tran = cdfeLocalTransform[entity];
            if (oldChaStats.chaProperty.scaleRatios != chaStates.chaProperty.scaleRatios)
            {
                Debug.Log(
                    $"oldChaStats{oldChaStats.chaProperty.scaleRatios}  scaleRatios{chaStates.chaProperty.scaleRatios}");


                ecb.AddComponent(sortKey, entity, new PushColliderData
                {
                    toBeSmall = chaStates.chaProperty.scaleRatios < oldChaStats.chaProperty.scaleRatios,
                    initScale = tran.Scale,
                    targetScale = tran.Scale / (oldChaStats.chaProperty.scaleRatios / 10000f) *
                                  (chaStates.chaProperty.scaleRatios / 10000f)
                });
            }
        }


        /// <summary>
        /// 黄金国修改  已弃用
        /// 用于通过ID读取属性参数
        /// </summary>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPlayerProperty(PlayerData playerData, ChaProperty chaProperty, int propertyId)
        {
            // if (propertyId == 101)
            // {
            //     return playerData.level;
            // }
            //
            // if (propertyId == 102)
            // {
            //     return (int)playerData.exp;
            // }
            //
            // if (propertyId == 103)
            // {
            //     return playerData.expRatios;
            // }
            //
            //
            // if (propertyId == 201)
            // {
            //     return chaProperty.maxHp;
            // }
            //
            // if (propertyId == 202)
            // {
            //     return chaResource.hp;
            // }
            //
            // if (propertyId == 203)
            // {
            //     return chaResource.hpRecoveryPerSecond;
            // }
            //
            // if (propertyId == 204)
            // {
            //     return chaResource.hpRecoveryPlusFixed;
            // }
            //
            // if (propertyId == 205)
            // {
            //     return chaResource.hpRecoveryPlusRatios;
            // }
            //
            // if (propertyId == 206)
            // {
            //     return chaProperty.rebirthCount;
            // }
            //
            //
            // if (propertyId == 301)
            // {
            //     return chaProperty.atk;
            // }
            //
            // if (propertyId == 302)
            // {
            //     return chaProperty.atkPlus;
            // }
            //
            // if (propertyId == 303)
            // {
            //     return chaProperty.critical;
            // }
            //
            // if (propertyId == 304)
            // {
            //     return chaProperty.criticalDamageAdd;
            // }
            //
            // if (propertyId == 305)
            // {
            //     return chaProperty.damagePlus;
            // }
            //
            // if (propertyId == 306)
            // {
            //     return chaProperty.collDamagePlus;
            // }
            //
            // if (propertyId == 307)
            // {
            //     return chaProperty.continuousCollDamagePlus;
            // }
            //
            // if (propertyId == 308)
            // {
            //     return chaProperty.damageAdd;
            // }
            //
            // if (propertyId == 401)
            // {
            //     return chaProperty.pushForce;
            // }
            //
            // if (propertyId == 402)
            // {
            //     return chaProperty.maxMoveSpeed;
            // }
            //
            // if (propertyId == 403)
            // {
            //     return chaProperty.acceleration;
            // }
            //
            // if (propertyId == 404)
            // {
            //     return chaProperty.maxMoveSpeedPlus;
            // }
            //
            // if (propertyId == 405)
            // {
            //     return chaProperty.basicMovementSpeed;
            // }
            //
            // if (propertyId == 406)
            // {
            //     return chaProperty.basicAcceleration;
            // }
            //
            // if (propertyId == 407)
            // {
            //     return chaProperty.stallAcceleration;
            // }
            //
            // if (propertyId == 408)
            // {
            //     return chaProperty.mass;
            // }
            //
            // if (propertyId == 409)
            // {
            //     return chaProperty.pushForcePlus;
            // }
            //
            // if (propertyId == 410)
            // {
            //     return chaProperty.basicRecoveryTime;
            // }
            //
            // if (propertyId == 411)
            // {
            //     return chaProperty.speedRecoveryTime;
            // }
            //
            //
            // if (propertyId == 501)
            // {
            //     return chaProperty.reduceHurtRatios;
            // }
            //
            // if (propertyId == 502)
            // {
            //     return chaProperty.reduceHurt;
            // }
            //
            // if (propertyId == 503)
            // {
            //     return chaProperty.reduceHitBackRatios;
            // }
            //
            // if (propertyId == 504)
            // {
            //     return chaProperty.reduceHitBack;
            // }
            //
            // if (propertyId == 505)
            // {
            //     return chaProperty.maxReduceHitBack;
            // }
            //
            // if (propertyId == 506)
            // {
            //     return chaProperty.reduceHitBackRecoveryTime;
            // }
            //
            // if (propertyId == 507)
            // {
            //     return chaProperty.reduceBulletHurtRatios;
            // }
            //
            // if (propertyId == 508)
            // {
            //     return chaProperty.reduceHitBackDamageRatios;
            // }
            //
            // if (propertyId == 509)
            // {
            //     return chaProperty.dodge;
            // }
            //
            // if (propertyId == 510)
            // {
            //     return chaProperty.recoveryTime;
            // }
            //
            // if (propertyId == 601)
            // {
            //     return chaProperty.coolDown;
            // }
            //
            // if (propertyId == 602)
            // {
            //     return chaProperty.skillRangeRatios;
            // }
            //
            // if (propertyId == 701)
            // {
            //     return playerData.gold;
            // }
            //
            // if (propertyId == 702)
            // {
            //     return playerData.goldRatios;
            // }
            //
            // if (propertyId == 703)
            // {
            //     return playerData.paper;
            // }
            //
            // if (propertyId == 704)
            // {
            //     return playerData.paperRatios;
            // }
            //
            // if (propertyId == 705)
            // {
            //     return playerData.killEnemy;
            // }
            //
            // if (propertyId == 706)
            // {
            //     return playerData.propsRangeRatios;
            // }

            return propertyId;
        }

        /// <summary>
        /// 迅捷蟹修改
        /// 用于Reward str转实际的Vector3
        /// </summary>
        /// <param name="str">Reward str</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 StrToVector3(string str)
        {
            Vector3 reslut = new Vector3();
            if (str == "" || !str.Contains(";"))
            {
                Debug.LogError(string.Format("后端字符串有误,返回的字符串为:{0}", str));
                return reslut;
            }

            var strings = str.Split(";");

            if (int.TryParse(strings[0], out var result0))
            {
                reslut.x = result0;
            }
            else
            {
                Debug.LogError(string.Format("reslut.x:{0}", str));
            }

            if (int.TryParse(strings[1], out var result1))
            {
                reslut.y = result1;
            }
            else
            {
                Debug.LogError(string.Format("reslut.y:{0}", str));
            }

            if (int.TryParse(strings[2], out var result2))
            {
                reslut.z = result2;
            }
            else
            {
                Debug.LogError(string.Format("reslut.z:{0}", str));
            }

            return reslut;
        }


        #region Audio

        /// <summary>
        /// 创建一个音效
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="pos"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateAudioClip(EventDescription audio, out EventInstance eventInstance, float volume = 1,
            float3 pos = default)
        {
            //audio.getInstanceList(out)
            audio.createInstance(out var instance);
            eventInstance = instance;
            //instance.stop();
            //instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
            instance.setVolume(volume);
            instance.start();
            instance.release();
            //instance.setVolume()
        }

        /// <summary>
        /// 创建一个音效
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="pos"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyAudioClip(EventInstance eventInstance, STOP_MODE stopMode = STOP_MODE.IMMEDIATE)
        {
            Debug.Log($"DestroyAudioClip{eventInstance}");
            eventInstance.stop(stopMode);
            eventInstance.release();
            //instance.setVolume()
        }

        /// <summary>
        /// 创建一个音效
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="pos"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateAudioClip(GlobalConfigData globalConfigData, GameOthersData gameOthersData,
            int id, out EventInstance eventInstance)
        {
            ref var audio =
                ref globalConfigData.value.Value.configTbaudios.Get(id);
            eventInstance = default;
            if (gameOthersData.allAudioClips.TryGetValue(audio.group, out var clip))
            {
                Debug.Log($"CreateAudioClip:{id}");
                CreateAudioClip(clip, out var instance, audio.volume / 10000f);
                eventInstance = instance;
                return true;
            }

            Debug.LogError($"audio:{audio.group} is not exist!");
            return false;
        }

        #endregion


        /// <summary>
        /// 条件成立时(为boss技能类型并且cd好了),随机选择一个boss技能
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="configData"></param>
        /// <param name="rand"></param>
        /// <param name="monsterId"></param>
        /// <param name="withEffective"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryChooseACommonBossSkill(ref DynamicBuffer<Skill> skills, GlobalConfigData configData,
            Random rand, int monsterId, bool withEffective,
            out int skillId)
        {
            bool canChoose = false;
            skillId = 0;
            ref var tbmonster = ref configData.value.Value.configTbmonsters.configTbmonsters;
            int monsterIndex = 0;
            for (int i = 0; i < tbmonster.Length; i++)
            {
                if (tbmonster[i].id == monsterId)
                {
                    monsterIndex = i;
                    break;
                }
            }

            ref var monster = ref configData.value.Value.configTbmonsters.configTbmonsters[monsterIndex];
            var canSelectSkills = new NativeList<int>(Allocator.Temp);

            foreach (var skill in skills)
            {
                if (skill.Int32_10 == 2)
                {
                    if (skill.Single_7 <= 0)
                    {
                        ref var skillsconfig = ref configData.value.Value.configTbskills.configTbskills;
                        int skillIndex = 0;
                        for (int j = 0; j < skillsconfig.Length; j++)
                        {
                            if (skillsconfig[j].id == skill.Int32_0)
                            {
                                skillIndex = j;
                                break;
                            }
                        }

                        //ref var skill0 = ref skillsconfig[skillIndex];
                        //怪物技能如果cd为0 则为被动技能 不进行选择
                        if (!skill.Boolean_17)
                        {
                            canSelectSkills.Add(skill.Int32_0);
                        }
                    }
                }
            }


            if (canSelectSkills.Length > 0)
            {
                var randIndex = rand.NextInt(0, canSelectSkills.Length);
                skillId = canSelectSkills[randIndex];
                canChoose = true;
                if (withEffective)
                {
                    for (int i = 0; i < skills.Length; i++)
                    {
                        var temp = skills[i];
                        if (temp.Int32_0 == skillId)
                        {
                            temp.Int32_6 = 0;
                            //temp.Boolean_11 = false;
                            skills[i] = temp;
                            break;
                        }

                        //skills[i] = temp;
                    }
                }
            }


            return canChoose;
        }

        #region Animation

        /// <summary>
        /// 拿到string的动画Enum
        /// </summary>
        /// <param name="animStr">string动画</param>
        /// <param name="animationEnum">动画Enum</param>
        /// <returns>是否有</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAnimEnum(FixedString128Bytes animStr, out AnimationEnum animationEnum)
        {
            animationEnum = AnimationEnum.Run;

            if (animStr.Contains((FixedString128Bytes)$"idle"))
            {
                animationEnum = AnimationEnum.Idle;
            }
            else if (animStr.Contains((FixedString128Bytes)$"run"))
            {
                animationEnum = AnimationEnum.Run;
            }
            else if (animStr.Contains((FixedString128Bytes)$"getHit"))
            {
                animationEnum = AnimationEnum.GetHit;
            }
            else if (animStr.Contains((FixedString128Bytes)$"die"))
            {
                animationEnum = AnimationEnum.Die;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill1"))
            {
                animationEnum = AnimationEnum.Skill1;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill2"))
            {
                animationEnum = AnimationEnum.Skill2;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill3"))
            {
                animationEnum = AnimationEnum.Skill3;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill4"))
            {
                animationEnum = AnimationEnum.Skill4;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill5"))
            {
                animationEnum = AnimationEnum.Skill5;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill6"))
            {
                animationEnum = AnimationEnum.Skill6;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill7"))
            {
                animationEnum = AnimationEnum.Skill7;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill8"))
            {
                animationEnum = AnimationEnum.Skill8;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill9"))
            {
                animationEnum = AnimationEnum.Skill9;
            }
            else if (animStr.Contains((FixedString128Bytes)$"skill10"))
            {
                animationEnum = AnimationEnum.Skill10;
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion

        #region FrameAnimation

        /// <summary>
        /// 根据障碍物状态(生命值比例) 切不同的序列帧动画
        /// </summary>
        /// <param name="obstacleId">障碍物id</param>
        /// <param name="chaStats">障碍物属性</param>
        /// <param name="configData">表</param>
        /// <param name="animIndex">第几张序列帧</param>
        /// <returns>能否</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetObstacleJiYuAnimIndex(int obstacleId, ChaStats chaStats, GlobalConfigData configData,
            long damage,
            out int animIndex, out bool isStrongShake)
        {
            isStrongShake = false;
            animIndex = 1;
            ref var tbscene_modules = ref configData.value.Value.configTbscene_modules.configTbscene_modules;

            int obstacleIndex = 0;
            for (int i = 0; i < tbscene_modules.Length; i++)
            {
                if (tbscene_modules[i].id == obstacleId)
                {
                    obstacleIndex = i;
                    break;
                }
            }

            ref var scene_module =
                ref tbscene_modules[obstacleIndex];
            ref var picList = ref scene_module.hpPic;
            if (picList.Length <= 0)
            {
                Debug.Log($"scene_module.id{scene_module.id}检查配表是否正确 当前的hpPic字段为空 但是该障碍物有血量");
                return false;
            }

            var lastHp = chaStats.chaResource.hp + damage;
            float lastHpRatios = lastHp / (float)chaStats.chaProperty.maxHp;
            float hpRatios = chaStats.chaResource.hp / (float)chaStats.chaProperty.maxHp;
            int picIndex = picList.Length - 1;
            for (int i = 0; i < picList.Length; i++)
            {
                var pic = picList[i];
                var ratios = pic.x / 10000f;
                if (hpRatios > ratios)
                {
                    // if (i == 0)
                    // {
                    //     //return true;
                    // }

                    picIndex = i - 1;
                    break;
                }
            }

            int lastPicIndex = picList.Length - 1;
            for (int i = 0; i < picList.Length; i++)
            {
                var pic = picList[i];
                var ratios = pic.x / 10000f;
                if (lastHpRatios > ratios)
                {
                    // if (i == 0)
                    // {
                    //     //return true;
                    // }

                    lastPicIndex = i - 1;
                    break;
                }
            }

            if (lastPicIndex != picIndex)
            {
                isStrongShake = true;
            }

            if (picIndex > -1)
            {
                animIndex = picList[picIndex].y;
            }

            return true;
        }

        // [BurstCompile]
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // private static void GenerateBossWarning(ref EntityCommandBuffer.ParallelWriter ecb,
        //     ComponentLookup<LocalTransform> cdfeLocalTransform,
        //     PrefabMapData prefabMapData, GlobalConfigData globalConfigData,
        //     LocalTransform loc, int sortKey, int warningID)
        // {
        //     //UnityEngine.Debug.Log($"GenerateBossWarning:{warningID[0]}");
        //     ref var curSpecial =
        //         ref globalConfigData.value.Value.configTbspecial_effects.Get(warningID);
        //
        //     FixedString128Bytes modelName = curSpecial.model;
        //
        //     var effectParaID = curSpecial.para1;
        //     var effectSizeX = curSpecial.sizeX / 1000f;
        //     var effectSizeY = curSpecial.sizeY / 1000f;
        //     int moveIndex = -1;
        //     ref var specialEffectsMoveConfig =
        //         ref globalConfigData.value.Value.configTbspecial_effect_movements.Get(effectParaID);
        //
        //
        //     var moveType = specialEffectsMoveConfig.type;
        //
        //     var duration = specialEffectsMoveConfig.para1 / 1000f;
        //
        //     var prefab = prefabMapData.prefabHashMap[modelName];
        //     var preTran = cdfeLocalTransform[prefab];
        //     preTran.Position = loc.Position;
        //     preTran.Rotation = loc.Rotation;
        //     var atkRangWarningData = new JiYuATKRangeWarningData { };
        //     new float4(0, 0, 0, 0);
        //     switch (moveType)
        //     {
        //         case 101:
        //             preTran.Scale = effectSizeX;
        //             atkRangWarningData.value.w = 0;
        //             break;
        //         case 102:
        //             preTran.Scale = effectSizeX;
        //             atkRangWarningData.value.x = specialEffectsMoveConfig.para2;
        //             atkRangWarningData.value.w = 0;
        //             break;
        //         case 103:
        //             preTran.Scale = 1;
        //             ecb.AddComponent(sortKey, prefab, new PostTransformMatrix
        //             {
        //                 Value = float4x4.Scale(preTran.Scale * (effectSizeX),
        //                     preTran.Scale * (effectSizeY), 1)
        //             });
        //             atkRangWarningData.value.w = 0;
        //             break;
        //         case 104:
        //             preTran.Scale = effectSizeX;
        //             atkRangWarningData.value.w = 1;
        //             break;
        //         case 105:
        //             preTran.Scale = effectSizeX;
        //             atkRangWarningData.value.x = specialEffectsMoveConfig.para2;
        //             atkRangWarningData.value.w = 1;
        //             break;
        //         case 106:
        //             preTran.Scale = 1;
        //             ecb.AddComponent(sortKey, prefab, new PostTransformMatrix
        //             {
        //                 Value = float4x4.Scale(preTran.Scale * (effectSizeX),
        //                     preTran.Scale * (effectSizeY), 1)
        //             });
        //             atkRangWarningData.value.w = 1;
        //             break;
        //         case 107:
        //             preTran.Scale = effectSizeX;
        //             atkRangWarningData.value.w = duration / 0.3f;
        //             break;
        //         case 108:
        //             preTran.Scale = effectSizeX;
        //             atkRangWarningData.value.x = specialEffectsMoveConfig.para2;
        //             atkRangWarningData.value.w = duration / 0.3f;
        //             break;
        //         case 109:
        //             preTran.Scale = 1;
        //             ecb.AddComponent(sortKey, prefab, new PostTransformMatrix
        //             {
        //                 Value = float4x4.Scale(preTran.Scale * (effectSizeX),
        //                     preTran.Scale * (effectSizeY), 1)
        //             });
        //             atkRangWarningData.value.w = duration / 0.3f;
        //             break;
        //     }
        //
        //     atkRangWarningData.value.z = duration;
        //
        //     //cdfeLocalTransform[prefab] = preTran;
        //     var ins = ecb.Instantiate(sortKey, prefab);
        //     ecb.SetComponent(sortKey, ins, preTran);
        //     ecb.AddComponent(sortKey, ins, atkRangWarningData);
        //
        //     ecb.AddComponent(sortKey, ins, new TimeToDieData
        //     {
        //         duration = duration + 1
        //     });
        //     // if (gameTimeData.logicTime.gameTimeScale > math.EPSILON)
        //     // {
        //     //    
        //     // }
        // }

        /// <summary>
        /// 生成特效(序列帧动画)
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="sortkey"></param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="configData"></param>
        /// <param name="specialEffectId"></param>
        /// <param name="prefabMapData"></param>
        /// <param name="carrier"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity SpawnBuffSpecialEffect(ref EntityCommandBuffer.ParallelWriter ecb, int sortkey, float eT,
            ComponentLookup<LocalTransform> cdfeLocalTransform, GameTimeData gameTimeData, GlobalConfigData configData,
            int specialEffectId, PrefabMapData prefabMapData, Entity carrier,
            LocalTransform tran = default)
        {
            //Debug.LogError($"eT:{eT}");
            Entity ins = Entity.Null;
            ref var tbspecial_effects = ref configData.value.Value.configTbspecial_effects.configTbspecial_effects;


            int specialEffectIndex = -1;
            for (int j = 0; j < tbspecial_effects.Length; j++)
            {
                if (tbspecial_effects[j].id == specialEffectId)
                {
                    specialEffectIndex = j;
                    break;
                }
            }

            if (specialEffectIndex == -1)
            {
                Debug.LogError($"vfx ID:{specialEffectId} is not exist!");
                return Entity.Null;
            }

            ref var specialEffect =
                ref tbspecial_effects[specialEffectIndex];

            if (specialEffect.type == 7)
            {
                // GenerateBossWarning(ref ecb, cdfeLocalTransform, prefabMapData, configData, tran, sortkey,
                //     specialEffect.id);
            }
            else
            {
                if (!prefabMapData.prefabHashMap.TryGetValue(specialEffect.model, out var prefab))
                {
                    Debug.LogError($"{specialEffect.model} is not exist!");
                    return Entity.Null;
                }

                Debug.Log($"{specialEffect.model} is Spawned!!!!");
                ins = ecb.Instantiate(sortkey, prefab);
                var oldTran = cdfeLocalTransform[prefab];
                var carrierTran = cdfeLocalTransform[carrier];

                float3 offset = default;
                float scaleMulti = 1f;
                switch (specialEffect.type)
                {
                    case 1:

                        switch (specialEffect.para2)
                        {
                            case 1:
                                scaleMulti = 1f;
                                break;
                            case 2:
                                scaleMulti = 1f / carrierTran.Scale;
                                break;
                        }


                        ecb.AddComponent(sortkey, ins, new Parent
                        {
                            Value = carrier
                        });
                        ecb.AppendToBuffer(sortkey, carrier, new LinkedEntityGroup
                        {
                            Value = ins
                        });

                        switch (specialEffect.offset)
                        {
                            case 0:
                                offset = float3.zero;
                                break;
                            case 1:
                                offset.y = 1;
                                break;
                            case 2:
                                offset.x = -1;
                                break;
                            case 3:
                                offset.x = 1;
                                break;
                            case 4:
                                offset.y = -1;
                                break;
                        }

                        //oldTran.Position = offset * (carrierTran.Scale / 2f);

                        //ecb.SetComponent(sortkey, ins, oldTran);
                        var offsetPara = specialEffect.offsetPara.Length > 0
                            ? specialEffect.offsetPara[0] / 1000f
                            : 0;
                        oldTran.Position = offset * (offsetPara) * scaleMulti;

                        break;
                    case 3:

                        switch (specialEffect.offset)
                        {
                            case 0:
                                offset = float3.zero;
                                break;
                            case 1:
                                offset.y = 1;
                                offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                break;
                            case 2:
                                offset.x = -1;
                                offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                break;
                            case 3:
                                offset.x = 1;
                                offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                break;
                            case 4:
                                offset.y = -1;
                                offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                break;
                            case 5:
                                offset = MathHelper.Forward(tran.Rotation);
                                offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                break;
                        }

                        oldTran.Position = tran.Position + offset;
                        //oldTran.Position = tran.Position;
                        oldTran.Rotation = tran.Rotation;

                        //TODO:正方向垂直向上 删掉
                        //oldTran = oldTran.RotateZ(math.radians(180));
                        break;
                    default:
                        Debug.LogError($"当前特效类型无有效类型 id:{specialEffect.id}");
                        break;
                }

                if (specialEffect.sizeX != specialEffect.sizeY)
                {
                    ecb.AddComponent(sortkey, ins, new PostTransformMatrix
                    {
                        Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                            specialEffect.sizeY / 10000f * scaleMulti, 1)
                    });
                }
                else
                {
                    oldTran.Scale = specialEffect.sizeX / 10000f * scaleMulti;
                }

                ecb.SetComponent(sortkey, ins, oldTran);
                ecb.AddComponent(sortkey, ins, new TimeToDieData
                {
                    duration = specialEffect.maxTime / 1000f
                });
                ecb.SetComponent(sortkey, ins, new JiYuFrameAnimSpeed
                {
                    value = (specialEffect.loopSpeed / 1000f) / gameTimeData.logicTime.gameTimeScale
                });
                ecb.SetComponent(sortkey, ins, new JiYuFrameAnimLoop
                {
                    value = specialEffect.loopType == 1 ? 0 : 1
                });
                ecb.SetComponent(sortkey, ins, new JiYuSort
                {
                    value = new int2(specialEffect.zSort, specialEffect.zIndex)
                });
                ecb.SetComponent(sortkey, ins, new SpecialEffectData
                {
                    id = specialEffect.id,
                    stateId = 0,
                    groupId = 0,
                    sort = 0,
                    caster = carrier
                });
            }


            return ins;
        }


        /// <summary>
        ///  生成特效(序列帧动画)
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="sortkey"></param>
        /// <param name="eT"></param>
        /// <param name="cdfeLocalTransform"></param>
        /// <param name="cdfePostTransformMatrix"></param>
        /// <param name="gameTimeData"></param>
        /// <param name="configData"></param>
        /// <param name="specialEffectIds"></param>
        /// <param name="prefabMapData"></param>
        /// <param name="carrier"></param>
        /// <param name="isWarning"></param>
        /// <param name="skillId"></param>
        /// <param name="casterTran"></param>
        /// <param name="targetPos">索敌索到的目标点 有可能是释放者位置(未索敌成功)</param>
        /// <param name="skills"></param>
        /// <param name="time"></param>
        /// <param name="target">索敌索到的目标 有可能是null</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity SpawnSpecialEffect(ref EntityCommandBuffer.ParallelWriter ecb, int sortkey, float eT,
            ComponentLookup<LocalTransform> cdfeLocalTransform,
            ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix, GameTimeData gameTimeData,
            GlobalConfigData configData,
            ref BlobArray<int> specialEffectIds, PrefabMapData prefabMapData, Entity carrier,
            bool isWarning = false, int skillId = 0, LocalTransform casterTran = default, float3 targetPos = default,
            DynamicBuffer<Skill> skills = default, int time = 1, Entity target = default)
        {
            Debug.Log($"特效：SpawnSpecialEffect：{specialEffectIds.Length}");
            Entity ins = Entity.Null;


            for (int i = 0; i < specialEffectIds.Length; i++)
            {
                int specialEffectId = specialEffectIds[i];

                ref var specialEffect = ref configData.value.Value.configTbspecial_effects.Get(specialEffectId);
                if (specialEffect.type == 8)
                {
                    var eventE = ecb.CreateEntity(sortkey);
                    ecb.AddComponent(sortkey, eventE, new HybridEventData()
                    {
                        type = 3,
                        args = new float4(1, specialEffect.para1, specialEffect.sizeX, 0),
                        bossEntity = carrier,
                        strArgs = $"{specialEffect.model}"
                    });
                    //Debug.LogError($"specialEffect.model{specialEffect.model}");
                    ins = ecb.CreateEntity(sortkey);
                    ecb.AddComponent(sortkey, ins, new TimeToDieData
                    {
                        duration = (specialEffect.maxTime / 1000f) / gameTimeData.logicTime.gameTimeScale
                    });

                    var specialEffectData = new SpecialEffectData
                    {
                        id = specialEffect.id,
                        stateId = 0,
                        groupId = 0,
                        sort = 0,
                        caster = carrier,
                        tick = 0,
                        skillId = skillId,
                        startPos = default,
                        targetPos = default
                    };
                    ecb.AddComponent(sortkey, ins, specialEffectData);
                }
                else if (specialEffect.type == 5)
                {
                    var eventE = ecb.CreateEntity(sortkey);
                    ecb.AddComponent(sortkey, eventE, new HybridEventData()
                    {
                        type = 6,
                        args = new float4(specialEffect.loopSpeed / 1000f, 0, 0, 0),
                        bossEntity = carrier,
                        strArgs = $"{specialEffect.model}"
                    });
                    Debug.LogError($"specialEffect.model{specialEffect.model}");
                    ins = ecb.CreateEntity(sortkey);
                    ecb.AddComponent(sortkey, ins, new TimeToDieData
                    {
                        duration = (specialEffect.maxTime / 1000f) / gameTimeData.logicTime.gameTimeScale
                    });

                    var specialEffectData = new SpecialEffectData
                    {
                        id = specialEffect.id,
                        stateId = 0,
                        groupId = 0,
                        sort = 0,
                        caster = carrier,
                        tick = 0,
                        skillId = skillId,
                        startPos = default,
                        targetPos = default
                    };
                    ecb.AddComponent(sortkey, ins, specialEffectData);
                }
                else
                {
                    if (!prefabMapData.prefabHashMap.TryGetValue(specialEffect.model, out var prefab))
                    {
                        Debug.LogError($"{specialEffect.model} is not exist!");
                        continue;
                    }

                    bool isSpawned = false;
                    if (specialEffect.type == 6 && specialEffect.para1 != 3)
                    {
                        // for (int m = 0; m < skills.Length; m++)
                        // {
                        //     if (skills[m].Int32_0 == skillId)
                        //     {
                        //         var skilltemp = skills[m];
                        //         //skilltemp.LocalTransform_15 = targetPos;
                        //         skills[m] = skilltemp;
                        //         break;
                        //     }
                        // }

                        Debug.LogError($"{time}timetime");
                        if (time != 1)
                        {
                            isSpawned = true;
                            //skilltemp.Boolean_16 = true;
                        }
                    }

                    if (isSpawned)
                    {
                        return Entity.Null;
                    }

                    Debug.Log(
                        $"特效skillId:{skillId} {specialEffect.model} casterTran.Position{casterTran.Position} targetPos{targetPos} is Spawned!!!!");
                    ins = ecb.Instantiate(sortkey, prefab);
                    var oldTran = cdfeLocalTransform[prefab];
                    var carrierTran = cdfeLocalTransform[carrier];
                    var specialEffectData = new SpecialEffectData
                    {
                        id = specialEffect.id,
                        stateId = 0,
                        groupId = 0,
                        sort = 0,
                        caster = carrier,
                        tick = 0,
                        skillId = skillId,
                        startPos = default,
                        targetPos = default
                    };
                    float3 offset = default;
                    float scaleMulti = 1f;
                    float offsetPara = 0;
                    var jiyuAtkWarning = new JiYuATKRangeWarningData();

                    switch (specialEffect.type)
                    {
                        case 1:

                            switch (specialEffect.para2)
                            {
                                case 1:
                                    scaleMulti = 1f;
                                    break;
                                case 2:
                                    scaleMulti = 1f / carrierTran.Scale;
                                    break;
                            }

                            Entity carr = specialEffect.para4 == 1 ? carrier : target;
                            if (carr == Entity.Null)
                            {
                                ecb.DestroyEntity(sortkey, ins);

                                return Entity.Null;
                            }

                            ecb.AddComponent(sortkey, ins, new Parent
                            {
                                Value = carr
                            });
                            ecb.AppendToBuffer(sortkey, carr, new LinkedEntityGroup
                            {
                                Value = ins
                            });

                            switch (specialEffect.offset)
                            {
                                case 0:
                                    offset = float3.zero;
                                    break;
                                case 1:
                                    offset.y = 1;
                                    break;
                                case 2:
                                    offset.x = -1;
                                    break;
                                case 3:
                                    offset.x = 1;
                                    break;
                                case 4:
                                    offset.y = -1;
                                    break;
                                case 5:
                                    offset = MathHelper.Forward(casterTran.Rotation);
                                    //offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                            }

                            //oldTran.Position = offset * (carrierTran.Scale / 2f);
                            offsetPara = specialEffect.offsetPara.Length > 0
                                ? specialEffect.offsetPara[0] / 1000f
                                : 0;
                            if (specialEffect.para3 == 1)
                            {
                                oldTran.Rotation = casterTran.Rotation;
                            }

                            oldTran.Position = offset * (offsetPara) * scaleMulti;
                            //ecb.SetComponent(sortkey, ins, oldTran);


                            break;
                        case 2:
                            casterTran = cdfeLocalTransform[carrier];

                            switch (specialEffect.offset)
                            {
                                case 0:
                                    offset = float3.zero;
                                    break;
                                case 1:
                                    offset.y = 1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 2:
                                    offset.x = -1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 3:
                                    offset.x = 1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 4:
                                    offset.y = -1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 5:

                                    offset = MathHelper.Forward(casterTran.Rotation);
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                            }

                            if (specialEffect.para3 == 1)
                            {
                                oldTran.Rotation = casterTran.Rotation;
                            }

                            Debug.Log($"targetPos{targetPos} {carrier.Index}");
                            oldTran.Position = casterTran.Position + offset;

                            specialEffectData.startPos = oldTran.Position;
                            specialEffectData.targetPos = targetPos;
                            if (math.length(targetPos - oldTran.Position) < 5f)
                            {
                                Debug.Log($"目标选点和自身相同{targetPos}  {oldTran.Position}");
                                specialEffectData.targetPos =
                                    targetPos + MathHelper.Forward(casterTran.Rotation) * 30f;
                            }

                            oldTran.Position = casterTran.Position + offset + 9999;
                            break;
                        case 3:

                            switch (specialEffect.offset)
                            {
                                case 0:
                                    offset = float3.zero;
                                    break;
                                case 1:
                                    offset.y = 1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 2:
                                    offset.x = -1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 3:
                                    offset.x = 1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 4:
                                    offset.y = -1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 5:
                                    offset = MathHelper.Forward(casterTran.Rotation);
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                            }

                            // + offset
                            oldTran.Position = targetPos + offset;

                            //oldTran.Position = casterTran.Position;
                            switch (specialEffect.para1)
                            {
                                case 1:
                                    oldTran.Rotation = casterTran.Rotation;
                                    oldTran = oldTran.RotateZ(math.radians(specialEffect.para2));
                                    break;
                                case 0:
                                    oldTran = oldTran.RotateZ(math.radians(specialEffect.para2));

                                    break;
                            }

                            break;
                        case 6:
                            switch (specialEffect.para1)
                            {
                                case 1:
                                    var bool3 = math.abs(targetPos - casterTran.Position) < math.EPSILON;
                                    if (bool3.x && bool3.y && bool3.z)
                                    {
                                        casterTran.Position = cdfeLocalTransform[carrier].Position;
                                    }

                                    var vector = targetPos - casterTran.Position;
                                    Debug.Log($"vector {vector}  {targetPos} {casterTran.Position}");
                                    var postTransformMatrix = cdfePostTransformMatrix[prefab];
                                    //postTransformMatrix.Value.Scale().
                                    ecb.AddComponent(sortkey, ins, new PostTransformMatrix
                                    {
                                        // Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                                        //     math.length(vector) * scaleMulti, 1)
                                        Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                                            math.length(vector) * 0f, 1)
                                    });
                                    //oldTran.Scale = 4;
                                    oldTran.Position = (targetPos + casterTran.Position) / 2f;
                                    oldTran.Rotation = MathHelper.LookRotation2D(vector);
                                    if (specialEffect.para2 == 1)
                                    {
                                        ecb.AddComponent(sortkey, ins, new JiYuMainTexST
                                        {
                                            value = new float4(1, math.length(vector), 0, 0)
                                        });
                                    }

                                    //oldTran = oldTran.RotateZ(math.radians(180));
                                    // MathHelper
                                    // quaternion.RotateZ()
                                    break;
                                case 2:
                                    ecb.AddComponent(sortkey, ins, new Parent
                                    {
                                        Value = carrier
                                    });
                                    ecb.AppendToBuffer(sortkey, carrier, new LinkedEntityGroup
                                    {
                                        Value = ins
                                    });

                                    switch (specialEffect.offset)
                                    {
                                        case 0:
                                            offset = float3.zero;
                                            break;
                                        case 1:
                                            offset.y = 1;
                                            break;
                                        case 2:
                                            offset.x = -1;
                                            break;
                                        case 3:
                                            offset.x = 1;
                                            break;
                                        case 4:
                                            offset.y = -1;
                                            break;
                                        case 5:
                                            offset = MathHelper.Forward(casterTran.Rotation);
                                            //offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                            break;
                                    }

                                    //oldTran.Position = offset * (carrierTran.Scale / 2f);
                                    // offsetPara = specialEffect.offsetPara.Length > 0
                                    //     ? specialEffect.offsetPara[0] / 1000f
                                    //     : 0;
                                    //oldTran.Position = offset * (offsetPara) * scaleMulti;
                                    oldTran.Position = 999;

                                    switch (specialEffect.para2)
                                    {
                                        case 1:
                                            scaleMulti = 1f;
                                            break;
                                        case 2:
                                            scaleMulti = 1f / carrierTran.Scale;
                                            break;
                                    }

                                    //oldTran.Scale = specialEffect.sizeX / 10000f * scaleMulti;
                                    //oldTran.Rotation = MathHelper.LookRotation2D(vector2);
                                    break;
                            }


                            break;
                        case 7:

                            switch (specialEffect.offset)
                            {
                                case 0:
                                    offset = float3.zero;
                                    break;
                                case 1:
                                    offset.y = 1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 2:
                                    offset.x = -1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 3:
                                    offset.x = 1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 4:
                                    offset.y = -1;
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                                case 5:
                                    offset = MathHelper.Forward(casterTran.Rotation);
                                    offset = offset * (specialEffect.offsetPara[0] / 1000f);
                                    break;
                            }

                            switch (specialEffect.para1)
                            {
                                case 1:
                                    oldTran.Position = targetPos + offset;
                                    oldTran.Rotation = casterTran.Rotation;
                                    break;
                                case 2:
                                    oldTran.Position = casterTran.Position + offset;
                                    oldTran.Rotation = casterTran.Rotation;
                                    break;
                                case 3:
                                    oldTran.Position = casterTran.Position + offset;
                                    oldTran.Rotation = casterTran.Rotation;
                                    break;
                            }

                            Debug.Log($"预警 {oldTran.Position} {oldTran.Rotation}");
                            // + offset
                            //oldTran.Position = targetPos + offset;

                            //oldTran.Position = casterTran.Position;
                            //oldTran.Rotation = casterTran.Rotation;
                            // switch (specialEffect.para1)
                            // {
                            //     case 1:
                            //         oldTran.Rotation = casterTran.Rotation;
                            //         oldTran = oldTran.RotateZ(math.radians(specialEffect.para2));
                            //         break;
                            //     case 0:
                            //         oldTran = oldTran.RotateZ(math.radians(specialEffect.para2));
                            //
                            //         break;
                            // }

                            break;
                        default:
                            Debug.LogError($"当前特效类型无有效类型 id:{specialEffect.id}");
                            break;
                    }

                    if (cdfePostTransformMatrix.HasComponent(prefab))
                    {
                        var postTransformMatrix = cdfePostTransformMatrix[prefab];
                        var scale = postTransformMatrix.Value.DecomposeScale();
                        scaleMulti *= scale.x;
                    }


                    if (!(specialEffect.type == 6 && specialEffect.para1 != 2))
                    {
                        // if (cdfePostTransformMatrix.HasComponent(prefab))
                        // {
                        //     ecb.AddComponent(sortkey, ins, new PostTransformMatrix
                        //     {
                        //         Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                        //             specialEffect.sizeY / 10000f * scaleMulti, 1)
                        //     });
                        // }
                        // else
                        // {
                        //     oldTran.Scale = specialEffect.sizeX / 10000f * scaleMulti;
                        // }
                        if (specialEffect.sizeX != specialEffect.sizeY)
                        {
                            ecb.AddComponent(sortkey, ins, new PostTransformMatrix
                            {
                                Value = float4x4.Scale(specialEffect.sizeX / 10000f * scaleMulti,
                                    specialEffect.sizeY / 10000f * scaleMulti, 1)
                            });
                        }
                        else
                        {
                            oldTran.Scale = specialEffect.sizeX / 10000f * scaleMulti;
                        }
                    }

                    if (gameTimeData.logicTime.gameTimeScale < 0.001f)
                    {
                        gameTimeData.logicTime.gameTimeScale = 1;
                    }


                    ecb.SetComponent(sortkey, ins, oldTran);
                    ecb.AddComponent(sortkey, ins, new TimeToDieData
                    {
                        duration = (specialEffect.maxTime / 1000f) / gameTimeData.logicTime.gameTimeScale
                    });

                    if (specialEffect.loopType == 3)
                    {
                        ecb.AddComponent(sortkey, ins, new JiYuAnimIndex()
                        {
                            value = 1
                        });
                    }

                    if (specialEffect.type != 7)
                    {
                        ecb.SetComponent(sortkey, ins, new JiYuFrameAnimSpeed
                        {
                            value = (specialEffect.loopSpeed / 1000f) / gameTimeData.logicTime.gameTimeScale
                        });
                        ecb.SetComponent(sortkey, ins, new JiYuFrameAnimLoop
                        {
                            value = specialEffect.loopType == 1 ? 0 : 1
                        });
                        ecb.SetComponent(sortkey, ins, specialEffectData);
                    }
                    else
                    {
                        jiyuAtkWarning.value.w = specialEffect.para2;
                        jiyuAtkWarning.value.x = specialEffect.para2;
                        jiyuAtkWarning.value.z =
                            (specialEffect.loopSpeed / 1000f) / gameTimeData.logicTime.gameTimeScale;
                        ecb.SetComponent(sortkey, ins, jiyuAtkWarning);
                    }


                    ecb.SetComponent(sortkey, ins, new JiYuSort
                    {
                        value = new int2(specialEffect.zSort, specialEffect.zIndex)
                    });
                }
                Debug.LogError($"specialEffect.model{specialEffect.model}");
            }


            return ins;
        }

        /// <summary>
        /// 清理失效linked
        /// </summary>
        /// <param name="dynamicBuffer"></param>
        /// <param name="storageInfoFromEntity"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearLinkedEntityGroup(ref DynamicBuffer<LinkedEntityGroup> dynamicBuffer,
            ref EntityStorageInfoLookup storageInfoFromEntity)
        {
            for (int i = 0; i < dynamicBuffer.Length; i++)
            {
                var linkedEntity = dynamicBuffer[i];
                if (!storageInfoFromEntity.Exists(linkedEntity.Value))
                {
                    dynamicBuffer.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 清理失效linked
        /// </summary>
        /// <param name="dynamicBuffer"></param>
        /// <param name="storageInfoFromEntity"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearLinkedEntityGroup(ref DynamicBuffer<Child> dynamicBuffer,
            ref EntityStorageInfoLookup storageInfoFromEntity)
        {
            for (int i = 0; i < dynamicBuffer.Length; i++)
            {
                var linkedEntity = dynamicBuffer[i];
                if (!storageInfoFromEntity.Exists(linkedEntity.Value))
                {
                    dynamicBuffer.RemoveAt(i);
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableShaderFX(ref EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity,
            int id, bool enable = true)
        {
            switch (id)
            {
                case 202:
                    ecb.AddComponent(sortKey, entity, new JiYuOverlayIndex
                    {
                        value = 0
                    });
                    ecb.AddComponent(sortKey, entity, new JiYuOverlayTexEnable()
                    {
                        value = enable ? 1 : 0
                    });
                    // ecb.AddComponent(sortKey, entity, new JiYuOverlayColor()
                    // {
                    //     value = Color2Float4(enable? Color.white)
                    // });
                    break;
                //怪物受击
                case 999101:
                    ecb.AddComponent(sortKey, entity, new JiYuOverlayIndex
                    {
                        value = 0
                    });
                    ecb.AddComponent(sortKey, entity, new JiYuOverlayTexEnable()
                    {
                        value = enable ? 1 : 0
                    });
                    ecb.AddComponent(sortKey, entity, new JiYuOverlayColor()
                    {
                        value = Color2Float4(enable ? Color.white : Color.white)
                    });

                    break;
                //角色闪避残影
                case 999001:
                    ecb.AddComponent(sortKey, entity, new JiYuChromAberrEnable()
                    {
                        value = enable ? 1 : 0
                    });
                    ecb.AddComponent(sortKey, entity, new JiYuOutlineEnable()
                    {
                        value = enable ? 0 : 1
                    });

                    break;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Color2Float4(Color color)
        {
            return new float4(color.r, color.g, color.b, color.a);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySpawnSpecialEffect(ref EntityCommandBuffer.ParallelWriter ecb,
            DynamicBuffer<LinkedEntityGroup> linkedEntityGroup, int sortkey,
            ComponentLookup<SpecialEffectData> cdfeSpecialEffectData,
            GlobalConfigData configData, int curBuffId, out int specialEffectId, out int curGroup,
            out int curSort, bool isDestory = false)
        {
            specialEffectId = 0;
            curGroup = 0;
            curSort = 0;


            ref var tbbattle_statuss = ref configData.value.Value.configTbbattle_statuss.configTbbattle_statuss;


            int buffIndex = -1;

            NativeHashMap<int, int> buffMap = new NativeHashMap<int, int>(tbbattle_statuss.Length, Allocator.Temp);
            for (int i = 0; i < tbbattle_statuss.Length; i++)
            {
                buffMap.TryAdd(tbbattle_statuss[i].id, tbbattle_statuss[i].animationGroup);
                if (tbbattle_statuss[i].id == curBuffId)
                {
                    buffIndex = i;
                }
            }


            if (buffIndex == -1)
            {
                Debug.LogError($"找不到这个battle_statuss id:{curBuffId}");
                return false;
            }

            ref var battle_status =
                ref tbbattle_statuss[buffIndex];

            //specialEffectId = battle_status.specialEffects;
            curGroup = battle_status.animationGroup;
            curSort = battle_status.sort;
            specialEffectId = battle_status.specialEffects;


            ref var tbspecial_effects = ref configData.value.Value.configTbspecial_effects.configTbspecial_effects;

            int specialEffectIndex = -1;
            for (int j = 0; j < tbspecial_effects.Length; j++)
            {
                if (tbspecial_effects[j].id == specialEffectId)
                {
                    specialEffectIndex = j;
                    break;
                }
            }

            if (specialEffectIndex == -1)
            {
                Debug.Log($"特效ID:{specialEffectId} 不存在!");
                return false;
            }

            for (int i = 0; i < linkedEntityGroup.Length; i++)
            {
                var linkedEntity = linkedEntityGroup[i];

                if (!cdfeSpecialEffectData.HasComponent(linkedEntity.Value)) continue;
                var specialEffectData = cdfeSpecialEffectData[linkedEntity.Value];
                if (isDestory)
                {
                    if (specialEffectData.stateId == curBuffId)
                    {
                        ecb.DestroyEntity(sortkey, linkedEntity.Value);
                        return true;
                    }

                    continue;
                }

                if (curGroup == specialEffectData.groupId)
                {
                    if (specialEffectData.sort < curSort)
                    {
                        ecb.DestroyEntity(sortkey, linkedEntity.Value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

                /*
                if (buffMap.TryGetValue(specialEffectData.stateId, out int groupId))
                {
                    if (curGroup == groupId)
                    {
                        if (specialEffectData.sort < curSort)
                        {
                            ecb.DestroyEntity(sortkey, linkedEntity.Value);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }*/
            }


            return true;
        }

        /// <summary>
        /// 按闪电链的逻辑排序一个float3 列表
        /// </summary>
        /// <param name="buffer"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySortChainList(ref DynamicBuffer<SpecialEffectChainBuffer> buffer,
            ComponentLookup<LocalTransform> cdfeLocalTransform, Entity centerEntity,
            EntityStorageInfoLookup storageInfoLookup)
        {
            //var array = buffer.ToNativeArray(Allocator.Temp);
            var list = new NativeList<SpecialEffectChainBuffer>(buffer.Length, Allocator.Temp);
            list.AddRange(buffer.ToNativeArray(Allocator.Temp));
            for (int i = 0; i < list.Length; i++)
            {
                if (!storageInfoLookup.Exists(list[i].Entity))
                {
                    list.RemoveAt(i);
                }
            }

            var newlist = new NativeList<SpecialEffectChainBuffer>(buffer.Length, Allocator.Temp);


            if (!storageInfoLookup.Exists(centerEntity))
            {
                foreach (var VARIABLE in list)
                {
                    if (!storageInfoLookup.Exists(centerEntity) && storageInfoLookup.Exists(VARIABLE.Entity))
                    {
                        centerEntity = VARIABLE.Entity;
                    }
                }
            }

            while (list.Length >= 1)
            {
                // if (!storageInfoLookup.Exists(centerEntity)&& storageInfoLookup.Exists(VARIABLE.Entity))
                // {
                //     centerEntity = VARIABLE.Entity;
                // }
                var index = FindMinDistanceIndex(list, cdfeLocalTransform, centerEntity);


                centerEntity = list[index].Entity;
                newlist.Add(list[index]);
                list.RemoveAt(index);
            }

            // foreach (var VARIABLE in newlist)
            // {
            //     //Debug.Log($"{VARIABLE.pos}");
            // }

            buffer.Clear();
            buffer.AddRange(newlist);
            //array.as
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindMinDistanceIndex(
            NativeList<SpecialEffectChainBuffer> buffer, ComponentLookup<LocalTransform> cdfeLocalTransform,
            Entity center)
        {
            //var newArray = new NativeArray<SpecialEffectChainBuffer>(buffer.Length, Allocator.Temp);
            if (buffer.Length <= 1)
            {
                return 0;
            }

            // if (!storageInfoLookup.Exists(center))
            // {
            //     
            //     
            // }

            float dis = 999;
            int index = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                var item = buffer[i];
                var thisDis =
                    math.length(cdfeLocalTransform[item.Entity].Position - cdfeLocalTransform[center].Position);
                if (thisDis < dis)
                {
                    dis = thisDis;
                    index = i;
                }
            }

            return index;
        }

        #endregion


        #region Monster/StateMachine

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetTargetId(int targetType)
        {
            return (int)math.pow(2, targetType); // 注意 n 较大时可能溢出
        }

        /// <summary>
        /// 给实体添加状态机
        /// </summary>
        /// <param name="ecb"></param>
        /// <param name="sortkey"></param>
        /// <param name="type">1:玩家状态机 2:小怪状态机 3:boss状态机</param>
        /// <param name="entity"></param>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddStateToStatesBuffer(ref EntityCommandBuffer.ParallelWriter ecb, int sortkey, int type,
            Entity entity)
        {
            //AddStateToStatesBuffer(State state, ref DynamicBuffer<State> statesBuffer)
            State defalutState = default;
            switch (type)
            {
                //player
                case 1:
                    defalutState = new PlayerMove
                    {
                        timeScale = 1,
                        duration = 1f,
                        stateId = 0
                    }.ToState();

                    ecb.SetComponent(sortkey, entity, new StateMachine
                    {
                        currentState = defalutState,
                        speed = 0,
                        startTranslation = default,
                        isInitialized = false,
                        transitionToStateIndex = -1,
                        curAnim = AnimationEnum.Idle,
                        lastAnim = AnimationEnum.Idle,
                        animSpeedScale = 1
                    });

                    ecb.AppendToBuffer(sortkey, entity, new PlayerMove
                    {
                        timeScale = 1,
                        duration = 1f,
                        stateId = 0
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new PlayerGetHit()
                    {
                        timeScale = 1,
                        duration = 0.4f,
                        stateId = 1
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new PlayerIdle()
                    {
                        timeScale = 1,
                        duration = 1f,
                        stateId = 2
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new PlayerDie()
                    {
                        timeScale = 1,
                        duration = 10f,
                        stateId = 3
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new PlayerBeController()
                    {
                        timeScale = 1,
                        duration = 10f,
                        stateId = 4
                    }.ToState());

                    break;
                //boss
                case 3:
                    defalutState = new CommonBossMove
                    {
                        stateId = 0,
                        timeScale = 1,
                        duration = 1.017f
                    }.ToState();
                    //ecb.AddComponent<BossTag>(sortkey, entity);
                    ecb.SetComponent(sortkey, entity, new StateMachine
                    {
                        currentState = defalutState,
                        speed = 0,
                        startTranslation = default,
                        isInitialized = false,
                        transitionToStateIndex = -1,
                        curAnim = AnimationEnum.Idle,
                        lastAnim = AnimationEnum.Idle,
                        animSpeedScale = 1
                    });

                    ecb.AppendToBuffer(sortkey, entity, new CommonBossMove
                    {
                        stateId = 0,
                        timeScale = 1,
                        duration = 1.017f
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new CommonBossGetHit()
                    {
                        stateId = 1,
                        timeScale = 1,
                        duration = 1f
                    }.ToState());

                    ecb.AppendToBuffer(sortkey, entity, new CommonBossAttack()
                    {
                        stateId = 2,
                        timeScale = 1,
                        duration = 10f,
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new CommonBossDie
                    {
                        stateId = 3,
                        timeScale = 1,
                        duration = 10f
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new CommonBossBeControlled()
                    {
                        stateId = 4,
                        timeScale = 1,
                        duration = 2f
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new CommonBossIdle()
                    {
                        stateId = 5,
                        timeScale = 1,
                        duration = 5f
                    }.ToState());
                    break;
                case 2:
                    defalutState = new LittleMonsterMove
                    {
                        stateId = 0,
                        timeScale = 1,
                        duration = 1.017f
                    }.ToState();

                    ecb.SetComponent(sortkey, entity, new StateMachine
                    {
                        currentState = defalutState,
                        speed = 0,
                        startTranslation = default,
                        isInitialized = false,
                        transitionToStateIndex = -1,
                        curAnim = AnimationEnum.Idle,
                        lastAnim = AnimationEnum.Idle,
                        animSpeedScale = 1
                    });

                    ecb.AppendToBuffer(sortkey, entity, new LittleMonsterMove
                    {
                        stateId = 0,
                        timeScale = 1,
                        duration = 1.017f
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new LittleMonsterGetHit()
                    {
                        stateId = 1,
                        timeScale = 1,
                        duration = 1f
                    }.ToState());

                    ecb.AppendToBuffer(sortkey, entity, new LittleMonsterAttack()
                    {
                        stateId = 2,
                        timeScale = 1,
                        duration = 10f
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new LittleMonsterDie
                    {
                        stateId = 3,
                        timeScale = 1,
                        duration = 10f,
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new LittleMonsterBeControlled()
                    {
                        stateId = 4,
                        timeScale = 1,
                        duration = 2f,
                    }.ToState());
                    ecb.AppendToBuffer(sortkey, entity, new LittleMonsterIdle()
                    {
                        stateId = 5,
                        timeScale = 1,
                        duration = 5f
                    }.ToState());
                    break;
            }
        }

        #endregion
    }
}