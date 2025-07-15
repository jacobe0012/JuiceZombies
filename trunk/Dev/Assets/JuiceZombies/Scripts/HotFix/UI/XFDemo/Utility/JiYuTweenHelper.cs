//---------------------------------------------------------------------
// JiYuStudio
// Author: 迅捷蟹
// Time: 2023-8-8 17:58:20
//---------------------------------------------------------------------


using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using Main;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//插件也写了一些DoTween效果,但不清楚是否适合我们,所以自己写了一个
namespace XFramework
{
    public static class JiYuTweenHelper
    {
        #region Trans

        public static void SetEffectUIState(int sort)
        {
            Log.Debug($"SetEffectUIState sort：{sort}");
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui))
            {
                var uiShop = ui as UIPanel_Shop;
                uiShop.GetAllMoudelUIs(sort == 1);
                // var effects = uiShop.GetEffectUIs();
                // for (int i = 0; i < effects.Count; i++)
                // {
                //     Log.Debug($"name：{effects[i].Name}");
                //     effects[i].SetActive(sort== 1);
                // }
            }
        }


        public static async UniTask<AsyncUnit> SetAngleRotate(UI ui, float endAngle, float rotate = 45,
            float duration = 0.35f,
            CancellationToken cancellationToken = default)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                var targetUi = ui.GetComponent<RectTransform>();
                UniTask.Delay(1, cancellationToken: cancellationToken);
                //await UniTask.Delay(200);
                DOTween.To(
                    () => -(endAngle - rotate), // 初始值
                    value => targetUi.localEulerAngles = new Vector3(0, 0, value), // 更新旋转
                    endAngle, // 目标角度
                    duration // 持续时间
                ).SetEase(Ease.InQuad); // 设置缓动类型，可选
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<Tween> SetBookmarkSwing(RectTransform bookmark, int loopCount = -1,
            float maxAngle = 10f, float duration = 1f)
        {
            if (bookmark == null) return null;

            // 设置书签的锚点为指定旋转点（例如顶部中心）
            bookmark.pivot = new Vector2(0.5f, 1); // pivotPoint 为相对于 RectTransform 的归一化坐标，例如 (0.5, 1) 表示顶部中心

            // 创建一个 DOTween Sequence 用于晃动效果
            Sequence sequence = DOTween.Sequence();

            // 初始状态：将书签旋转归零
            bookmark.localRotation = Quaternion.Euler(0, 0, -maxAngle);
            ;

            // 添加多层晃动效果，模拟书签的不规则摆动
            sequence.Append(bookmark.DOLocalRotate(new Vector3(0, 0, maxAngle), duration * 0.5f)
                .SetEase(Ease.InQuad)); // 第一次向右摆动，平滑过渡

            sequence.Append(bookmark.DOLocalRotate(new Vector3(0, 0, -maxAngle * 0.6f), duration * 0.2f)
                .SetEase(Ease.InQuad)); // 向左摆动，幅度略减小

            sequence.Append(bookmark.DOLocalRotate(new Vector3(0, 0, maxAngle * 0.3f), duration * 0.2f)
                .SetEase(Ease.InQuad)); // 回到初始位置
            sequence.Append(bookmark.DOLocalRotate(new Vector3(0, 0, 0), duration * 0.2f)
                .SetEase(Ease.InQuad)); // 回到初始位置

            // 设置循环，-1 表示无限循环
            sequence.SetLoops(loopCount, LoopType.Yoyo); // Yoyo 模式让动画来回播放
            sequence.SetUpdate(true); // 使用未缩放时间，忽略暂停

            return sequence;
        }


        public static async UniTask<AsyncUnit> SetAngleRotateXZ(UI ui, float endAngleX, float endAngleZ,
            CancellationToken cancellationToken = default, float rotateX = 45,
            float rotateZ = 45, float duration = 0.35f)
        {
            var targetUi = ui.GetComponent<RectTransform>();

            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                DOTween.To(
                    () => new Vector2(-(endAngleX - rotateX), -(endAngleZ - rotateZ)), // 初始值（X: 0, Z: 0）
                    value => targetUi.localEulerAngles = new Vector3(value.x, 0, value.y), // 更新X和Z旋转
                    new Vector2(endAngleX, endAngleZ), // 目标角度（X: endX, Z: endZ）
                    duration // 持续时间
                ).SetEase(Ease.InQuad).SetUpdate(true); // 设置缓动类型
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<AsyncUnit> SetEaseAlphaAndPosB2U(UI ui, float endPosY, float incremental = 200f,
            CancellationToken cancellationToken = default,
            float duration = 0.35f, bool isAlpha = true, bool isBounce = true)
        {
            if (ui == null)
            {
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                return AsyncUnit.Default;
            }

            try
            {
                if (isAlpha)
                {
                    ui.GetComponent<CanvasGroup>().alpha = 0.5f;
                    ui.GetComponent<CanvasGroup>().DOFade(1, duration).SetEase(Ease.InQuad).SetUpdate(true);
                }

                ui.GetRectTransform().SetAnchoredPositionY(endPosY - incremental);

                if (isBounce)
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosY(endPosY + 20, duration).SetEase(Ease.InQuad)
                        .SetUpdate(true);
                    await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken);
                    ui.GetComponent<RectTransform>().DOAnchorPosY(endPosY, duration / 2f).SetEase(Ease.InQuad)
                        .SetUpdate(true);
                }
                else
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosY(endPosY, duration).SetEase(Ease.InQuad)
                        .SetUpdate(true);
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<AsyncUnit> SetEaseAlphaAndPosUtoB(UI ui, float endPosY, float incremental = 200f,
            CancellationToken cancellationToken = default,
            float duration = 0.35f, bool isAlpha = true, bool isBounce = true)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                //Log.Debug("SetEaseAlphaAndPosUtoB", Color.cyan);
                if (isAlpha)
                {
                    ui.GetComponent<CanvasGroup>().alpha = 0.2f;
                    ui.GetComponent<CanvasGroup>().DOFade(1, duration).SetEase(Ease.InQuad);
                }

                ui.GetRectTransform().SetAnchoredPositionY(endPosY + incremental);
                if (isBounce)
                {
                    ui.GetRectTransform().SetAnchoredPositionY(endPosY + incremental);
                    ui.GetComponent<RectTransform>().DOAnchorPosY(endPosY - 20, duration).SetEase(Ease.InQuad);
                    await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken);
                    ui.GetComponent<RectTransform>().DOAnchorPosY(endPosY, duration / 2f).SetEase(Ease.InQuad);
                }
                else
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosY(endPosY, duration).SetEase(Ease.InQuad);
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<AsyncUnit> SetEaseAlphaAndPosLtoR(UI ui, float endPosX, float incremental = 200f,
            CancellationToken cancellationToken = default,
            float duration = 0.25f, bool isAlpha = true, bool isBounce = false)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                if (isAlpha)
                {
                    if (ui.GetComponent<CanvasGroup>() != null)
                    {
                        ui.GetComponent<CanvasGroup>().alpha = 0.2f;
                        ui.GetComponent<CanvasGroup>().DOFade(1, duration).SetEase(Ease.InQuad).SetUpdate(true);
                    }
                }

                ui.GetRectTransform().SetAnchoredPositionX(endPosX - incremental);
                if (isBounce)
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosX(endPosX + 20, duration).SetEase(Ease.InQuad)
                        .SetUpdate(true);
                    await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken);
                    ui.GetComponent<RectTransform>().DOAnchorPosX(endPosX, duration / 2f).SetEase(Ease.InQuad)
                        .SetUpdate(true);
                }
                else
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosX(endPosX, duration).SetEase(Ease.InQuad)
                        .SetUpdate(true);
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<AsyncUnit> SetEaseAlphaAndPosRtoL(UI ui, float endPosX, float incremental = 200f,
            CancellationToken cancellationToken = default,
            float duration = 0.25f, bool isAlpha = true, bool isBounce = true)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                if (isAlpha)
                {
                    ui.GetComponent<CanvasGroup>().alpha = 0.2f;
                    ui.GetComponent<CanvasGroup>().DOFade(1, duration).SetEase(Ease.InQuad);
                }

                ui.GetRectTransform().SetAnchoredPositionX(endPosX + incremental);

                if (isBounce)
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosX(endPosX - 20, duration).SetEase(Ease.InQuad);
                    await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken);
                    ui.GetComponent<RectTransform>().DOAnchorPosX(endPosX, duration / 2f).SetEase(Ease.InQuad);
                }
                else
                {
                    ui.GetComponent<RectTransform>().DOAnchorPosX(endPosX, duration).SetEase(Ease.InQuad);
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }


        public static async UniTask<AsyncUnit> SetEaseAlphaAndScale(UI ui, float duration = 0.25f, bool isAlpha = true,
            float scaleStart = 0f, float scaleEnd = 1f, CancellationToken cancellationToken = default,
            Ease type = Ease.InQuad)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                if (isAlpha)
                {
                    ui.GetComponent<CanvasGroup>().alpha = 0f;
                    ui.GetComponent<CanvasGroup>().DOFade(1, duration).SetEase(type).SetUpdate(true);
                }

                await UniTask.Delay((int)(1), cancellationToken: cancellationToken);
                ui?.GetRectTransform()?.SetScale(new float3(scaleStart, scaleStart, scaleStart));
                var tween = ui.GetComponent<RectTransform>().DOScale(new float3(scaleEnd, scaleEnd, scaleEnd), duration)
                    .SetEase(type);
                tween.SetUpdate(true);
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }


        public static async UniTask<AsyncUnit> SetEaseAlphaAndScaleWithBounce(UI ui, float duration = 0.25f,
            bool isAlpha = true, float scaleStart = 0f, float scaleEnd = 1f,
            CancellationToken cancellationToken = default)
        {
            if (ui == null) return AsyncUnit.Default;

            var canvasGroup = ui.GetComponent<CanvasGroup>();
            var rectTransform = ui.GetComponent<RectTransform>();
            if (rectTransform == null) return AsyncUnit.Default;

            // 创建一个 DOTween Sequence 用于多层回弹效果
            Sequence sequence = DOTween.Sequence();
            try
            {
                // 透明度动画，带回弹效果
                if (isAlpha && canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    sequence.Append(canvasGroup.DOFade(1f, duration).SetEase(Ease.OutBack).SetUpdate(true));
                }

                // 缩放动画，带多层回弹效果
                if (rectTransform != null)
                {
                    rectTransform.localScale = new Vector3(scaleStart, scaleStart, scaleStart);

                    // 第一次回弹：稍微超过目标缩放值
                    float overshootScale = scaleEnd * 1.2f;
                    sequence.Append(rectTransform
                        .DOScale(new Vector3(overshootScale, overshootScale, overshootScale), duration * 0.6f)
                        .SetEase(Ease.OutBack).SetUpdate(true));

                    // 第二次回弹：回落到目标缩放值，带轻微回弹
                    sequence.Append(rectTransform.DOScale(new Vector3(scaleEnd, scaleEnd, scaleEnd), duration * 0.4f)
                        .SetEase(Ease.OutBounce).SetUpdate(true));
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                sequence.Kill();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f; // 重置透明度
                }

                rectTransform.localScale = new Vector3(scaleEnd, scaleEnd, scaleEnd); // 重置缩放
                Debug.Log("缩放和透明度动画已取消");
                return AsyncUnit.Default;
            }
        }


        public static async UniTask<AsyncUnit> SetEaseAlphaAndScaleWithFour(UI ui, float duration = 1.5f,
            bool isAlpha = true, float scaleStart = 0f, float scaleEnd = 1f,
            CancellationToken cancellationToken = default)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            var canvasGroup = ui.GetComponent<CanvasGroup>();
            var rectTransform = ui.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return AsyncUnit.Default;
            }

            // 创建一个 DOTween Sequence 用于多层回弹效果
            Sequence sequence = DOTween.Sequence();

            try
            {
                // 透明度动画
                if (isAlpha && canvasGroup != null)
                {
                    canvasGroup.alpha = 0.3f;
                    sequence.Append(canvasGroup.DOFade(1, 0.3f).SetEase(Ease.InQuad).SetUpdate(true));
                }

                // 缩放动画，模拟皮球四次弹跳
                rectTransform.localScale = new Vector3(scaleStart, scaleStart, scaleStart);

                // 第一次弹跳：超过目标缩放值
                float bounce1 = scaleEnd * 1.2f;
                sequence.Append(rectTransform.DOScale(new Vector3(bounce1, bounce1, bounce1), duration * 0.3f)
                    .SetEase(Ease.InQuad).SetUpdate(true));

                sequence.Append(rectTransform.DOScale(new Vector3(scaleEnd, scaleEnd, scaleEnd), duration * 0.1f)
                    .SetEase(Ease.InQuad).SetUpdate(true));

                // 第二次弹跳：略低于第一次
                float bounce3 = scaleEnd * 1.08f;
                sequence.Append(rectTransform.DOScale(new Vector3(bounce3, bounce3, bounce3), duration * 0.2f)
                    .SetEase(Ease.InQuad).SetUpdate(true));

                sequence.Append(rectTransform.DOScale(new Vector3(scaleEnd, scaleEnd, scaleEnd), duration * 0.1f)
                    .SetEase(Ease.InQuad).SetUpdate(true));

                // 第三次弹跳：更小幅度
                float bounce4 = scaleEnd * 1.02f;
                sequence.Append(rectTransform.DOScale(new Vector3(bounce4, bounce4, bounce4), duration * 0.2f)
                    .SetEase(Ease.InQuad).SetUpdate(true));

                sequence.Append(rectTransform.DOScale(new Vector3(scaleEnd, scaleEnd, scaleEnd), duration * 0.1f)
                    .SetEase(Ease.InQuad).SetUpdate(true));

                // 等待动画完成或取消
                await UniTask.WaitUntil(() => sequence.IsComplete() || cancellationToken.IsCancellationRequested,
                    cancellationToken: cancellationToken);

                // 如果是取消触发的退出，抛出 OperationCanceledException
                cancellationToken.ThrowIfCancellationRequested();

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                sequence.Complete();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f; // 重置透明度
                }

                rectTransform.localScale = new Vector3(scaleEnd, scaleEnd, scaleEnd); // 重置缩放
                Debug.Log("缩放和透明度动画已取消");
                return AsyncUnit.Default;
            }
        }


        public static async UniTask<AsyncUnit> SetScaleWithBounce(UI ui, float duration = 0.2f,
            float biggerScale = 1.2f,
            float startScale = 0f, float endScale = 1f, Ease type = Ease.InQuad, bool isAlpha = true,
            CancellationToken cancellationToken = default)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                if (isAlpha)
                {
                    ui.GetComponent<CanvasGroup>().alpha = 0f;
                    ui.GetComponent<CanvasGroup>().DOFade(1, duration * (1.5f)).SetEase(type).SetUpdate(true);
                }

                JiYuTweenHelper.SetEaseAlphaAndScale(ui, duration, false, startScale, biggerScale, cancellationToken,
                    type);
                await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken);
                JiYuTweenHelper.SetEaseAlphaAndScale(ui, duration / 2f, false, biggerScale, endScale, cancellationToken,
                    type);
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<CanvasGroup>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<AsyncUnit> SetScaleWithBounceClose(UI ui, float duration = 0.2f,
            float biggerScale = 1.2f,
            float startScale = 1f, float endScale = 0f, CancellationToken cancellationToken = default)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            ui.GetComponent<CanvasGroup>().alpha = 1f;
            ui.GetComponent<CanvasGroup>().DOFade(0, duration * (1.5f)).SetEase(Ease.InQuad).SetUpdate(true);

            return await JiYuTweenHelper.SetEaseAlphaAndScale(ui, duration, false, startScale, endScale,
                cancellationToken);
        }


        /// <summary>
        /// 左右羽化柔性展开
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="width">展开值 越大展开效果越明显</param>
        /// <param name="duration">展开持续时间</param>
        public static async UniTask<AsyncUnit> ChangeSoftness(UI ui, int width = 400, float duration = 0.25f,
            CancellationToken cancellationToken = default)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                // 确保初始值设置为1000
                var rectmask = ui.GetComponent<RectMask2D>();
                rectmask.softness = new Vector2Int(width, 0);

                await UniTask.Delay(1, cancellationToken: cancellationToken);
                // 创建动画
                DOTween.To(
                    () => (Vector2)rectmask.softness, // 将Vector2Int转换为Vector2以支持插值
                    value => rectmask.softness = new Vector2Int((int)value.x, (int)value.y), // 将插值结果转换回Vector2Int
                    new Vector2(0, 0), // 目标值
                    duration
                ).SetEase(Ease.OutQuad).SetUpdate(true);
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                ui.GetComponent<RectMask2D>()?.DOComplete();
                ui.GetComponent<RectTransform>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }


        public static void ChangePaddingLR(UI ui, int width = 400, float duration = 0.25f)
        {
            // 确保初始值设置为1000
            var rectmask = ui.GetComponent<RectMask2D>();
            rectmask.padding = new Vector4(width, 0, width, 0);
            DOTween.To(() => rectmask.padding.x,
                x => rectmask.padding = new Vector4(x, rectmask.padding.y, x, rectmask.padding.w),
                0, duration).SetEase(Ease.OutQuad);
        }


        public static float EaseFromTo(float start, float end, float value,
            UIManager.EaseType type = UIManager.EaseType.EaseInOut)
        {
            value = Mathf.Clamp01(value);

            switch (type)
            {
                case UIManager.EaseType.EaseInOut:
                    return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));

                case UIManager.EaseType.EaseOut:
                    return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));

                case UIManager.EaseType.EaseIn:
                    return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
                case UIManager.EaseType.EaseOutQuad: // 新增 OutQuad 类型
                    return Mathf.Lerp(start, end, 1.0f - (1.0f - value) * (1.0f - value));
                case UIManager.EaseType.EaseInQuad: // 从慢到快的二次缓动
                    return Mathf.Lerp(start, end, value * value);
                default:
                    return Mathf.Lerp(start, end, value);
            }
        }

        public async static UniTask EnableLoading(bool enable = true,
            UIManager.LoadingType loadingType = UIManager.LoadingType.Loading)
        {
            var uim = XFramework.Common.Instance.Get<UIManager>();
            await uim.EnableLoading(enable, loadingType);
        }

        public enum UIDir
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            UpRight = 4,
            UpLeft = 5,
            DownRight = 6,
            DownLeft = 7,
        }

        public static async UniTask<AsyncUnit> PlayUIImageTranstionFX(UI imageUI,
            CancellationToken cancellationToken = default,
            string colorHex = "A1DD01", UIDir dir = UIDir.UpLeft, float startRatios = 0,
            float duration = 1f, float colorIntensity = 0,
            UIManager.EaseType easeType = UIManager.EaseType.EaseOutQuad)
        {
            Image image = imageUI.GetComponent<Image>();
            if (image == null || image.overrideSprite == null)
            {
                return AsyncUnit.Default;
            }

            // 创建一个新的 Material，并使用指定的 Shader
            Material material = new Material(Shader.Find("JiYuStudioUI/JiYuUITransCircle"));

            try
            {
                // 将 Sprite 的纹理转换为 Texture2D，并赋值给 Material 的 _MainTex 属性
                Texture2D texture = image.overrideSprite.texture;
                material.SetTexture("_MainTex", texture); // 确保 Shader 有 _MainTex 属性
                var col = UnityHelper.HexRGB2Color(colorHex);
                col = UnityHelper.TurnHDRColor(col, colorIntensity);
                material.SetColor("_FXColor", col);
                Vector2 tilling = new Vector2(25, 0);
                tilling.y = tilling.x / ((float)texture.width / texture.height);
                material.SetVector("_Tilling", tilling);

                // 将 Material 赋值给 Image 组件
                image.material = material;

                imageUI.SetActive(true);

                float TransStartValue = 0;
                float TransEndValue = 1;
                float ClipStartValue = 0;
                float ClipEndValue = 1;
                Vector2 StartValue = Vector2.zero;

                switch (dir)
                {
                    case UIDir.Up:
                        TransStartValue = 0f;
                        TransEndValue = 1.8f;
                        ClipStartValue = 0f;
                        ClipEndValue = 1f;
                        StartValue.x = 0;
                        StartValue.y = 1;
                        break;
                    case UIDir.Down:
                        TransStartValue = -1f;
                        TransEndValue = 0.8f;
                        ClipStartValue = -1f;
                        ClipEndValue = 0f;
                        StartValue.x = 0;
                        StartValue.y = -1;
                        break;
                    case UIDir.Left:
                        TransStartValue = -1f;
                        TransEndValue = 0.8f;
                        ClipStartValue = -1f;
                        ClipEndValue = 0f;
                        StartValue.x = -1;
                        StartValue.y = 0;
                        break;
                    case UIDir.Right:
                        TransStartValue = 0f;
                        TransEndValue = 1.8f;
                        ClipStartValue = 0f;
                        ClipEndValue = 1f;
                        StartValue.x = 1;
                        StartValue.y = 0;
                        break;
                    case UIDir.UpLeft:
                        TransStartValue = -1f;
                        TransEndValue = 1.8f;
                        ClipStartValue = -1f;
                        ClipEndValue = 1f;
                        StartValue.x = -1;
                        StartValue.y = 1;
                        break;
                    case UIDir.UpRight:
                        TransStartValue = 0f;
                        TransEndValue = 2.8f;
                        ClipStartValue = 0f;
                        ClipEndValue = 2f;
                        StartValue.x = 1;
                        StartValue.y = 1;
                        break;
                    case UIDir.DownLeft:
                        TransStartValue = -2f;
                        TransEndValue = 0.8f;
                        ClipStartValue = -2f;
                        ClipEndValue = 0f;
                        StartValue.x = -1;
                        StartValue.y = -1;
                        break;
                    case UIDir.DownRight:
                        TransStartValue = -1f;
                        TransEndValue = 1.8f;
                        ClipStartValue = -1f;
                        ClipEndValue = 1f;
                        StartValue.x = 1;
                        StartValue.y = -1;
                        break;
                }

                image.material.SetVector("_ClipDir", StartValue);
                float t = Mathf.Clamp01(startRatios);

                while (t <= 1.0f)
                {
                    t += 0.02f / duration;
                    var _step = EaseFromTo(TransStartValue, TransEndValue, t, easeType);
                    var _step1 = EaseFromTo(ClipStartValue, ClipEndValue, t, easeType);
                    image.material.SetFloat("_TransWidth", _step);
                    image.material.SetFloat("_ClipWidth", _step1);
                    await UniTask.Delay(1, true, cancellationToken: cancellationToken);
                }

                image.material = null;
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理材质
                image.material.SetFloat("_TransWidth", 1);
                image.material.SetFloat("_ClipWidth", 1);
                image.material = null;
                //UnityEngine.Object.Destroy(material);
                Debug.Log("Transition FX cancelled");
                return AsyncUnit.Default;
            }
        }

        public static async UniTask<AsyncUnit> PlayUIImageSweepFX(UI imageUI,
            CancellationToken cancellationToken = default,
            string colorHex = "A1DD01", UIDir dir = UIDir.Right,
            float duration = 1f, float colorIntensity = 0, UIManager.EaseType easeType = UIManager.EaseType.EaseOutQuad)
        {
            Log.Debug($"PlayUIImageSweepFX");
            Image image = imageUI?.GetComponent<Image>();
            if (image == null || image.overrideSprite == null)
            {
                return AsyncUnit.Default;
            }

            // Create a new Material with the specified Shader
            Material material = new Material(Shader.Find("JiYuStudioUI/JiYuUISweep"));

            try
            {
                // Assign texture to Material's _MainTex property
                Texture2D texture = image.overrideSprite.texture;
                Log.Debug($"texture{texture.name}");
                material.SetTexture("_MainTex", texture);
                var col = UnityHelper.HexRGB2Color(colorHex);
                col = UnityHelper.TurnHDRColor(col, colorIntensity);
                material.SetColor("_FXColor", col);
                Vector2 tilling = new Vector2(25, 0);
                tilling.y = tilling.x / ((float)texture.width / texture.height);
                material.SetVector("_Tilling", tilling);

                // Assign Material to Image component
                image.material = material;

                imageUI.SetActive(true);

                float TransStartValue = 0;
                float TransEndValue = 1;
                Vector2 StartValue = Vector2.zero;

                switch (dir)
                {
                    case UIDir.Up:
                        TransStartValue = -0.5f;
                        TransEndValue = 2.1f;
                        StartValue.x = 0;
                        StartValue.y = 1;
                        break;
                    case UIDir.Down:
                        TransStartValue = 2.1f;
                        TransEndValue = -0.5f;
                        StartValue.x = 0;
                        StartValue.y = 1;
                        break;
                    case UIDir.Left:
                        TransStartValue = 1.8f;
                        TransEndValue = -0.8f;
                        StartValue.x = -1;
                        StartValue.y = 0;
                        break;
                    case UIDir.Right:
                        TransStartValue = -0.8f;
                        TransEndValue = 1.8f;
                        StartValue.x = -1;
                        StartValue.y = 0;
                        break;
                    case UIDir.UpLeft:
                        TransStartValue = -1f;
                        TransEndValue = 1.8f;
                        StartValue.x = -1;
                        StartValue.y = 1;
                        break;
                    case UIDir.UpRight:
                        TransStartValue = 0f;
                        TransEndValue = 2.8f;
                        StartValue.x = 1;
                        StartValue.y = 1;
                        break;
                    case UIDir.DownLeft:
                        TransStartValue = -2f;
                        TransEndValue = 0.8f;
                        StartValue.x = -1;
                        StartValue.y = -1;
                        break;
                    case UIDir.DownRight:
                        TransStartValue = -1f;
                        TransEndValue = 1.8f;
                        StartValue.x = 1;
                        StartValue.y = -1;
                        break;
                }

                image.material.SetVector("_ClipDir", StartValue);

                float t = 0f;
                while (t <= 1.0f)
                {
                    t += 0.02f / duration;
                    var _step = EaseFromTo(TransStartValue, TransEndValue, t, easeType);
                    image.material.SetFloat("_PosOffset", _step);
                    await UniTask.Delay(1, true, cancellationToken: cancellationToken);
                }

                image.material = null;
                UnityEngine.Object.Destroy(material);
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // Clean up resources on cancellation
                image.material.SetFloat("_PosOffset", 1);
                image.material = null;
                UnityEngine.Object.Destroy(material);
                Debug.Log("Sweep FX cancelled");
                return AsyncUnit.Default;
            }
        }

        #endregion

        public const float OnClickAnimTime = 0.08f;
        public const float OnPressAnimTime = 0.12f;
        public const float LongPressInterval = 0.12f;
        public const int MaxLongPressCount = 1;

        public const float smallScale = 0.85f;
        public const float bigScale = 1.1f;

        /// <summary>
        /// 基于UI框架的按钮通用动效：单击缩放和长按缩放 必须是xButton
        /// </summary>
        /// <param name="ui">有XButton按钮组件的ui</param>
        /// <param name="action">点击事件</param>
        /// <param name="audioId">音效id</param>
        /// <param name="onClickAnimTime">动效时间，默认0.15s</param>
        /// <param name="onPressAnimTime">动效时间，默认0.15s</param>
        /// <param name="coolDown">按钮冷却时间，默认0s</param>
        /// <typeparam name="T"></typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoScaleTweenOnClickAndLongPress(UI ui, UnityAction action = null, int audioId = 1102,
            float coolDown = 0,
            float onClickAnimTime = OnClickAnimTime,
            float onPressAnimTime = OnPressAnimTime, float smallScale = smallScale,
            float bigScale = bigScale)
        {
            var uiXBtn = ui.GetXButton();
            if (uiXBtn == null)
            {
                Log.Error($"{ui.Name}.GetXButton() is null");
                return;
            }

            //ui.GetRectTransform().SetScale(Vector3.one);
            uiXBtn.SetPointerActive(true);
            uiXBtn.SetLongPressInterval(LongPressInterval);
            uiXBtn.SetMaxLongPressCount(MaxLongPressCount);

            var scale = ui.GetRectTransform().Scale();


            uiXBtn.OnClick.Add(async () =>
            {
                ui.GetRectTransform().DoScale(scale * smallScale, onClickAnimTime).AddOnCompleted(
                    () =>
                    {
                        ui.GetRectTransform().DoScale(scale * bigScale, onClickAnimTime)
                            .AddOnCompleted(() => { ui.GetRectTransform().DoScale(scale, onClickAnimTime / 2f); });
                    });
                AudioManager.Instance.PlayFModAudio(audioId);
                //AudioManager.Instance.PlaySFXAudio("Click", true);
                //AudioManager.Instance.
                action?.Invoke();
                if (coolDown > 0)
                {
                    uiXBtn?.SetEnabled(false);
                    await UniTask.Delay((int)(coolDown * 1000f));
                    uiXBtn?.SetEnabled(true);
                }
            });

            uiXBtn.OnLongPress.Add((f) =>
            {
                ui.GetRectTransform().DoScale(scale * smallScale, onPressAnimTime);
                uiXBtn.OnPointerExit.Add(() =>
                {
                    ui.GetRectTransform().DoScale(scale * bigScale, onClickAnimTime).AddOnCompleted(() =>
                    {
                        ui.GetRectTransform().DoScale(scale, onClickAnimTime / 2f).AddOnCompleted(() => { });
                    });
                });
            });

            uiXBtn.OnLongPressEnd.Add((f) =>
            {
                ui.GetRectTransform().DoScale(scale * bigScale, onClickAnimTime).AddOnCompleted(() =>
                {
                    ui.GetRectTransform().DoScale(scale, onClickAnimTime / 2f);
                });
                AudioManager.Instance.PlayFModAudio(audioId);
                //AudioManager.Instance.PlaySFXAudio("Click", true);
                action?.Invoke();
            });
        }


        /// <summary>
        /// 基于UI框架的按钮通用动效：单击缩放和长按缩放 必须是xButton
        /// </summary>
        /// <param name="ui">有XButton按钮组件的ui</param>
        /// <param name="action">点击事件</param>
        /// <param name="onClickAnimTime">动效时间，默认0.15s</param>
        /// <param name="onPressAnimTime">动效时间，默认0.15s</param>
        /// <param name="coolDown">按钮冷却时间，默认0s</param>
        /// <typeparam name="T"></typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JiYuOnClickNoAnim(UI ui, UnityAction action = null, float coolDown = 0)
        {
            var uiXBtn = ui.GetXButton();
            if (uiXBtn == null)
            {
                Log.Error($"{ui.Name}.GetXButton() is null");
                return;
            }

            uiXBtn.SetPointerActive(true);
            uiXBtn.SetLongPressInterval(LongPressInterval);
            uiXBtn.SetMaxLongPressCount(MaxLongPressCount);

            uiXBtn.OnClick.Add(async () =>
            {
                action?.Invoke();
                if (coolDown > 0)
                {
                    uiXBtn?.SetEnabled(false);
                    await UniTask.Delay((int)(coolDown * 1000f));
                    uiXBtn?.SetEnabled(true);
                }
            });


            uiXBtn.OnLongPressEnd.Add((f) => { action?.Invoke(); });
        }

        /// <summary>
        /// 按钮的缩放
        /// </summary>
        /// <param name="obj">变化的对象</param>
        /// <param name="variation">变化值</param>
        /// <param name="startTime">正向时间</param>
        /// <param name="EndTime">反向时间</param>
        public static void GradualChange(GameObject obj, float variation, float startTime, float EndTime)
        {
            var InitScale = obj.transform.localScale;
            obj.transform.DOScale(obj.transform.localScale * variation, startTime).OnComplete(() =>
            {
                obj.transform.DOScale(InitScale, EndTime);
            });
        }


        /// <summary>
        /// 升级特效
        /// </summary>
        /// <param name="obj">变化的对象</param>
        /// <param name="changePosition">改变位置值</param>
        /// <param name="variation">改变缩放的值</param>
        /// <param name="startTime">正向时间</param>
        /// <param name="EndTime">反向时间</param>
        public static void LevelUp(GameObject obj, Vector3 changePosition, float variation, float startTime,
            float EndTime)
        {
            RectTransform objTransform = obj.GetComponent<RectTransform>();
            obj.transform.DOMove(objTransform.position + changePosition, startTime).SetEase(Ease.OutQuad)
                .OnComplete(() => { obj.transform.DOMove(objTransform.position, EndTime).SetEase(Ease.OutQuad); });


            obj.transform.DOScale(obj.transform.localScale * variation, startTime).OnComplete(() =>
            {
                obj.transform.DOScale(obj.transform.localScale, EndTime);
            });
        }

        /// <summary>
        /// 退出效果
        /// </summary>
        /// <param name="Imageobj">退出依赖的图片</param>
        /// <param name="Textobj">退出的文字</param>
        /// <param name="AlphaValue">退出文字的Alpha值</param>
        /// <param name="AlphaTime">退出文字从0变化到Alpha值的时间</param>
        /// <param name="IncreateFontSize">退出文字字体大小变化增量</param>
        /// <param name="SizeChageTime">增量时间</param>
        /// <param name="YchangeValue">整个退出Y轴变化值</param>
        /// <param name="startTime">正向时间</param>
        public static void ExitEffct(GameObject Imageobj, GameObject Textobj, float AlphaValue, float AlphaTime,
            float IncreateFontSize, float SizeChageTime, float YchangeValue, float startTime)
        {
            CanvasGroup canvasGroup = Imageobj.GetComponent<CanvasGroup>();
            TMP_Text text = Textobj.GetComponent<TMP_Text>();
            //获得字体大小
            float textSize = text.fontSize;
            //获得字体空间初始Y值
            float Inity = Imageobj.GetComponent<RectTransform>().position.y;
            //变化通道
            //TODO:
            // canvasGroup.DOFade(AlphaValue, AlphaTime);
            //字体大小变化
            DOTween.To(() => text.fontSize, x => text.fontSize = x, textSize + IncreateFontSize, SizeChageTime);
            //字体位置变化
            Imageobj.transform.DOMoveY(Inity + YchangeValue, startTime).OnComplete(() =>
            {
                canvasGroup.alpha = 0;
                Imageobj.transform.position =
                    new Vector3(Imageobj.transform.position.x, Inity, Imageobj.transform.position.z);
                //这里是字体瞬间复原
                text.fontSize = textSize;
            });
        }


        /// <summary>
        /// 移动动画
        /// </summary>
        /// <param name="obj1">对象1</param>
        /// <param name="obj2">对象2</param>
        /// <param name="EndPos1">终止位置1</param>
        /// <param name="EndPos2">终止位置2</param>
        /// <param name="durTime1">持续时间1</param>
        /// <param name="durTime2">持续时间2</param>
        /// <param name="isUseEndPos2">是否使用EndPos2</param>
        public static void MoveEffect(GameObject obj1, GameObject obj2, Vector3 EndPos1, Vector3 EndPos2,
            float durTime1, float durTime2 = 0, bool isUseEndPos2 = false)
        {
            RectTransform rectTransformObj = obj1.GetComponent<RectTransform>();
            if (!isUseEndPos2)
            {
                DOTween.To(() => rectTransformObj.anchoredPosition3D,
                    x => rectTransformObj.anchoredPosition3D = x,
                    EndPos1, durTime1);
            }
            else
            {
                RectTransform rectTransformObj2 = obj2.GetComponent<RectTransform>();
                DOTween.To(() => rectTransformObj.anchoredPosition3D, x => rectTransformObj.anchoredPosition3D = x,
                    EndPos1, durTime1).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    DOTween.To(() => rectTransformObj2.anchoredPosition3D,
                        x => rectTransformObj2.anchoredPosition3D = x,
                        EndPos2,
                        durTime2).SetEase(Ease.OutQuad);
                });
            }
        }


        //通用动画,框架
        public static void FadeInOut(UI ui, string key)
        {
            CanvasGroup canvasGroup = ui.GetFromReference(key).GetComponent<CanvasGroup>();


            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1.0f, 1).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    // Fade out
                    DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0.0f, 1).SetEase(Ease.Linear)
                        .OnComplete(() => { FadeInOut(ui, key); });
                });
        }


        //通用动画,不走框架
        public static void FadeInOut(GameObject obj)
        {
            var CanvasGroupComponent = obj.GetComponent<CanvasGroup>();


            DOTween.To(() => CanvasGroupComponent.alpha, x => CanvasGroupComponent.alpha = x, 1.0f, 1)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    // Fade out
                    DOTween.To(() => CanvasGroupComponent.alpha, x => CanvasGroupComponent.alpha = x, 0.0f, 1)
                        .SetEase(Ease.Linear).OnComplete(() => { FadeInOut(obj); });
                });
        }

        /// <summary>
        /// 黄金国修改
        /// 打开界面放大动画
        /// </summary>
        public static void OpenPanelScale(UI ui)
        {
            ui.GetRectTransform().SetScale(new Vector3(0, 0, 0));
            ui.GetRectTransform().DoScale(new Vector3(1, 1, 1), 0.15f);
        }
    }
}