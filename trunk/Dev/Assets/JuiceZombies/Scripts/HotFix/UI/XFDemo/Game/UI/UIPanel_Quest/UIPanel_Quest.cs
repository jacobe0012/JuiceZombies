//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Google.Protobuf;
using HotFix_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIPanel_Quest)]
    internal sealed class UIPanel_QuestEvent : AUIEvent, IUILayer
    {
	    public override string Key => UIPathSet.UIPanel_Quest;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => true;
		
		public UILayer Layer => UILayer.Low;
		
        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Quest>();
        }
    }

    public partial class UIPanel_Quest : UI, IAwake
	{
        private Tblanguage tbLanguage;

        public async void Initialize()
		{
			await JiYuUIHelper.InitBlur(this);
			 InitNode();
		     InitText();
			 InitBtn();
		}

        private void InitBtn()
        {
			JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtnClose), Close);
			this.GetButton(KCloseMask)?.OnClick.Add(Close);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KBtn_Submit), OnSubmitBtnClick);

        }

        private void OnSubmitBtnClick()
        {
            var KTextInput = GetFromReference(UIPanel_Quest.KTextInput);
            var str = KTextInput.GetComponent<TMP_InputField>().text;
            Log.Debug(str);
            NetWorkManager.Instance.SendMessage(CMD.SUBMITQUEST, new StringValue {
                Value =str,
            });
            WebMessageHandlerOld.Instance.AddHandler(CMD.SUBMITQUEST, OnResponceQuest);


        }

        private void OnResponceQuest(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.SUBMITQUEST, OnResponceQuest);

            var submitInfo = new BoolValue();
            submitInfo.MergeFrom(e.data);
            Log.Debug($"submitInfo:{submitInfo}");
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }
            string errorStr = tbLanguage.Get("text_submit_success").current;
            JiYuUIHelper.ClearCommonResource();
            UIHelper.CreateAsync(UIType.UICommon_Resource, errorStr);
            Close();


        }

        private void InitText()
        {
			var KText_Title = GetFromReference(UIPanel_Quest.KText_Title);
            var KText_Btn = GetFromReference(UIPanel_Quest.KText_Btn);
            var KPlaceholder = GetFromReference(UIPanel_Quest.KPlaceholder);
            KText_Title.GetTextMeshPro().SetTMPText(tbLanguage.Get("setting_feedback_name").current);
            KText_Btn.GetTextMeshPro().SetTMPText(tbLanguage.Get("common_state_submit").current);
            KPlaceholder.GetTextMeshPro().SetTMPText(tbLanguage.Get("setting_feedback_text").current);
        }

        void InitNode()
		{
			var KText_Title = GetFromReference(UIPanel_Quest.KText_Title);
			var KText_Input = GetFromReference(UIPanel_Quest.KTextInput);
			var KBtn_Submit = GetFromReference(UIPanel_Quest.KBtn_Submit);
			var KText_Btn = GetFromReference(UIPanel_Quest.KText_Btn);
			tbLanguage= ConfigManager.Instance.Tables.Tblanguage;
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
