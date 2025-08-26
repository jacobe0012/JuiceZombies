//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIRapidCompoundResult)]
    internal sealed class UIRapidCompoundResultEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIRapidCompoundResult;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIRapidCompoundResult>();
        }
    }

    public partial class UIRapidCompoundResult : UI, IAwake
    {
        public void Initialize()
        {
            this.GetFromReference(KCloseText).SetActive(true);
           

            this.GetButton(KMask)?.OnClick.AddListener(OnClose);
        }

        protected override void OnClose()
        {
            base.OnClose();

            //刷新装备页面,也就是这里需要注册一下事件
            InitEquipPanel?.Invoke();
            //刷新一下资产
            //WebMessageHandlerOld.Instance.AddHandler(CMDOld.INITPLAYER, OnOpenMainPanelResponse);
            //NetWorkManager.Instance.SendMessage(CMDOld.INITPLAYER);


            Close();

            //产生新页面
            //UIHelper.Create(UIType.UICompound, UILayer.Mid);
        }

        // async void OnOpenMainPanelResponse(object sender, WebMessageHandlerOld.Execute e)
        // {
        //     var gameRole = new GameRole();
        //     gameRole.MergeFrom(e.data);
        //     if (e.data.IsEmpty)
        //     {
        //         Log.Debug("e.data.IsEmpty", Color.red);
        //         return;
        //     }
        //
        //     var tbuserLevel = ConfigManager.Instance.Tables.Tbuser_level;
        //     ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy = gameRole.RoleAssets.Energy;
        //     ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin = gameRole.RoleAssets.Bitcoin;
        //     ResourcesSingletonOld.Instance.UserInfo.RoleAssets.UsBill = gameRole.RoleAssets.UsBill;
        //     var totalExp = gameRole.RoleAssets.Level > 1
        //         ? tbuserLevel.Get(gameRole.RoleAssets.Level - 1).exp + gameRole.RoleAssets.Exp
        //         : gameRole.RoleAssets.Exp;
        //     ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Exp = totalExp;
        //
        //     ResourcesSingletonOld.Instance.UpdateResourceUI();
        //
        //     //var uiType = await UIHelper.CreateAsync(UIType.UIPanel_JiyuGame, gameRole) as UIPanel_JiyuGame;
        //
        //     //ResourcesSingletonOld.Instance.SetJiYuGamePanel(uiType);
        //
        //
        //     Log.Debug($"{gameRole}", Color.red);
        //
        //     WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.INITPLAYER, OnOpenMainPanelResponse);
        // }
        //

        //刷新装备面板
        public delegate void EquipPanelEventHandler2();

        public static event EquipPanelEventHandler2 InitEquipPanel;
    }
}