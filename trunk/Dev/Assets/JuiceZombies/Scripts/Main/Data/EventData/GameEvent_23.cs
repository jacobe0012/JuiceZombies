using cfg.config;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    /// <summary>
    ///事件类型 23 刷新怪物-继承召唤者
    /// </summary>
    public partial struct GameEvent_23 : IGameEvent
    {
        /// <summary>
        ///事件id 0
        /// </summary>
        public int id;

        /// <summary>
        /// 触发事件类型id 1
        /// </summary>
        public GameEventTriggerType triggerType;

        /// <summary>
        /// 触发间隔时间 2
        /// 当 触发事件类型为 间隔触发(3)时用该参数
        /// </summary>
        public float triggerGap;

        /// <summary>
        /// 指定时间(2)触发时间点 3
        /// </summary>
        public float onceTime;

        /// <summary>
        /// 剩余执行时间 4
        /// </summary>
        public float remainTime;

        /// <summary>
        /// 已经执行时间的tick  5
        /// </summary>
        public int durationTick;

        /// <summary>
        /// 是否永久事件 6
        /// 永久存在
        /// </summary>
        public bool isPermanent;

        /// <summary>
        /// 参数123   7
        /// </summary>
        public int3 args123;

        /// <summary>
        /// 参数456   8
        /// </summary>
        public int3 args456;

        /// <summary>
        /// 数量      9
        /// </summary>
        public int num;


        /// <summary>
        /// 触发次数上限 10
        /// </summary>
        public int maxLimit;

        /// <summary>
        /// 是否启用 11
        /// </summary>
        public bool isActive;

        /// <summary>
        /// 是否是环境事件 12
        /// </summary>
        public bool isEnvEvent;

        /// <summary>
        /// 是否是随机事件 13
        /// </summary>
        public bool isRandomEvent;

        /// <summary>
        /// 参数789   14
        /// </summary>
        public int3 args789;

        /// <summary>
        /// 技能id   15
        /// </summary>
        public int skillId;

        public void OnAbsorbBarrage(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnAuto(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterStay(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnClose(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            GenerateItem(ref refData, inData);
        }

        private void GenerateItem(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            Debug.Log($"GameEvent23");
            float3 parentPos = default;
            if (inData.wbe == inData.selefEntity)
            {
                parentPos = inData.cdfeLocalTransform[inData.player].Position;
            }
            else
            {
                parentPos = inData.cdfeLocalTransform[inData.selefEntity].Position;
            }

            if (args456.z == 1)
            {
                if (inData.cdfeEnemyData.HasComponent(inData.selefEntity))
                {
                    var enemyData = inData.cdfeEnemyData[inData.selefEntity];
                    if (refData.StorageInfoLookup.Exists(enemyData.target))
                    {
                        parentPos = inData.cdfeLocalTransform[enemyData.target].Position;
                    }
                }
            }

            ChaStats parentChaStats = default(ChaStats);
            if (!refData.cdfeChaStats.HasComponent(inData.selefEntity))
            {
                Debug.LogError($"23事件需要绑定释放者ChaStats不存在");
                return;
            }

            parentChaStats = refData.cdfeChaStats[inData.selefEntity];

            //Debug.Log($"GameEvent23  {parentPos}");

            NativeList<float3> posValues = new NativeList<float3>(args123.z, Allocator.Temp);
            bool isGap = false;

            float scale = GetItemScale(inData, ref refData, args123.x);


            for (int i = 0; i < args123.z; i++)
            {
                int maxTimes = 0;
                float3 pos = default;
                do
                {
                    pos = BuffHelper.GetRandomPointInCircle(parentPos,
                        args123.y / 1000f + args456.y / 1000f, args123.y / 1000f - args456.y / 1000f,
                        (uint)(inData.gameRandomData.seed + maxTimes));
                    maxTimes++;
                    if (maxTimes > 50)
                    {
                        isGap = true;
                        break;
                    }
                } while (!IsInBossMap(pos, scale, refData.cdfePlayerData[inData.player].playerOtherData, inData,
                             ref refData) || !BuffHelper.IsPosCanUse(pos, inData.config, inData.mapModels,
                             inData.cdfeMapElementData, inData.cdfeLocalTransform) ||
                         !IsPosDistance(posValues, pos, inData));

                if (isGap)
                {
                    continue;
                }

                //Debug.Log($"pos:{pos},posValuel{posValues.Length},距离:{args789.x}");
                posValues.Add(pos);
                BuffHelper.GenerateMonsterAndAddData(ref refData.ecb, inData.cdfeLocalTransform, inData.cdfeWeaponData,
                    refData.cdfeChaStats, refData.cdfePhysicsMass,
                    inData.cdfeAgentLocomotion, inData.cdfeEnemyData,
                    inData.config,
                    inData.prefabMapData,
                    inData.gameTimeData, inData.gameOthersData, inData.sortKey,
                    args123.x, parentChaStats.chaProperty.defaultAtk, parentChaStats.chaProperty.defaultMaxHp,
                    args456.x, pos);
            }
        }

        bool IsInBossMap(float3 pos, float scale, PlayerOtherData playerOtherData, GameEventData_ReadOnly inData,
            ref GameEventData_ReadWrite refData)
        {
            if (playerOtherData.isBossFight)
            {
                float3 bossScenePos = playerOtherData.bossScenePos;
                var size = bossScenePos.x;
                var bossPos = bossScenePos.yz;
                Rect rectNew = new Rect(pos.x - scale * 0.5f, pos.y - scale * 0.5f, scale, scale);

                if (scale < 1f)
                {
                    scale = math.max(10, scale);
                    rectNew = new Rect(pos.x - scale * 0.5f, pos.y - scale * 0.5f, scale, scale);
                }
                var walkScene = new Rect(bossPos.x - size * 0.5f, bossPos.y - size * 0.5f, size, size);

                if (!BuffHelper.IsFullyOverlapping(walkScene, rectNew))
                {
                    return false;
                }

                return true;
            }

            return false;
        }


        private float GetItemScale(GameEventData_ReadOnly inData, ref GameEventData_ReadWrite refData, int monsterId)
        {
            ref var monsterTable = ref inData.config.value.Value.configTbmonsters.configTbmonsters;
            int monsterIndex = 0;
            for (int i = 0; i < monsterTable.Length; i++)
            {
                if (monsterTable[i].id == monsterId)
                {
                    monsterIndex = i;
                    break;
                }
            }

            ref var monster = ref monsterTable[monsterIndex];

            ref var monsterAttrTable = ref inData.config.value.Value.configTbmonster_attrs.configTbmonster_attrs;
            int monsterAttrIndex = 0;
            for (int i = 0; i < monsterAttrTable.Length; i++)
            {
                if (monsterAttrTable[i].id == monster.monsterAttrId)
                {
                    monsterAttrIndex = i;
                    break;
                }
            }

            ref var monsterAttr = ref monsterAttrTable[monsterAttrIndex];

            ref var monster_models =
                ref inData.config.value.Value.configTbmonster_models.configTbmonster_models;

            int monster_modelsIndex = 0;
            for (int m = 0; m < monster_models.Length; m++)
            {
                if (monsterAttr.bookId == monster_models[m].id)
                {
                    monster_modelsIndex = m;
                    break;
                }
            }

            ref var monsterModel = ref monster_models[monster_modelsIndex];

            if (!inData.prefabMapData.prefabHashMap.TryGetValue(monsterModel.model, out var monsterPrefab))
            {
                Debug.LogError($"{monsterModel.model} 找不到预制件");
                return -1;
            }

            return inData.cdfeLocalTransform[monsterPrefab].Scale;
        }

        private bool IsPosDistance(NativeList<float3> posValues, float3 pos, GameEventData_ReadOnly inData)
        {
            var size = inData.playerData.playerOtherData.bossScenePos.x;
            float3 bossPos = new float3(inData.playerData.playerOtherData.bossScenePos.yz, 0);
            if (pos.x > (bossPos.x + size) || pos.x < (bossPos.x - size) || pos.y > (bossPos.y + size) ||
                pos.y < (bossPos.y - size))
            {
                return false;
            }


            for (int i = 0; i < posValues.Length; i++)
            {
                if (math.distance(pos, posValues[i]) <= args789.x / 1000f)
                {
                    return false;
                }
            }

            return true;
        }

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            GenerateItem(ref refData, inData);
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            GenerateItem(ref refData, inData);
        }

        public void OnTick0(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }
    }
}