//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Share)]
    internal sealed class UIPanel_ShareEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Share;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Share>();
        }
    }

    public partial class UIPanel_Share : UI, IAwake
    {
        #region properties

        private Tblanguage tblanguage;
        private Tbthird_party tbthird_Party;
        private Tbuser_avatar tbuser_Avatar;
        private Tbuser_variable tbuser_Variable;
        private Tbconstant tbconstant;

        //private long shareId = 0;

        //test
        //private int shareTime = 1;

        private bool isInit;
        // private GameObject testCanvas = null;
        // private Transform testCanvasTrasnform = null;

        #endregion

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            InitJson();
            WebInit();
            NetWorkManager.Instance.SendMessage(CMD.QUERYSHARE);
            this.GetFromReference(KBtn_Close).GetXButton().OnClick?.Add(() =>
            {
                //testCanvas.transform.SetParent(testCanvasTrasnform);
                Close();
            });
        }

        void InitNode()
        {
            TextSet();
            CreateItems().Forget();
        }

        void Refresh()
        {
            TextSet();
        }

        private void WebInit()
        {
            WebMessageHandler.Instance.AddHandler(CMD.QUERYSHARE, OnShareInfoResponse);
            WebMessageHandler.Instance.AddHandler(CMD.SETSHARE, OnSetShareResponse);
        }

        private void OnSetShareResponse(object sender, WebMessageHandler.Execute e)
        {
            Log.Debug($"OnShareResponse ", Color.green);
            NetWorkManager.Instance.SendMessage(CMD.QUERYSHARE);
        }

        private void OnShareInfoResponse(object sender, WebMessageHandler.Execute e)
        {
            var shareData = new GameShare();
            shareData.MergeFrom(e.data);
            Log.Debug($"OnShareInfoResponse {shareData}", Color.green);
            //shareData.
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnShareInfoResponse IsEmpty", Color.green);
                return;
            }

            ResourcesSingleton.Instance.gameShare = shareData;
            // shareTime = 0;
            // if (shareData.IsShare == true)
            // {
            //     shareTime = 1;
            // }
            if (ResourcesSingleton.Instance.gameShare.IsShare == null || !ResourcesSingleton.Instance.gameShare.IsShare)
            {
                
            }

            if (!isInit)
            {
                InitNode();
            }
            else
            {
                Refresh();
            }
        }

        private void TextSet()
        {
            var KText_Num = GetFromReference(UIPanel_Share.KText_Num);
            var KText_Received = GetFromReference(UIPanel_Share.KText_Received);
            var KImg_Mid = GetFromReference(UIPanel_Share.KImg_Mid);
            JiYuUIHelper.LoadImage($"share_pic{ResourcesSingleton.Instance.gameShare.Id}.png", KImg_Mid);
            var count = tbconstant.Get("share_reward").constantValue;
            var maxCount = tbconstant.Get("share_reward_limit").constantValue;
            var numStr =
                $"{tblanguage.Get("setting_share_no_text").current} {JiYuUIHelper.GetRewardTextIconName($"icon_diamond")}{count}";
            KText_Num.GetTextMeshPro().SetTMPText(numStr);
            KText_Received.GetTextMeshPro().SetTMPText(tblanguage.Get("setting_share_yes_text").current);
            var shareTime = ResourcesSingleton.Instance.gameShare.IsShare ? 1 : 0;
            if (shareTime < maxCount)
            {
                //can get
                KText_Num.SetActive(true);

                KText_Received.SetActive(false);
            }
            else
            {
                //can not get
                KText_Num.SetActive(false);

                KText_Received.SetActive(true);
            }
        }

        private void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbthird_Party = ConfigManager.Instance.Tables.Tbthird_party;
            tbuser_Avatar = ConfigManager.Instance.Tables.Tbuser_avatar;
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
        }

        private async UniTaskVoid CreateItems()
        {
            var KContainer = GetFromReference(UIPanel_Share.KContainer);
            var list = KContainer.GetList();
            list.Clear();
            var displayList = tbthird_Party.DataList.Where(a => a.displayYn == 1).ToList();
            displayList.Add(new third_party(0, 1, "name", "pic", 0, 1));
            foreach (var item in displayList)
            {
                var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_ShareItem, item.id, false);
                var KImg_IconBtn = ui.GetFromReference(UISubPanel_ShareItem.KImg_IconBtn);
                var KText_IconBtn = ui.GetFromReference(UISubPanel_ShareItem.KText_IconBtn);
                var KImg_RedDot = ui.GetFromReference(UISubPanel_ShareItem.KImg_RedDot);
                KText_IconBtn.GetTextMeshPro().SetTMPText(tblanguage.Get(item.name).current);
                KImg_IconBtn.GetImage().SetSpriteAsync(item.pic, true);

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KImg_IconBtn, () =>
                {
                    if (!ResourcesSingleton.Instance.gameShare.IsShare)
                    {
                        NetWorkManager.Instance.SendMessage(CMD.SETSHARE, new LongValue
                        {
                            Value = ResourcesSingleton.Instance.gameShare.Id
                        });
                    }

                    //SaveTexture();
                    switch (item.id)
                    {
                        default:
                            return;
                    }
                });
            }
        }

        private void SaveTexture()
        {
            int w = 1170;
            int h = 1329;
            var camera = this.GetFromReference(KGameObject).GetComponent<Camera>();
            System.DateTime currentTime = System.DateTime.Now;
            string pngName = currentTime.Year.ToString() + currentTime.Month.ToString() + currentTime.Day.ToString() +
                             currentTime.Hour.ToString() + currentTime.Minute.ToString() +
                             currentTime.Second.ToString() + "Share";
            string path = Application.dataPath + "/Resources/Local/" + pngName + ".png";
            RenderTexture renderTexture = new RenderTexture(w, h, 0);
            camera.targetTexture = renderTexture;
            Texture2D texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            camera.Render();
            texture.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            texture.Apply();
            FileStream fileStream = new System.IO.FileStream(path, FileMode.Create, FileAccess.Write);
            fileStream.Write(texture.EncodeToPNG(), 0, texture.EncodeToPNG().Length);
            fileStream.Dispose();
            fileStream.Close();
            camera.targetTexture = null;
            GameObject.Destroy(renderTexture);
        }

        private void CreateCanvas()
        {
            var camera = this.GetFromReference(KGameObject).GetComponent<Camera>();
            var canvas = this.GetFromReference(KTestCanvas).GetComponent<Canvas>();
            // testCanvas = this.GetFromReference(KTestCanvas).GameObject;
            // testCanvasTrasnform = this.GetFromReference(KTestCanvas).GameObject.transform.parent;
            this.GetFromReference(KTestCanvas).GameObject.transform.SetParent(null);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            this.GetFromReference(KGameObject).SetActive(true);
            canvas.worldCamera = this.GetFromReference(KGameObject).GetComponent<Camera>();
            var shotUI = UIHelper.Create(UIType.UISubPanel_Share_Shot, canvas.gameObject.transform, false);

            shotUI.GetFromReference(UISubPanel_Share_Shot.KText_Name).GetTextMeshPro()
                .SetTMPText(ResourcesSingleton.Instance.UserInfo.Nickname);
            shotUI.GetFromReference(UISubPanel_Share_Shot.KText_ID).GetTextMeshPro().SetTMPText("ID:" +
                ResourcesSingleton.Instance.UserInfo.UserId.ToString());
            shotUI.GetFromReference(UISubPanel_Share_Shot.KImg_Head).GetImage().SetSpriteAsync(
                tbuser_Avatar.Get(ResourcesSingleton.Instance.UserInfo.RoleAvatar).icon, false);
        }

        protected override void OnClose()
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYSHARE, OnShareInfoResponse);
            WebMessageHandler.Instance.RemoveHandler(CMD.SETSHARE, OnSetShareResponse);

            base.OnClose();
        }
    }
}