//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_ToggleItem)]
    internal sealed class UISubPanel_ToggleItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_ToggleItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_ToggleItem>();
        }
    }

    public partial class UISubPanel_ToggleItem : UI, IAwake<int, int>
    {
        private Tbtag tbtag;
        private Tblanguage tblanguage;
        public int sort;
        public int tagId;

        protected override void OnClose()
        {
            base.OnClose();
        }

        public UI GetRedDot()
        {
            return this.GetFromReference(KImg_RedDot);
        }

        public void RefreshLanguage()
        {
            var text = GetFromReference(KText);

            text.GetTextMeshPro().SetTMPText(tblanguage.Get(tbtag.DataList[this.sort - 1].name).current);
        }

        public void Initialize(int id, int sort)
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag = ConfigManager.Instance.Tables.Tbtag;
            this.tagId = id;
            this.sort = sort;
            var KText = GetFromReference(UISubPanel_ToggleItem.KText);
            var KContent = GetFromReference(UISubPanel_ToggleItem.KContent);
            var KImg_Btn = GetFromReference(UISubPanel_ToggleItem.KImg_Btn);
            
            KText.GetTextMeshPro().SetTMPText(tblanguage.Get(tbtag.DataList[this.sort - 1].name).current);
            var icon = this.GetFromReference(KIcon);
            icon.GetImage().SetSpriteAsync(tbtag.DataList[this.sort - 1].icon, false).Forget();
          
            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this);


            var uiBtn = KImg_Btn.GetXButton();
            if (uiBtn == null)
            {
                Log.Error($"{this.Name}.GetXButton() is null");
                return;
            }

            uiBtn.SetPointerActive(true);

            uiBtn.SetLongPressInterval(UnicornTweenHelper.LongPressInterval);

            uiBtn.SetMaxLongPressCount(UnicornTweenHelper.MaxLongPressCount);


            uiBtn.OnClick.Add(() =>
            {
                if (!ResourcesSingletonOld.Instance.settingData.UnlockMap.ContainsKey(this.tagId))
                {
                    return;
                }

                //uiBtn.SetEnabled(false);
                KContent.GetRectTransform().DoScale(new Vector3(0.85f, 1.445f, 1), UnicornTweenHelper.OnClickAnimTime)
                    .AddOnCompleted(() =>
                    {
                        KContent.GetRectTransform().DoScale(new Vector3(1.1f, 1.87f, 1), UnicornTweenHelper.OnClickAnimTime)
                            .AddOnCompleted(() =>
                            {
                                KContent.GetRectTransform().DoScale(new Vector3(1, 1.7f, 1),
                                    UnicornTweenHelper.OnClickAnimTime / 2f);
                                //uiBtn.SetEnabled(true);
                            });
                    });
                KText.GetTextMeshPro().SetTMPText(tblanguage.Get(tbtag.DataList[this.sort - 1].name).current);
            });
            uiBtn.OnLongPress.Add((f) =>
            {
                if (!ResourcesSingletonOld.Instance.settingData.UnlockMap.ContainsKey(this.tagId))
                {
                    return;
                }

                var lastId = this.GetParent<UIPanel_JiyuGame>().GetLastTabId();

                if (lastId == this.sort)
                {
                    KContent.GetRectTransform().DoScale(new Vector3(0.85f, 1.445f, 1), UnicornTweenHelper.OnClickAnimTime);
                }
                else
                {
                    KContent.GetRectTransform().DoScale(new Vector3(0.85f, 0.85f, 1), UnicornTweenHelper.OnClickAnimTime);
                }

                uiBtn.OnPointerExit.Add(() =>
                {
                    var lastId = this.GetParent<UIPanel_JiyuGame>().GetLastTabId();
                    if (lastId == this.sort)
                    {
                        KContent.GetRectTransform().DoScale(new Vector3(1.1f, 1.87f, 1), UnicornTweenHelper.OnClickAnimTime)
                            .AddOnCompleted(
                                () =>
                                {
                                    KContent.GetRectTransform().DoScale(new Vector3(1, 1.7f, 1),
                                        UnicornTweenHelper.OnClickAnimTime / 2f);
                                });
                    }
                    else
                    {
                        KContent.GetRectTransform().DoScale(new Vector3(1.1f, 1.1f, 1), UnicornTweenHelper.OnClickAnimTime)
                            .AddOnCompleted(
                                () =>
                                {
                                    KContent.GetRectTransform().DoScale(new Vector3(1, 1f, 1),
                                        UnicornTweenHelper.OnClickAnimTime / 2f);
                                });
                    }
                });
            });

            uiBtn.OnLongPressEnd.Add((f) =>
            {
                if (!ResourcesSingletonOld.Instance.settingData.UnlockMap.ContainsKey(this.tagId))
                {
                    return;
                }

                var lastId = this.GetParent<UIPanel_JiyuGame>().GetLastTabId();
                if (lastId == this.sort)
                {
                    KContent.GetRectTransform().DoScale(new Vector3(1.1f, 1.87f, 1), UnicornTweenHelper.OnClickAnimTime)
                        .AddOnCompleted(
                            () =>
                            {
                                KContent.GetRectTransform().DoScale(new Vector3(1, 1.7f, 1),
                                    UnicornTweenHelper.OnClickAnimTime / 2f);
                            });
                }
                else
                {
                    KContent.GetRectTransform().DoScale(new Vector3(1.1f, 1.1f, 1), UnicornTweenHelper.OnClickAnimTime)
                        .AddOnCompleted(
                            () =>
                            {
                                KContent.GetRectTransform().DoScale(new Vector3(1, 1f, 1),
                                    UnicornTweenHelper.OnClickAnimTime / 2f);
                            });
                }
            });
            RefreshBtnEnable();
        }

        public void RefreshBtnEnable()
        {
            var icon = this.GetFromReference(KIcon);
            var uiBtn = this.GetXButton();
            //uiBtn.SetEnabled(true);
            icon.GetImage().SetAlpha(1f);
            if (!ResourcesSingletonOld.Instance.settingData.UnlockMap.ContainsKey(this.tagId))
            {
                //uiBtn.SetEnabled(false);
                icon.GetImage().SetAlpha(0.5f);
            }
        }
    }
}