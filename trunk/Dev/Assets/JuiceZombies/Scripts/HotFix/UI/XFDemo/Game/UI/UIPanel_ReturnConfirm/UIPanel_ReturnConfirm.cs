//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cfg.config;
using HotFix_UI;
using Main;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using ProjectDawn.Navigation;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_ReturnConfirm)]
    internal sealed class UIPanel_ReturnConfirmEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_ReturnConfirm;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_ReturnConfirm>();
        }
    }

    public partial class UIPanel_ReturnConfirm : UI, IAwake
    {
        private Tblanguage tblanguage;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            InitJson();
            InitNode();
            JiYuTweenHelper.SetScaleWithBounce(GetFromReference(UIPanel_ReturnConfirm.KMid));
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }

        void InitNode()
        {
            //var KText_Title = GetFromReference(UIPanel_ReturnConfirm.KText_Title);
            var KText_Desc = GetFromReference(UIPanel_ReturnConfirm.KText_Desc);
            var KBtn_Close = GetFromReference(UIPanel_ReturnConfirm.KBtn_Close);
            var KText_Close = GetFromReference(UIPanel_ReturnConfirm.KText_Close);
            var KBtn_Cotinue = GetFromReference(UIPanel_ReturnConfirm.KBtn_Cotinue);
            var KText_Continue = GetFromReference(UIPanel_ReturnConfirm.KText_Continue);
            //KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get("level_leave_title").current);
            KText_Desc.GetTextMeshPro().SetTMPText(tblanguage.Get("level_leave_warning_desc").current);
            KText_Close.GetTextMeshPro().SetTMPText(tblanguage.Get("level_leave_button").current);

            KText_Continue.GetTextMeshPro().SetTMPText(tblanguage.Get("level_continue").current);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, async() =>
            {

                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_ReturnConfirm.KMid));
                await UniTask.Delay(200,true);

                JiYuUIHelper.StartStopTime(false);
                JiYuUIHelper.ExitRunTimeScene();

                // var sceneController = Common.Instance.Get<SceneController>();
                // var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                // SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
                this.Close();
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Cotinue, async() => {
                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UIPanel_ReturnConfirm.KMid));
                await UniTask.Delay(200, true);
                Close(); });
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}