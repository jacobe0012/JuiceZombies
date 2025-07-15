//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2024-02-18 18:15:48
//---------------------------------------------------------------------

using Spine.Unity;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

namespace Main
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class SpineHybridUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<WorldBlackBoardTag>();
            RequireForUpdate<SpineHybridData>();
        }

        protected override void OnUpdate()
        {
            var wbe = SystemAPI.GetSingletonEntity<GameTimeData>();
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);

            //set player animation by state
            foreach (var (stateMachine, playerCom, trans, flip, e) in SystemAPI
                         .Query<RefRW<StateMachine>, RefRW<SpineHybridData>, RefRO<LocalTransform>,
                             RefRO<FlipData>>()
                         .WithEntityAccess())
            {
                // var transform = playerCom.go.Value.transform.transform;
                //
                // if (!float.IsNaN(trans.ValueRO.Position.x) && !float.IsNaN(trans.ValueRO.Position.y) &&
                //     !float.IsNaN(trans.ValueRO.Position.z))
                // {
                //     transform.position = trans.ValueRO.Position;
                //
                //     transform.localScale = new Vector3(flip.ValueRO.value.x < 0 ? -1 : 1, 1, 1);
                // }
                //playerCom.ValueRW.skeletonAnimation.Value.
                //playerCom.ValueRW.skeletonAnimation.Value.initialFlipX = flip.ValueRO.value.x < 0;
                //playerCom.ValueRW.skeletonAnimation.Value.Skeleton.ScaleX = flip.ValueRO.value.x < 0 ? -1 : 1;
                //flipa
                // var scale = playerCom.ValueRW.skeletonAnimation.Value.transform.localScale;
                // scale.x *= flip.ValueRO.value.x < 0 ? -1 : 1;.
                if (playerCom.ValueRW.skeletonAnimation == null)
                {
                    continue;
                }

                var transform = playerCom.ValueRW.go.Value.transform;
                if (flip.ValueRO.value.x < 0.5f)
                {
                    playerCom.ValueRW.skeletonAnimation.Value.skeleton.ScaleX = 1;

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        var trail = transform.GetChild(i);
                        trail.localRotation = Quaternion.Euler(0, 0, 0);
                        //Debug.Log($"trail.name1{trail.name} {trail.localRotation}");
                    }
                }
                else
                {
                    playerCom.ValueRW.skeletonAnimation.Value.skeleton.ScaleX = -1;
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        var trail = transform.GetChild(i);
                        trail.localRotation = Quaternion.Euler(0, 180, 0);
                        //Debug.Log($"trail.name{trail.name} {trail.localRotation}");
                    }
                }


                //playerCom.ValueRW.skeletonAnimation.Value.transform.localScale = scale;
                playerCom.ValueRW.skeletonAnimation.Value.loop = true;
                // Debug.Log(
                //     $"skeletonAnimation{playerCom.ValueRW.skeletonAnimation.Value.name} {stateMachine.ValueRW.curAnim}");
                switch ((int)stateMachine.ValueRW.curAnim)
                {
                    case 0:
                        playerCom.ValueRW.skeletonAnimation.Value.AnimationName =
                            $"run";
                        break;
                    case 1:
                        playerCom.ValueRW.skeletonAnimation.Value.AnimationName =
                            $"idle";
                        break;
                    case 2:
                        playerCom.ValueRW.skeletonAnimation.Value.AnimationName = $"getHit";
                        break;
                    case 3:
                        playerCom.ValueRW.skeletonAnimation.Value.loop = false;
                        playerCom.ValueRW.skeletonAnimation.Value.AnimationName =
                            $"die";
                        break;
                    default:

                        var str = $"{stateMachine.ValueRW.animStr}";
                        bool isLoop = str.Contains($"_loop");
                        if (isLoop)
                        {
                            str = str.Replace("_loop", "");
                        }

                        playerCom.ValueRW.skeletonAnimation.Value.loop = isLoop;
                        playerCom.ValueRW.skeletonAnimation.Value.AnimationName = str;
                        // playerCom.ValueRW.skeletonAnimation.Value.AnimationName =
                        //     $"skill{(int)stateMachine.ValueRW.curAnim - 3}";
                        break;
                }

                // if (stateMachine.ValueRW.curAnim == AnimationEnum.Die)
                // {
                //     playerCom.ValueRW.skeletonAnimation.Value.loop = false;
                // }


                // playerCom.ValueRW.skeletonAnimation.Value.AnimationName =
                //     $"die";
                playerCom.ValueRW.skeletonAnimation.Value.timeScale = stateMachine.ValueRW.animSpeedScale *
                                                                      gameTimeData.logicTime.gameTimeScale;
                //Debug.Log($"AnimationName{playerCom.ValueRW.skeletonAnimation.Value.AnimationName}");
                stateMachine.ValueRW.lastAnim = stateMachine.ValueRW.curAnim;


                // playerCom.ValueRW.skeletonAnimation.Value.loop = true;
                // if (stateMachine.ValueRW.curAnim == AnimationEnum.Die)
                // {
                //     playerCom.ValueRW.skeletonAnimation.Value.loop = false;
                // }
                //
                // playerCom.ValueRW.skeletonAnimation.Value.AnimationName =
                //     $"skill{(int)stateMachine.ValueRW.curAnim - 3}";
                // playerCom.ValueRW.skeletonAnimation.Value.timeScale = stateMachine.ValueRW.animSpeedScale *
                //                                                       gameTimeData.logicTime.gameTimeScale;
                // Debug.Log($"AnimationName{playerCom.ValueRW.skeletonAnimation.Value.AnimationName}");
                // stateMachine.ValueRW.lastAnim = stateMachine.ValueRW.curAnim;
                // var spineAnimation=boss.GetComponent<SkeletonAnimation>();
                // spineAnimation.loop = true;
                // spineAnimation.AnimationName = "die";

                // if (stateMachine.ValueRO.currentState.Int32_0 == Animator.StringToHash("PlayerIdle"))
                // {
                //     playerCom.skeletonAnimation.AnimationName = "Player_Stand";
                //     playerCom.skeletonAnimation.loop = true;
                // }
                // else if (stateMachine.ValueRO.currentState.Int32_0 == Animator.StringToHash("PlayerMove"))
                // {
                //     playerCom.skeletonAnimation.AnimationName = "Player_Walk_Left";
                //     playerCom.skeletonAnimation.loop = true;
                // }
                // else if (stateMachine.ValueRO.currentState.Int32_0 == Animator.StringToHash("PlayerGetHit"))
                // {
                //     playerCom.skeletonAnimation.AnimationName = "Player_Hurt_Force";
                //     playerCom.skeletonAnimation.loop = false;
                // }
                // else if (stateMachine.ValueRO.currentState.Int32_0 == Animator.StringToHash("PlayerDie"))
                // {
                //     playerCom.skeletonAnimation.AnimationName = "Player_Dying";
                //     playerCom.skeletonAnimation.loop = false;
                // }
            }

            //set boss animation by state
            // foreach (var (transform, bossCom, bossinfo, Enemyid, sortingValue, stateMachine) in SystemAPI
            //              .Query<RefRO<LocalTransform>, BossHybridData, ChaStats, EnemyData, RefRO<SortingValue>,
            //                  RefRO<StateMachine>>())
            // {
            //     var offset = new float3(0, 4.2f, 0);
            //
            //     if (!float.IsNaN(transform.ValueRO.Position.x) && !float.IsNaN(transform.ValueRO.Position.y) &&
            //         !float.IsNaN(transform.ValueRO.Position.z))
            //     {
            //         bossCom.player.transform.position = transform.ValueRO.Position + offset;
            //         //bossCom.player.transform.localScale = new Vector3(flip.ValueRO.Value.x < 0 ? -1 : 1, 1, 1);
            //     }
            //
            //     int bossID = Enemyid.enemyID;
            //     //boss的技能数组
            //     //NativeList<int> bossSkills = new NativeList<int>(Allocator.Temp);
            //
            //     // for (int i = 0; i < mosterIDarray.Length; i++)
            //     // {
            //     //     //找到对应的bossid
            //     //     if (bossID.Equals(mosterIDarray[i].id))
            //     //     {
            //     //         //找到boos有多少个技能,并且保存boss技能ID
            //     //         for (int j = 0; j < mosterIDarray[i].skill.Length; j++)
            //     //         {
            //     //             bossSkills.Add(mosterIDarray[i].skill[j].y);
            //     //         }
            //     //     }
            //     // }
            //
            //
            //     //如果还有通用状态的话可以继续添加
            //     if (stateMachine.ValueRO.currentState.Int32_0 ==
            //         Animator.StringToHash("Boss_" + bossID.ToString() + "_Stand_S"))
            //     {
            //         bossCom.skeletonAnimation.AnimationName = "Boss_" + bossID.ToString() + "_Stand";
            //     }
            //     else if (stateMachine.ValueRO.currentState.Int32_0 ==
            //              Animator.StringToHash("Boss_" + bossID.ToString() + "_Walk_Left_S"))
            //     {
            //         bossCom.skeletonAnimation.AnimationName = "Boss_" + bossID.ToString() + "_Walk_Left";
            //     }
            //     else if (stateMachine.ValueRO.currentState.Int32_0 ==
            //              Animator.StringToHash("Boss_" + bossID.ToString() + "_Hurt_Force_S"))
            //     {
            //         bossCom.skeletonAnimation.AnimationName = "Boss_" + bossID.ToString() + "_Hurt_Force";
            //     }
            //     else if (stateMachine.ValueRO.currentState.Int32_0 ==
            //              Animator.StringToHash("Boss_" + bossID.ToString() + "_Dying_S"))
            //     {
            //         bossCom.skeletonAnimation.AnimationName = "Boss_" + bossID.ToString() + "_Dying";
            //     }
            //
            //     //boss现在是哪一个技能那就是哪个状态
            //     // for (int i = 0; i < bossSkills.Length; i++)
            //     // {
            //     //     if (stateMachine.ValueRO.currentState.Int32_0 ==
            //     //         Animator.StringToHash("Boss_" + bossID.ToString() + "_" + bossSkills[i].ToString() + "_S"))
            //     //     {
            //     //         bossCom.skeletonAnimation.AnimationName =
            //     //             "Boss_" + bossID.ToString() + "_" + bossSkills[i].ToString();
            //     //     }
            //     // }
            // }
        }
    }
}