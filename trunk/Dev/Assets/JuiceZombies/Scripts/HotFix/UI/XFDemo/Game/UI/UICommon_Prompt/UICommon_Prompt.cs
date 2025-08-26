//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: 2023-10-17
//---------------------------------------------------------------------

using System.Threading;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using dnlib.Threading;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Prompt)]
    internal sealed class UICommon_PromptEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Prompt;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

      
        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Prompt>();
        }
    }

    public partial class UICommon_Prompt : UI, IAwake<string>
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        //放大倍数
        public float EnlargeMultiple = 1.3f;

        //向上位移距离
        public float MoveY = 200f;

        //位移时间
        public float MoveTime = 0.5f;

        //保持时间
        public float UnchangedTime = 1.4f;

        //消失时间
        public float DisappearTime = 0.8f;

        //取用于文字透明度设置的组件
        private XTextMeshProUGUI xTextMeshProUGUI = null;

        //更新透明度时间间隔（单位毫秒）
        private int timeInterval = 50;

        public void Initialize(string str)
        {
            var KTxt_Prompt = GetFromReference(UICommon_Prompt.KTxt_Prompt);
            var KImage = GetFromReference(UICommon_Prompt.KImage);
            var imageRect = KImage.GetRectTransform();
            imageRect.SetAnchoredPositionY(200);

            var KTxt_PromptTMP = KTxt_Prompt.GetTextMeshPro();
            KImage.GetImage().SetAlpha(0.8f);
            KTxt_PromptTMP.SetAlpha(0.8f);
            xTextMeshProUGUI = KTxt_Prompt.GetComponent<XTextMeshProUGUI>();
            //设置提示语内容
            KTxt_PromptTMP.SetTMPText(str);
            //提取文本的高度
            var preferredH = KTxt_PromptTMP.Get().preferredHeight;

            //用文本高度设置背景高度
            KImage.GetRectTransform().SetHeight(preferredH + 10);

            //复制背景图的材质防止调整透明度变化的时候全部变化
            Material originalMaterial = KImage.GetComponent<XImage>().material;
            Material material = new Material(originalMaterial);
            KImage.GetComponent<XImage>().material = material;
            //创建动画相关的参数
            //放大参数
            Vector3 enlarge = new Vector3(1.0f, 1.0f, 1.0f);
            enlarge = EnlargeMultiple * enlarge;
            StartAnimation(enlarge,cts.Token);


        }
        public async UniTask StartAnimation(Vector3 enlarge, CancellationToken token)
        {
           
           
            var KImage = GetFromReference(UICommon_Prompt.KImage);
            KImage.GetComponent<CanvasGroup>().alpha = 1;
            try
            {
                var imageRect = KImage.GetComponent<RectTransform>();

                // 缩放动画（放大）
                imageRect.DOScale(enlarge, MoveTime).SetUpdate(true);

                // 移动动画（向上移动）
                Sequence sequence = DOTween.Sequence();
                sequence.Append(imageRect.DOAnchorPosY(imageRect.anchoredPosition.y + MoveY, MoveTime).SetUpdate(true))
                       .AppendInterval(UnchangedTime)
                       .AppendCallback(async () =>
                       {
                           // 检查是否取消
                           token.ThrowIfCancellationRequested();

                           // 淡出动画
                           KImage.GetComponent<CanvasGroup>().DOFade(0, DisappearTime).SetUpdate(true).OnComplete(Close);
                       })
                       .SetUpdate(true);

                await UniTask.WaitWhile(() => sequence.IsActive() && !sequence.IsComplete(), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                // 动画被取消时，停止所有 DOTween 动画
                DOTween.CompleteAll();
                Close();
            }

        }
            /// <summary>
            /// 在规定输入时间内逐步把字体的透明度设置到0，输入时间单位为毫秒
            /// </summary>
            private async UniTaskVoid AlphaUpdate(int timeN)
        {
            float helpTime = timeN;
            float tAlpha = xTextMeshProUGUI.color.a;
            xTextMeshProUGUI.color = new Color(xTextMeshProUGUI.color.r, xTextMeshProUGUI.color.g,
                xTextMeshProUGUI.color.b, tAlpha - ((float)timeInterval) / helpTime);
            while (timeN > 0)
            {
                tAlpha = xTextMeshProUGUI.color.a;
                xTextMeshProUGUI.color = new Color(xTextMeshProUGUI.color.r, xTextMeshProUGUI.color.g,
                    xTextMeshProUGUI.color.b, tAlpha - ((float)timeInterval) / helpTime);
                timeN -= timeInterval;
                await UniTask.Delay(timeInterval);
            }
        }

        protected override void OnClose()
        {
            cts.Cancel();
            base.OnClose();
        }
    }
}