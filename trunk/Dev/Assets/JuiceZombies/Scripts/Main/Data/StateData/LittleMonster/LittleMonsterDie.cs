using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    public partial struct LittleMonsterDie : IState
    {
        ///<summary>0
        ///动画ID
        ///</summary>
        public int stateId;

        ///<summary>1
        ///存在了多少时间了，单位：秒
        ///</summary>
        public float timeElapsed;

        ///<summary>2
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float timeScale;

        ///<summary>3
        ///剩余多少时间，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///持续多少帧了，单位：帧
        ///</summary>
        public int tick;

        ///<summary>5
        ///是否是一次性播放动画
        ///</summary>
        public bool isOneShotAnim;

        public float hitTime;

        public float a;
        public float minDeathTime;

        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.stateMachine.curAnim = AnimationEnum.Die_lm;
            refData.chaStats.chaResource.isDead = true;
            // if (inData.cdfeLocalTransform[inData.entity].Position.x <
            //     inData.cdfeLocalTransform[inData.player].Position.x)
            // {
            //     refData.flip.Value.x = -1;
            // }
            // else
            // {
            //     refData.flip.Value.x = 0;
            // }
            var type = refData.enemyData.type;
            var id = refData.enemyData.enemyID;
            var playerData = refData.cdfePlayerData[inData.player];
            if (!playerData.playerOtherData.killMonsterIdList.Contains(id))
            {
                playerData.playerOtherData.killMonsterIdList.Add(id);
            }

            switch (type)
            {
                case 8:
                    playerData.playerOtherData.killLittleMonster++;
                    break;
                case 16:
                    playerData.playerOtherData.killElite++;
                    break;
                case 32:
                    playerData.playerOtherData.killBoss++;
                    playerData.playerOtherData.killBossIdList.Add(id);
                    Debug.LogError($"killBoss:{playerData.playerOtherData.killBoss}");
                    break;
            }

            playerData.playerData.killEnemy++;
            refData.cdfePlayerData[inData.player] = playerData;

            refData.agentBody.Stop();
            ref var config = ref inData.configData.value.Value.configTbbattle_constants.configTbbattle_constants;
            ref var environment = ref inData.configData.value.Value.configTbenvironments.Get(103);

            int battle_force_factor = 0;
            int battle_monster_restore_time_min = 0;
            int battle_monster_restore_unit = 0;

            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].constantName == (FixedString128Bytes)"battle_force_factor")
                {
                    battle_force_factor = config[i].constantValue;
                }

                if (config[i].constantName == (FixedString128Bytes)"battle_monster_restore_time_min")
                {
                    battle_monster_restore_time_min = config[i].constantValue;
                }

                if (config[i].constantName == (FixedString128Bytes)"battle_monster_restore_unit")
                {
                    battle_monster_restore_unit = config[i].constantValue;
                }
            }

            var chaStats = inData.cdfeChaStats[inData.entity];
            chaStats.chaProperty.mass = math.max(chaStats.chaProperty.mass, 1);

            refData.chaStats.chaResource.curMoveSpeed = math.length(refData.physicsVelocity.Linear) * 1000f;
            refData.chaStats.chaResource.direction = math.normalizesafe(refData.physicsVelocity.Linear);

            //预计僵直时间 = 速度/推力修正系数/m(角色)*角色的 每单位速度硬直时间 ）+ 基础硬直时间
            hitTime = (refData.chaStats.chaResource.curMoveSpeed) / (battle_force_factor / 10000f) /
                chaStats.chaProperty.mass *
                (battle_monster_restore_unit / 1000f) + battle_monster_restore_time_min / 1000f;


            a = (refData.chaStats.chaResource.curMoveSpeed / 1000f) / hitTime;
            float physicsDamping = (2 * (a / 100f)) / hitTime;
            refData.physicsDamping.Linear = physicsDamping;

            minDeathTime = 2f;
            //Debug.Log($"speedRecoveryTime{refData.chaStats.chaProperty.speedRecoveryTime}a{a} time{hitTime} physicsDamping{physicsDamping}");
            Debug.Log($"monster hitTime:{hitTime}  a:{a} speed:{refData.chaStats.chaResource.curMoveSpeed}");

            // if (inData.gameOthersData.enableTest1)
            // {
            //     return;
            // }
            //dir = math.normalizesafesafe(refData.localTransform.Position -inData.cdfeLocalTransform[inData.playerEntity].Position);

            //refData.agentBody.Stop();
            // refData.physicsVelocity = new PhysicsVelocity
            // {
            //     Linear = float3.zero,
            //     Angular = float3.zero
            // };

            //refData.physicsCollider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.None);

            //refData.agentShape.Radius = -1;


            ref var configTbbattle_drops = ref inData.configData.value.Value.configTbbattle_drops.configTbbattle_drops;
            ref var configTbbattle_items = ref inData.configData.value.Value.configTbbattle_items.configTbbattle_items;

            var random =
                Unity.Mathematics.Random.CreateFromIndex((uint)(inData.gameRandomData.seed + inData.entity.Index +
                                                                inData.entity.Version +
                                                                inData.sortkey + inData.timeTick));

            var list = new NativeList<DropsBuffer>(Allocator.Temp);
            if (inData.bfeGameEvent.HasBuffer(inData.wbe))
            {
                var wbeEvents = inData.bfeGameEvent[inData.wbe];
                foreach (var gameEvent in wbeEvents)
                {
                    if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_24 &&
                        gameEvent.int3_7.x == refData.enemyData.type)
                    {
                        list.Add(new DropsBuffer
                        {
                            id = gameEvent.int3_7.y
                        });
                    }
                }
            }

            list.AddRange(inData.dropsBuffer.ToNativeArray(Allocator.Temp));
            bool isMulitDrop = list.Length >= 2;


            foreach (var drop in list)
            {
                bool packIndYn = false;

                for (int i = 0; i < configTbbattle_drops.Length; i++)
                {
                    if (configTbbattle_drops[i].id == drop.id)
                    {
                        if (configTbbattle_drops[i].packIndYn == 1)
                        {
                            packIndYn = true;
                        }
                        else
                        {
                            packIndYn = false;
                        }

                        break;
                    }
                }

                if (packIndYn)
                {
                    for (int i = 0; i < configTbbattle_drops.Length; i++)
                    {
                        if (configTbbattle_drops[i].id == drop.id)
                        {
                            var dropRate = configTbbattle_drops[i].packPower / 10000f;
                            var isDrop = random.NextFloat(0, 100) < dropRate;

                            if (!isDrop) break;

                            for (int j = 0; j < configTbbattle_drops[i].reward.Length; j++)
                            {
                                var reward = configTbbattle_drops[i].reward[j];
                                for (int k = 0; k < reward.y; k++)
                                {
                                    FixedString128Bytes propBattlePrefab = default;
                                    for (int l = 0; l < configTbbattle_items.Length; l++)
                                    {
                                        if (configTbbattle_items[l].id == reward.x)
                                        {
                                            propBattlePrefab = (FixedString128Bytes)configTbbattle_items[l].model;
                                            break;
                                        }
                                    }

                                    //Debug.LogError($"{propBattlePrefab} {reward.x}");
                                    var prefab = inData.prefabMapData.prefabHashMap[propBattlePrefab];
                                    //var prefab = inData.prefabMapData.prefabHashMap[propBattlePrefab];
                                    var ins = refData.ecb.Instantiate(inData.sortkey, prefab);
                                    float3 point = refData.localTransform.Position;
                                    if (isMulitDrop)
                                    {
                                        // 随机选择一个角度
                                        float angle = (float)(random.NextFloat() * 2 * math.PI); // 0到2π的随机数

                                        // 随机选择一个半径，范围在小圆到大圆之间
                                        float radius = (float)(random.NextFloat() * UnityHelper.DropRadius);

                                        // 使用极坐标转化为笛卡尔坐标
                                        point.x = point.x + radius * math.cos(angle);
                                        point.y = point.y + radius * math.sin(angle);
                                    }

                                    var newpos = new LocalTransform
                                    {
                                        Position = point,
                                        Scale = inData.cdfeLocalTransform[prefab].Scale,
                                        Rotation = inData.cdfeLocalTransform[prefab].Rotation
                                    };
                                    //refData.ecb.SetComponent(inData.sortkey, ins, newpos);

                                    var dropsData = new DropsData
                                    {
                                        point0 = newpos.Position,
                                        point1 = default,
                                        point2 = new float3(newpos.Position.x,
                                            newpos.Position.y, 0),
                                        point3 = default,
                                        isLooting = false,
                                        lootingAniDuration = inData.gameOthersData.pickupDuration / 1000f,
                                        id = reward.x,
                                        dropPoint2 = newpos.Position,
                                        dropPoint0 = refData.localTransform.Position,
                                    };

                                    refData.ecb.SetComponent(inData.sortkey, ins, dropsData);
                                }
                            }

                            break;
                        }
                    }
                }
                else
                {
                    //pack_id   pack_power
                    NativeHashMap<int, int> dropPairs = new NativeHashMap<int, int>(5, Allocator.Temp);
                    for (int i = 0; i < configTbbattle_drops.Length; i++)
                    {
                        if (configTbbattle_drops[i].id == drop.id)
                        {
                            dropPairs.TryAdd(configTbbattle_drops[i].packId, configTbbattle_drops[i].packPower);
                        }
                    }

                    var propid = MathHelper.SelectRandomItem(dropPairs, random);
                    if (propid == -1)
                    {
                        Debug.LogError($"选择随机有误");
                        break;
                    }


                    for (int i = 0; i < configTbbattle_drops.Length; i++)
                    {
                        if (configTbbattle_drops[i].packId == propid)
                        {
                            for (int j = 0; j < configTbbattle_drops[i].reward.Length; j++)
                            {
                                var reward = configTbbattle_drops[i].reward[j];
                                for (int k = 0; k < reward.y; k++)
                                {
                                    FixedString128Bytes propBattlePrefab = default;
                                    for (int l = 0; l < configTbbattle_items.Length; l++)
                                    {
                                        if (configTbbattle_items[l].id == reward.x)
                                        {
                                            propBattlePrefab = (FixedString128Bytes)configTbbattle_items[l].model;
                                            break;
                                        }
                                    }

                                    var prefab = inData.prefabMapData.prefabHashMap[propBattlePrefab];
                                    //var prefab = inData.prefabMapData.prefabHashMap[propBattlePrefab];
                                    var ins = refData.ecb.Instantiate(inData.sortkey, prefab);
                                    float3 point = refData.localTransform.Position;
                                    if (isMulitDrop)
                                    {
                                        // 随机选择一个角度
                                        float angle = (float)(random.NextFloat() * 2 * math.PI); // 0到2π的随机数

                                        // 随机选择一个半径，范围在小圆到大圆之间
                                        float radius = (float)(random.NextFloat() * UnityHelper.DropRadius);

                                        // 使用极坐标转化为笛卡尔坐标
                                        point.x = point.x + radius * math.cos(angle);
                                        point.y = point.y + radius * math.sin(angle);
                                    }

                                    var newpos = new LocalTransform
                                    {
                                        Position = point,
                                        Scale = inData.cdfeLocalTransform[prefab].Scale,
                                        Rotation = inData.cdfeLocalTransform[prefab].Rotation
                                    };
                                    //refData.ecb.SetComponent(inData.sortkey, ins, newpos);
                                    var dropsData = new DropsData
                                    {
                                        point0 = newpos.Position,
                                        point1 = default,
                                        point2 = new float3(newpos.Position.x,
                                            newpos.Position.y, 0),
                                        point3 = default,
                                        isLooting = false,
                                        lootingAniDuration = inData.gameOthersData.pickupDuration / 1000f,
                                        id = reward.x,
                                        dropPoint2 = newpos.Position,
                                        dropPoint0 = refData.localTransform.Position,
                                    };

                                    refData.ecb.SetComponent(inData.sortkey, ins, dropsData);
                                }
                            }
                        }
                    }
                }
            }

            var entityGroupData = inData.cdfeEntityGroupData[inData.entity];
            if (refData.storageInfoFromEntity.Exists(entityGroupData.renderingEntity))
            {
                refData.ecb.AddComponent(inData.sortkey, entityGroupData.renderingEntity, new JiYuAlphaTo0()
                {
                    value = UnityHelper.Color2Float4(Color.white)
                });
            }
        }


        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            if (inData.cdfeTimeToDieData.HasComponent(inData.entity))
            {
                return;
            }

            //TODO:是否受重力影响
            //refData.localTransform.Position += 2f * inData.fdT * dir;
            hitTime -= inData.fdT;
            //Debug.Log($"{hitTime}");
            minDeathTime -= inData.fdT;

            if (minDeathTime < 0 && refData.chaStats.chaResource.curMoveSpeed < math.EPSILON &&
                !inData.cdfeTimeToDieData.HasComponent(inData.entity))
            {
                const float BossDeathTime = 10f;
                const float JiyingDeathTime = 5f;
                float deathTime = refData.enemyData.type == 32 ? BossDeathTime : JiyingDeathTime;

                refData.ecb.AddComponent(inData.sortkey, inData.entity, new TimeToDieData
                {
                    duration = deathTime
                });
                if (inData.cdfeEntityGroupData.HasComponent(inData.entity))
                {
                    var entityGroupData = inData.cdfeEntityGroupData[inData.entity];
                    if (refData.storageInfoFromEntity.Exists(entityGroupData.renderingEntity))
                    {
                        refData.ecb.AddComponent(inData.sortkey, entityGroupData.renderingEntity,
                            new JiYuDissolveThreshold());
                    }
                }

                return;
            }

            refData.physicsVelocity.Angular = 0;
            //Debug.Log($"{refData.chaStats.chaResource.curMoveSpeed}");
            refData.chaStats.chaResource.curMoveSpeed -= a * 1000f *
                                                         inData.fdT;


            // refData.physicsVelocity.Linear =
            //     (refData.chaStats.chaResource.curMoveSpeed / 100f) * refData.chaStats.chaResource.direction;
        }
    }
}