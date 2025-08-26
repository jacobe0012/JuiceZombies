//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Common;
using HotFix_UI;
using SuperScrollView;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UILevelScrollView)]
    internal sealed class UILevelScrollViewEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UILevelScrollView;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UILevelScrollView>();
        }
    }

    public partial class UILevelScrollView : UI, IAwake
    {
        public delegate void ChangeMissionSpriteHandle(int chapterID = 0);

        public static event ChangeMissionSpriteHandle ChangeEvent;
        bool isGetButton;

        public void Initialize()
        {
            LevelScrollScript.SelectChangeEvent += OnSelectButtonClicked;
            isGetButton = false;
            this.GetButton(KBackButton)?.OnClick.Add(Close);
        }


        private void OnSelectButtonClicked(int chapterID)
        {
            var chapterMap = ConfigManager.Instance.Tables.Tbchapter;
            Log.Debug($"��ǰѡ����½�Ϊ:{chapterID}", Color.black);

            ResourcesSingletonOld.Instance.levelInfo.levelId = chapterMap.Get(chapterID).levelId;

            NetWorkManager.Instance.SendMessage(1, 7, new IntValue { Value = ResourcesSingletonOld.Instance.levelInfo.levelId });
            Log.Debug("�ر�", Color.black);
            ChangeEvent(ResourcesSingletonOld.Instance.levelInfo.levelId);
            base.Close();
        }

        protected override void OnClose()
        {
            LevelScrollScript.SelectChangeEvent -= OnSelectButtonClicked;
            Debug.Log("close");
            base.OnClose();
        }
    }
}