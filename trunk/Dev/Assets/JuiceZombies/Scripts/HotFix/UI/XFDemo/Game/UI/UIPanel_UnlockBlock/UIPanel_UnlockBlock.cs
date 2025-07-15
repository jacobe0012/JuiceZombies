//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_UnlockBlock)]
    internal sealed class UIPanel_UnlockBlockEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_UnlockBlock;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_UnlockBlock>();
        }
    }

    public partial class UIPanel_UnlockBlock : UI, IAwake<Vector2>
    {
        public int blockId;
        public int chapterId;
        private Tblanguage tblanguage;
        private Tbtag_func tbtag_func;
        private Tbblock tbblock;
        private Tbchapter tbchapter;
        private long timerId;
        private const int maxTitleH= 160;
        private const int maxTitleW = 244;
        /// <summary>
        /// args.x = 类型 0街区 1章节 args.y = id
        /// </summary>
        /// <param name="args"></param>
        public async void Initialize(Vector2 args)
        {
            await JiYuUIHelper.InitBlur(this);
            InitJson();
            Init(args);
            StartTimer();
        }

        public void Init(Vector2 args)
        {
            var KImg_Unlock = GetFromReference(UIPanel_UnlockBlock.KImg_Unlock);
            //var KText_Title = GetFromReference(UIPanel_UnlockBlock.KText_Title);
            var KText_Name = GetFromReference(UIPanel_UnlockBlock.KText_Name);
            var KText_Click = GetFromReference(UIPanel_UnlockBlock.KText_Click);
            var KBtn_Mask = GetFromReference(UIPanel_UnlockBlock.KBtn_Mask);
            var KImg_Block = GetFromReference(UIPanel_UnlockBlock.KImg_Block);
            var KImg_Chapter = GetFromReference(UIPanel_UnlockBlock.KImg_Chapter);
            var KImg_BlockPic = GetFromReference(UIPanel_UnlockBlock.KKImg_BlockPic);
            //''？？？？？？？？？？？？？？？？？？？？？var KImg_ChapterPic = GetFromReference(UIPanel_UnlockBlock.KImg_ChapterPic);
            var KImg_NameBg= GetFromReference(UIPanel_UnlockBlock.KImg_NameBg);
            KImg_Block.SetActive(false);
            KImg_Chapter.SetActive(false);

            int id = (int)args.y;

            switch ((int)args.x)
            {
                case 0:
                    KImg_Block.SetActive(true);
                    //KImg_Chapter.SetActive(false);
                    GetFromReference(KImgTitle).GetXImage().SetSprite(JiYuUIHelper.GetL10NPicName("block_unlock"),true);
                    KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get(tbblock.Get(id).name).current);
                    KImg_BlockPic.GetImage().SetSpriteAsync(tbblock.Get(id).pic, false).Forget();

                    break;
                case 1:
                    //KImg_Block.SetActive(false);
                    KImg_Chapter.SetActive(true);
                    GetFromReference(KImgTitle).GetXImage().SetSprite(JiYuUIHelper.GetL10NPicName("level_unlock"), true);
                    KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get(tbchapter.Get(id).name).current);
                    break;
            }
            
            var closeStr = JiYuUIHelper.GetBulletTypeStr(tblanguage.Get("text_window_close").current);

            KText_Click.GetTextMeshPro().SetTMPText(closeStr);
            var height=KText_Name.GetTextMeshPro().Get().preferredHeight;
            var width = KText_Name.GetTextMeshPro().Get().preferredWidth;
            width = width > maxTitleW ? maxTitleW : width;
            height = height > maxTitleH ? maxTitleH : height;
            KText_Name.GetRectTransform(). SetHeight(height);
            KText_Name.GetRectTransform().SetWidth(width);
            KImg_NameBg.GetRectTransform().SetHeight(height+20);
            KImg_NameBg.GetRectTransform().SetWidth(width+70);
            //KText_Click.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);
        }

        void Update()
        {
            var KText_Click = GetFromReference(UIPanel_UnlockBlock.KText_Click);

            KText_Click.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KText_Click.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(2500, this.Update);
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

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tbblock = ConfigManager.Instance.Tables.Tbblock;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
        }


        protected override void OnClose()
        {
            RemoveTimer();
            base.OnClose();
        }
    }
}