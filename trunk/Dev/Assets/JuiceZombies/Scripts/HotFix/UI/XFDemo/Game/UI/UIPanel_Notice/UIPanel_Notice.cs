//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using cfg.config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Notice)]
    internal sealed class UIPanel_NoticeEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Notice;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Notice>();
        }
    }

    public partial class UIPanel_Notice : UI, IAwake
    {
        private Tblanguage tblanguage;
        private int lastNoticeId;

        private long timerId;

        private CancellationTokenSource cts = new CancellationTokenSource();
        private UIListComponent ContentList;

        private UI lastItem;

        //private List<Notice> nlist = new List<Notice>();
        private Tblanguage.L10N noticeL10N;
        private const string localPathHelp = "/Resources/Local/Notice/";
        public const string Person_Red_Point_Root = "Person_Red_Point_Root";
        private int tagFunc = 5202;
        private string m_RedDotName;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            InitJson();
            InitRedDot();
            StartTimer();
            var KBtn_Bg = GetFromReference(UIPanel_Notice.KBtn_Bg);
            var KBtn_Close = GetFromReference(UIPanel_Notice.KBtn_Close);
            KBtn_Bg.GetXButton().OnClick.Add(async () => { ClosePanel(); });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, async () => { ClosePanel(); });
            await DataInit();
            InitNode();

            // SortNotice();
            // SetNotice(0);
            // CreateNoticeItem().Forget();
        }

        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
        }

        private void InitJson()
        {
            //tbannounce = ConfigManager.Instance.Tables.Tbannounce;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }


        private async UniTask ClosePanel()
        {
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(UIPanel_Notice.KImg_Bg), 0 - 100, 100, cts.Token,
                0.15f, false);
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(UIPanel_Notice.KImg_Bg), 0 - 100, 100, cts.Token,
                0.15f, false);
            GetFromReference(UIPanel_Notice.KImg_Bg).GetComponent<CanvasGroup>().alpha = 1f;
            GetFromReference(UIPanel_Notice.KImg_Bg).GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.InQuad);
            await UniTask.Delay(150, cancellationToken: cts.Token);
            Close();
        }

        private void InitNode()
        {
            var KText_Tile = GetFromReference(UIPanel_Notice.KText_Tile);


            var KImg_Notice = GetFromReference(UIPanel_Notice.KImg_Notice);
            var KText_Name = GetFromReference(UIPanel_Notice.KText_Name);
            var KText_Notice = GetFromReference(UIPanel_Notice.KText_Notice);

            var KScrollView = GetFromReference(UIPanel_Notice.KScrollView);
            var KImg_Empty = GetFromReference(UIPanel_Notice.KImg_Empty);
            var KText_Empty = GetFromReference(UIPanel_Notice.KText_Empty);
            var KText_Close = GetFromReference(UIPanel_Notice.KText_Close);


            var closeStr =
                $"{JiYuUIHelper.GetRewardTextIconName("common_bullet_logo_left")}  {tblanguage.Get("text_window_close").current}  {JiYuUIHelper.GetRewardTextIconName("common_bullet_logo_right")}";
            KText_Close.GetTextMeshPro().SetTMPText(closeStr);


            var content = KScrollView.GetXScrollRect().Content;
            CreateNoticeItem().Forget();
            KText_Tile.GetTextMeshPro().SetTMPText(tblanguage.Get("func_3103_name").current);



        }

        private async UniTask DataInit()
        {
            // ContentList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();
            // nlist.Clear();
            if (JsonManager.Instance.sharedData.noticesList == null)
            {
                await JiYuUIHelper.DownloadNotice();
            }
            // else
            // {
            //     var nlistHelp = JsonManager.Instance.sharedData.noticesList.notices;
            //     foreach (var nlh in nlistHelp)
            //     {
            //         if (nlh.noticeStatus == 1)
            //         {
            //             nlist.Add(nlh);
            //         }
            //     }
            // }

            //noticeL10N = (Tblanguage.L10N)ResourcesSingleton.Instance.settingData.CurrentL10N;
        }


        private void SortNotice()
        {
            // nlist.Sort(delegate(Notice n1, Notice n2)
            // {
            //     if (n1.readStatus < n2.readStatus)
            //     {
            //         return -1;
            //     }
            //     else
            //     {
            //         if (n1.initTime > n2.initTime)
            //         {
            //             return -1;
            //         }
            //         else
            //         {
            //             if (n1.id > n2.id)
            //             {
            //                 return -1;
            //             }
            //             else
            //             {
            //                 return 1;
            //             }
            //         }
            //     }
            // });
        }

        private void SetNotice(int i)
        {
            // if (nlist.Count != 0)
            // {
            //     #region nlist.count != 0
            //
            //     this.GetFromReference(KImg_Empty).SetActive(false);
            //     this.GetFromReference(KBg_Text).SetActive(true);
            //     // SelectNoticePropertyByL10N(nlist[i], out var linkStr, out var picStr, out var titleStr,
            //     //     out var contentStr);
            //     int id = nlist[i].id;
            //     //this.GetFromReference(KText_Name).GetTextMeshPro().SetTMPText(titleStr);
            //     //this.GetFromReference(KText_Notice).GetTextMeshPro().SetTMPText(NoticeTextHelp(nlist[i], contentStr));
            //     float textH = this.GetFromReference(KText_Notice).GetRectTransform().Height();
            //     var scrollViewText = this.GetFromReference(KScrollView_Text);
            //     var scrollViewTextContentRect = scrollViewText.GetScrollRect().Content.GetRectTransform();
            //     scrollViewTextContentRect.SetHeight(textH);
            //     scrollViewTextContentRect.SetOffsetWithLeft(0);
            //     scrollViewTextContentRect.SetOffsetWithRight(0);
            //     scrollViewTextContentRect.SetAnchoredPositionX(0);
            //     if (textH > scrollViewText.GetRectTransform().Height())
            //     {
            //         scrollViewText.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
            //     }
            //     else
            //     {
            //         scrollViewText.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            //     }
            //
            //     //set image
            //     //string picPath = SelectNoticePicByL10N(nlist[i]);
            //    // SetSpriteByLocalPath(picPath, this.GetFromReference(KImg_Notice));
            //
            //     #endregion
            // }
            // else
            // {
            //     this.GetFromReference(KText_Name).GetTextMeshPro().SetTMPText("");
            //     this.GetFromReference(KText_Notice).GetTextMeshPro().SetTMPText("");
            //     this.GetFromReference(KBg_Text).SetActive(false);
            //     this.GetFromReference(KImg_Empty).SetActive(true);
            // }
        }

        // private void SelectNoticePropertyByL10N(Notice input, out string linkStr, out string picStr,
        //     out string titleStr, out string contentStr)
        // {
        //     SelectTitlrByL10N(input, out var title);
        //     titleStr = title;
        //     switch (noticeL10N)
        //     {
        //         case Tblanguage.L10N.en:
        //             linkStr = input.enLink;
        //             picStr = input.enPic;
        //             //titleStr = input.enTitle;
        //             contentStr = input.enContent;
        //             break;
        //         case Tblanguage.L10N.zh_cn:
        //             linkStr = input.zhCnLink;
        //             picStr = input.zhCnPic;
        //             //titleStr = input.zhCnTitle;
        //             contentStr = input.zhCnContent;
        //             break;
        //         case Tblanguage.L10N.kr:
        //             //notice do not have kr
        //             linkStr = input.enLink;
        //             picStr = input.enPic;
        //             //titleStr = input.enTitle;
        //             contentStr = input.enContent;
        //             break;
        //         case Tblanguage.L10N.jp:
        //             linkStr = input.ryLink;
        //             picStr = input.ryPic;
        //             //titleStr = input.ryTitle;
        //             contentStr = input.ryContent;
        //             break;
        //         case Tblanguage.L10N.fr:
        //             linkStr = input.frLink;
        //             picStr = input.frPic;
        //             //titleStr = input.frTitle;
        //             contentStr = input.frContent;
        //             break;
        //         case Tblanguage.L10N.de:
        //             linkStr = input.deLink;
        //             picStr = input.dePic;
        //             //titleStr = input.deTitle;
        //             contentStr = input.deContent;
        //             break;
        //         case Tblanguage.L10N.ru:
        //             //do not have ru
        //             linkStr = input.enLink;
        //             picStr = input.enPic;
        //             //titleStr = input.enTitle;
        //             contentStr = input.enContent;
        //             break;
        //         case Tblanguage.L10N.th:
        //             //do not have th
        //             linkStr = input.enLink;
        //             picStr = input.enPic;
        //             //titleStr = input.enTitle;
        //             contentStr = input.enContent;
        //             break;
        //         case Tblanguage.L10N.es:
        //             linkStr = input.esLink;
        //             picStr = input.esPic;
        //             //titleStr = input.esTitle;
        //             contentStr = input.esContent;
        //             break;
        //         default:
        //             linkStr = input.enLink;
        //             picStr = input.enPic;
        //             //titleStr = input.enTitle;
        //             contentStr = input.enContent;
        //             break;
        //     }
        // }

        // private string SelectNoticePicByL10N(Notice input)
        // {
        //     string picName;
        //     // switch (noticeL10N)
        //     // {
        //     //     case Tblanguage.L10N.en:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.enPic);
        //     //         break;
        //     //     case Tblanguage.L10N.zh_cn:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.zhCnPic);
        //     //         break;
        //     //     case Tblanguage.L10N.kr:
        //     //         //notice do not have kr
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.enPic);
        //     //         break;
        //     //     case Tblanguage.L10N.jp:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.ryPic);
        //     //         break;
        //     //     case Tblanguage.L10N.fr:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.frPic);
        //     //         break;
        //     //     case Tblanguage.L10N.de:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.dePic);
        //     //         break;
        //     //     case Tblanguage.L10N.ru:
        //     //         //do not have ru
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.enPic);
        //     //         break;
        //     //     case Tblanguage.L10N.th:
        //     //         //do not have th
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.enPic);
        //     //         break;
        //     //     case Tblanguage.L10N.es:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.esPic);
        //     //         break;
        //     //     default:
        //     //         picName = JiYuUIHelper.GetNameStringFormUrl(input.enPic);
        //     //         break;
        //     // }
        //
        //     return Application.dataPath + localPathHelp + picName;
        // }

        // private string NoticeTextHelp(Notice input, string contentStr)
        // {
        //     //string helpStr = tblanguage.Get(tbannounce.Get(i).content).current;
        //     var contentStrArray = input.para.Split(';');
        //     for (int i = 0; i < contentStrArray.Length; i++)
        //     {
        //         contentStr = contentStr.Replace("{" + i.ToString() + "}", contentStrArray[i]);
        //         //helpStr = helpStr.Replace("{" + j.ToString() + "}", tbannounce.Get(i).para[j]);
        //     }
        //
        //     return contentStr;
        // }


        private async UniTaskVoid SetSpriteByLocalPath(string name, UI ui)
        {
            //string fileName = Path.GetFileName(localPath);
            string localPath = $"SharedData/{name}";
            Log.Debug($"localPath {localPath}");
            var sprite = await Resources.LoadAsync<Sprite>(localPath) as Sprite;

            Log.Debug($"sprite {sprite.name}");
            ui.GetImage().SetOverrideSprite(sprite, true);

            return;
            byte[] imageData = File.ReadAllBytes(localPath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            Sprite sprite0 = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            ui.GetComponent<XImage>().overrideSprite = sprite0;


            //ui.GetComponent<Image>().overrideSprite
        }

        private async UniTaskVoid CreateNoticeItem()
        {
            var KScrollView = GetFromReference(UIPanel_Notice.KScrollView);
            var KImg_Empty = GetFromReference(UIPanel_Notice.KImg_Empty);
            var KText_Empty = GetFromReference(UIPanel_Notice.KText_Empty);
            var KImg_Notice = GetFromReference(UIPanel_Notice.KImg_Notice);
            var KText_Name = GetFromReference(UIPanel_Notice.KText_Name);
            var KText_Notice = GetFromReference(UIPanel_Notice.KText_Notice);

            var content = KScrollView.GetXScrollRect().Content;
            var list = content.GetList();
            list.Clear();

            JsonManager.Instance.sharedData.noticesList.notices.Sort((a, b) => { return a.id.CompareTo(b.id); });

            for (int i = 0; i < JsonManager.Instance.sharedData.noticesList.notices.Count; i++)
            {
                var index = i;
                var noticeMulti = JsonManager.Instance.sharedData.noticesList.notices[index];
                Log.Debug($"l10N:{JsonManager.Instance.sharedData.l10N}");
                var noticeSingle = noticeMulti.notices
                    .Where(a => a.l10N == JsonManager.Instance.sharedData.l10N).FirstOrDefault();
                //KText_Name.s  noticeSingle.title;
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UISubPanel_Notice_Item, noticeMulti.id, false) as
                        UISubPanel_Notice_Item;
                var KText = ui.GetFromReference(UISubPanel_Notice_Item.KText);
                var KButton = ui.GetFromReference(UISubPanel_Notice_Item.KButton);
                var KImg = ui.GetFromReference(UISubPanel_Notice_Item.KImg);
                var KImg_RedPoint = ui.GetFromReference(UISubPanel_Notice_Item.KImg_RedPoint);
                Log.Debug(
                    $"noticeSingle.title {noticeSingle.title} ");
                KText.GetTextMeshPro().SetTMPText(noticeSingle.title);
                KImg_RedPoint.SetActive(noticeMulti.readStatus == 0);
                string iconName = Path.GetFileName(noticeSingle.icon);
                string picName = Path.GetFileName(noticeSingle.pic);
                JiYuUIHelper.LoadImage(iconName, KImg);
                if (index == 0)
                {
                    KText_Name.GetTextMeshPro().SetTMPText(noticeSingle.title);
                    KText_Notice.GetTextMeshPro().SetTMPText(noticeSingle.content);
                    JiYuUIHelper.LoadImage(picName, KImg_Notice);
                }

                var itemStr = $"{m_RedDotName}|Pos{index}";
                RedDotManager.Instance.InsterNode(itemStr);
                RedDotManager.Instance.SetRedPointCnt(itemStr, noticeMulti.readStatus == 0 ? 1 : 0);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KButton, () =>
                {
                    if (lastNoticeId == noticeMulti.id)
                    {
                        return;
                    }

                    KText_Name.GetTextMeshPro().SetTMPText(noticeSingle.title);
                    KText_Notice.GetTextMeshPro().SetTMPText(noticeSingle.content);
                    JiYuUIHelper.LoadImage(picName, KImg_Notice);
                    noticeMulti.readStatus = 1;
                    RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
                    KImg_RedPoint.SetActive(noticeMulti.readStatus == 0);
                });
            }

            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Notice_Item;
                var uib = b as UISubPanel_Notice_Item;
                return uia.id.CompareTo(uib.id);
            });

            //return;
            // if (nlist.Count != 0)
            // {
            //     this.GetFromReference(KScrollView).SetActive(true);
            //
            //     #region nlist.count != 0
            //
            //     SetItemContentWidth();
            //     ContentList.Clear();
            //
            //     for (int i = 0; i < nlist.Count; i++)
            //     {
            //         int ihelp = i;
            //         var uiItem = await ContentList.CreateWithUITypeAsync(UIType.UISubPanel_Notice_Item, false);
            //
            //         //RedPointMgr.instance.Init(Person_Red_Point_Root, "notice" + nlist[ihelp].id.ToString(),
            //         //    (RedPointState state, int data) =>
            //         //    {
            //         //        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Notice, out UI uuii))
            //         //        {
            //         //            uiItem.GetFromReference(UISubPanel_Notice_Item.KImg_RedPoint)
            //         //                .SetActive(state == RedPointState.Show);
            //         //        }
            //         //    });
            //
            //         //if (nlist[ihelp].readStatus == 0)
            //         //{
            //         //    //set redpoint state show
            //         //    RedPointMgr.instance.SetState(Person_Red_Point_Root, "notice" + nlist[ihelp].id.ToString(),
            //         //        RedPointState.Show);
            //         //}
            //         //else
            //         //{
            //         //    //set redpoint state hide
            //         //    RedPointMgr.instance.SetState(Person_Red_Point_Root, "notice" + nlist[ihelp].id.ToString(),
            //         //        RedPointState.Hide);
            //         //}
            //
            //         if (ihelp == 0)
            //         {
            //             uiItem.GetFromReference(UISubPanel_Notice_Item.KImg_Select).SetActive(true);
            //             //RedPointMgr.instance.SetState(Person_Red_Point_Root, "notice" + nlist[ihelp].id.ToString(),
            //             //    RedPointState.Hide);
            //             JiYuUIHelper.SetNoticeReadStatusByID(nlist[ihelp].id);
            //             ResourcesSingleton.Instance.UpdateResourceUI();
            //             lastItem = uiItem;
            //         }
            //         else
            //         {
            //             uiItem.GetFromReference(UISubPanel_Notice_Item.KImg_Select).SetActive(false);
            //         }
            //
            //         var btn = uiItem.GetFromReference(UISubPanel_Notice_Item.KButton);
            //         JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
            //         {
            //             uiItem.GetFromReference(UISubPanel_Notice_Item.KImg_Select).SetActive(true);
            //             if (lastItem != uiItem)
            //             {
            //                 lastItem.GetFromReference(UISubPanel_Notice_Item.KImg_Select).SetActive(false);
            //                 lastItem = uiItem;
            //                 SetNotice(ihelp);
            //             }
            //
            //             if (nlist[ihelp].readStatus == 0)
            //             {
            //                 foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
            //                 {
            //                     if (nt.id == nlist[ihelp].id)
            //                     {
            //                         nt.readStatus = 1;
            //                         break;
            //                     }
            //                 }
            //
            //                 //set redpoint state hide
            //                 JiYuUIHelper.SetNoticeReadStatusByID(nlist[ihelp].id);
            //             }
            //
            //             ResourcesSingleton.Instance.UpdateResourceUI();
            //         });
            //
            //         //int id = fakenews[ihelp].ID;
            //         SelectTitlrByL10N(nlist[ihelp], out var title);
            //         uiItem.GetFromReference(UISubPanel_Notice_Item.KText).GetTextMeshPro().SetTMPText(title);
            //
            //         var picPath = SelectNoticePicByL10N(nlist[ihelp]);
            //         SetSpriteByLocalPath(picPath, uiItem.GetFromReference(UISubPanel_Notice_Item.KImg));
            //     }
            //
            //     JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
            //
            //     #endregion
            // }
            // else
            // {
            //     this.GetFromReference(KScrollView).SetActive(false);
            // }
        }

        void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Notice.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
            
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(2000, this.Update);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        // private void SelectTitlrByL10N(Notice input, out string title)
        // {
        //     switch (noticeL10N)
        //     {
        //         case Tblanguage.L10N.en:
        //             title = input.enTitle;
        //             break;
        //         case Tblanguage.L10N.zh_cn:
        //             title = input.zhCnTitle;
        //             break;
        //         case Tblanguage.L10N.kr:
        //             //notice do not have kr
        //             title = input.enTitle;
        //             break;
        //         case Tblanguage.L10N.jp:
        //             title = input.ryTitle;
        //             break;
        //         case Tblanguage.L10N.fr:
        //             title = input.frTitle;
        //             break;
        //         case Tblanguage.L10N.de:
        //             title = input.deTitle;
        //             break;
        //         case Tblanguage.L10N.ru:
        //             //do not have ru
        //             title = input.enTitle;
        //             break;
        //         case Tblanguage.L10N.th:
        //             //do not have th
        //             title = input.enTitle;
        //             break;
        //         case Tblanguage.L10N.es:
        //             title = input.esTitle;
        //             break;
        //         default:
        //             title = input.enTitle;
        //             break;
        //     }
        // }

        // private void SetItemContentWidth()
        // {
        //     int noticeCount = nlist.Count;
        //     float viewW = noticeCount * 330;
        //     var scrollView = this.GetFromReference(KScrollView);
        //     if (viewW > scrollView.GetRectTransform().Width())
        //     {
        //         scrollView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
        //     }
        //     else
        //     {
        //         scrollView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
        //     }
        //
        //     var scrollViewContentRect = scrollView.GetScrollRect().Content.GetRectTransform();
        //     scrollViewContentRect.SetWidth(viewW);
        //     scrollViewContentRect.SetOffsetWithTop(0);
        //     scrollViewContentRect.SetOffsetWithBottom(0);
        //     scrollViewContentRect.SetAnchoredPositionX(0);
        // }

        protected override void OnClose()
        {
            RemoveTimer();
            JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}