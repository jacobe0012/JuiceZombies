//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_AnimTools)]
    internal sealed class UIPanel_AnimToolsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_AnimTools;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_AnimTools>();
        }
    }

    public partial class UIPanel_AnimTools : UI, IAwake
    {
        public bool stop = false;

        public void Initialize()
        {
            InitNode();
        }

        async void InitNode()
        {
            var KImg_Tips_1 = GetFromReference(UIPanel_AnimTools.KImg_Tips_1);
            var KImg_Tips_2 = GetFromReference(UIPanel_AnimTools.KImg_Tips_2);
            var KImg_Tips_3 = GetFromReference(UIPanel_AnimTools.KImg_Tips_3);
            var KImg_Tips_4 = GetFromReference(UIPanel_AnimTools.KImg_Tips_4);
            var KImg_Bg = GetFromReference(UIPanel_AnimTools.KImg_Bg);
            var KImg_noForceBg = GetFromReference(UIPanel_AnimTools.KImg_noForceBg);
            //var KText_Tips_4 = GetFromReference(UIPanel_AnimTools.KText_Tips_4);
            var KBtn_Bg = GetFromReference(UIPanel_AnimTools.KBtn_Bg);
            var KImg_Tips_5 = GetFromReference(UIPanel_AnimTools.KImg_Tips_5);
            var KImg_Tips_6 = GetFromReference(UIPanel_AnimTools.KImg_Tips_6);
            // var KText_Tips_5 = GetFromReference(UIPanel_AnimTools.KText_Tips_5);
            // var KText_Tips_6 = GetFromReference(UIPanel_AnimTools.KText_Tips_6);
            // var KText_Tips_2 = GetFromReference(UIPanel_AnimTools.KText_Tips_2);
            Refresh();
            //var =KImg_Tips_1.GetComponent<UIAnimationTools>();
        }

        private async UniTask ScaleRefresh(UI ui, List<UIAnimationTools.AnimationScale> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.duration = Mathf.Max(item.duration, 0.02f);
                await UniTask.Delay((int)(item.startTime * 1000f));
                ui.GetRectTransform().DoScale(new Vector3(item.offset0, item.offset0, 0),
                    new Vector3(item.offset1, item.offset1, 0),
                    item.duration);
                //
                if (i == list.Count - 1)
                {
                    //ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                    await UniTask.Delay((int)(item.duration * 1000f));
                }
            }
        }

        private async UniTask AlphaRefresh(UI ui, List<UIAnimationTools.AnimationAlpha> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.duration = Mathf.Max(item.duration, 0.02f);
                await UniTask.Delay((int)(item.startTime * 1000f));
                ui.GetImage().DoFade(item.offset0, item.offset1, item.duration);
                //
                if (i == list.Count - 1)
                {
                    //ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                    await UniTask.Delay((int)(item.duration * 1000f));
                }
            }
        }

        private async UniTask TranRefresh(UI ui, List<UIAnimationTools.AnimationTran> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.duration = Mathf.Max(item.duration, 0.02f);
                await UniTask.Delay((int)(item.startTime * 1000f));
                ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                //
                if (i == list.Count - 1)
                {
                    //ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                    await UniTask.Delay((int)(item.duration * 1000f));
                }
            }
        }

        public void ReSet()
        {
            for (int i = 1; i < 13; i++)
            {
                var index = i;
                var str = $"Img_Tips_{index}";
                var img = this.GetFromReference(str);
                var tools = img.GetComponent<UIAnimationTools>();
                if (tools == null)
                {
                    continue;
                }

                for (int j = 0; j < tools.animationScales.Count; j++)
                {
                    var item = tools.animationScales[j];
                    if (j == 0)
                    {
                        img.GetRectTransform().SetScale(new Vector2(item.offset0, item.offset0));
                        break;
                    }
                }

                for (int j = 0; j < tools.animationAlphas.Count; j++)
                {
                    var item = tools.animationAlphas[j];
                    if (j == 0)
                    {
                        img.GetImage().SetAlpha(item.offset0);
                        break;
                    }
                }

                for (int j = 0; j < tools.animationTrans.Count; j++)
                {
                    var item = tools.animationTrans[j];
                    if (j == 0)
                    {
                        img.GetRectTransform().SetAnchoredPosition(item.offset0);
                        break;
                    }
                }
            }

            //stop = true;
        }

        struct AnimToolsStuct
        {
            public UI ui;

            public UIAnimationTools tools;
            //public int type;
        }

        public async UniTaskVoid Refresh()
        {
            stop = false;
            var tasks = new List<UniTask>();
            var animToolsStuct = new List<AnimToolsStuct>();
            for (int i = 1; i < 13; i++)
            {
                var index = i;
                var str = $"Img_Tips_{index}";
                var img = this.GetFromReference(str);
                var tools = img.GetComponent<UIAnimationTools>();
                if (tools == null || !img.GameObject.activeSelf)
                {
                    continue;
                }

                animToolsStuct.Add(new AnimToolsStuct
                {
                    ui = img,
                    tools = tools,
                });
                // var task1 = ScaleRefresh(img, tools.animationScales);
                // var task2 = AlphaRefresh(img, tools.animationAlphas);
                // var task3 = TranRefresh(img, tools.animationTrans);

                // tasks.Add(task1);
                // tasks.Add(task2);
                // tasks.Add(task3);
            }

            // foreach (var VARIABLE in animToolsStuct)
            // {
            //     
            // }
            while (!stop)
            {
                foreach (var VARIABLE in animToolsStuct)
                {
                    var task1 = ScaleRefresh(VARIABLE.ui, VARIABLE.tools.animationScales);
                    var task2 = AlphaRefresh(VARIABLE.ui, VARIABLE.tools.animationAlphas);
                    var task3 = TranRefresh(VARIABLE.ui, VARIABLE.tools.animationTrans);

                    tasks.Add(task1);
                    tasks.Add(task2);
                    tasks.Add(task3);
                }

                await UniTask.WhenAll(tasks);
                tasks.Clear();
                ReSet();
            }
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}