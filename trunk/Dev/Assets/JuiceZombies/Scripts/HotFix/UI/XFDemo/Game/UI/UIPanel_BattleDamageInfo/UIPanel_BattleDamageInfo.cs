//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BattleDamageInfo)]
    internal sealed class UIPanel_BattleDamageInfoEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_BattleDamageInfo;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BattleDamageInfo>();
        }
    }

    public partial class UIPanel_BattleDamageInfo : UI, IAwake
    {
        private EntityQuery playerQuery;
        private EntityManager entityManager;
        private Tblanguage tblanguage;
        private Tbequip_data tbequip_data;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            InitJson();
            InitNode();
            JiYuTweenHelper.SetScaleWithBounce(GetFromReference(UIPanel_BattleDamageInfo.KMid));
        }

        async void InitNode()
        {
            var KText_DamageInfo = GetFromReference(UIPanel_BattleDamageInfo.KText_DamageInfo);
            var KContainerPos = GetFromReference(UIPanel_BattleDamageInfo.KContainerPos);
            var KBtn_Mask = GetFromReference(UIPanel_BattleDamageInfo.KBtn_Mask);


            KText_DamageInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_title").current);

            var playerData = playerQuery.ToComponentDataArray<PlayerData>(Allocator.Temp)[0];


            var list = KContainerPos.GetList();
            list.Clear();
            var weaponId = ResourcesSingleton.Instance.playerProperty.playerData.playerOtherData.weaponId;
            //var weaponQuality = ResourcesSingleton.Instance.playerProperty.playerData.playerOtherData.weaponQuality;

            var equip_data = tbequip_data.Get(weaponId);
            var sumDamage = playerData.playerOtherData.playerDamageSumInfo.weaponDamage +
                            playerData.playerOtherData.playerDamageSumInfo.collideDamage +
                            playerData.playerOtherData.playerDamageSumInfo.areaDamage;
            sumDamage = Mathf.Max(1, sumDamage);
            int count = 3;
            for (int i = 0; i < count; i++)
            {
                var index = i;
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UISubPanel_DamageInfoItem, index, false) as
                        UISubPanel_DamageInfoItem;
                var KImg_WeaponIcon = ui.GetFromReference(UISubPanel_DamageInfoItem.KImg_WeaponIcon);
                var KText_Name = ui.GetFromReference(UISubPanel_DamageInfoItem.KText_Name);
                var KText_Num = ui.GetFromReference(UISubPanel_DamageInfoItem.KText_Num);
                var KImg_Filled = ui.GetFromReference(UISubPanel_DamageInfoItem.KImg_Filled);
                var KText_Ratios = ui.GetFromReference(UISubPanel_DamageInfoItem.KText_Ratios);

                float ratios = default;
                switch (index)
                {
                    case 0:

                        ratios = playerData.playerOtherData.playerDamageSumInfo.weaponDamage / sumDamage;
                        KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_name_1").current);
                        KImg_WeaponIcon.GetImage().SetSpriteAsync(equip_data.icon, false);
                        KText_Num.GetTextMeshPro()
                            .SetTMPText(playerData.playerOtherData.playerDamageSumInfo.weaponDamage.ToString());
                        KImg_Filled.GetImage()
                            .SetFillAmount(ratios);
                        KText_Ratios.GetTextMeshPro()
                            .SetTMPText($"{Mathf.FloorToInt(ratios * 100)}%");
                        break;
                    case 1:
                        ratios = playerData.playerOtherData.playerDamageSumInfo.collideDamage / sumDamage;
                        KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_name_2").current);
                        KImg_WeaponIcon.GetImage().SetSpriteAsync("pic_collide_damage", false);
                        KText_Num.GetTextMeshPro()
                            .SetTMPText(playerData.playerOtherData.playerDamageSumInfo.collideDamage.ToString());
                        KImg_Filled.GetImage()
                            .SetFillAmount(ratios);
                        KText_Ratios.GetTextMeshPro()
                            .SetTMPText($"{Mathf.FloorToInt(ratios * 100)}%");
                        break;
                    case 2:
                        ratios = playerData.playerOtherData.playerDamageSumInfo.areaDamage / sumDamage;
                        KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_name_3").current);
                        KImg_WeaponIcon.GetImage().SetSpriteAsync("pic_area_damage", false);
                        KText_Num.GetTextMeshPro()
                            .SetTMPText(playerData.playerOtherData.playerDamageSumInfo.areaDamage.ToString());
                        KImg_Filled.GetImage()
                            .SetFillAmount(ratios);
                        KText_Ratios.GetTextMeshPro()
                            .SetTMPText($"{Mathf.FloorToInt(ratios * 100)}%");

                        break;
                }

                if (ratios >= 0 && ratios < 1f / count)
                {
                    KImg_Filled.GetImage().SetColor("2E88F6");
                }
                else if (ratios >= 1f / count && ratios < (1f / count) * 2)
                {
                    KImg_Filled.GetImage().SetColor("7167FF");
                }
                else
                {
                    KImg_Filled.GetImage().SetColor("EF4444");
                }
            }

            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_DamageInfoItem;
                var uib = b as UISubPanel_DamageInfoItem;
                return uia.index.CompareTo(uib.index);
            });


            KBtn_Mask.GetButton().OnClick.Add(async() => {
                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_BattleDamageInfo.KMid));
                await UniTask.Delay(200,true);
                Close(); });
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbequip_data = ConfigManager.Instance.Tables.Tbequip_data;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}