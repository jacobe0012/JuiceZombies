using UnityEngine;
using UnityEngine.UI;

public class CustomHorizontalOrVerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
{
    // 重写父类的 `CalculateLayoutInputHorizontal` 方法，计算水平方向的布局
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        
        // 获取父物体的宽度
        float parentWidth = rectTransform.rect.width;
        
        // 计算子物体的总宽度（包括间距）
        float totalWidth = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            totalWidth += rectChildren[i].rect.width + spacing;  // 每个子物体宽度加上间距
        }
        
        // 如果子物体的总宽度超过父物体的宽度，按比例缩放
        if (totalWidth > parentWidth)
        {
            float scale = parentWidth / totalWidth;
            float adjustedSpacing = spacing * scale;

            // 调整间距（可以选择性地缩放间距）
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                child.sizeDelta = new Vector2(child.rect.width * scale, child.sizeDelta.y);
            }
            
            spacing = adjustedSpacing;  // 调整间距
        }
    }

    // 重写父类的 `CalculateLayoutInputVertical` 方法，计算垂直方向的布局
    public override void CalculateLayoutInputVertical()
    {
        //base.CalculateLayoutInputVertical();
        
        // 获取父物体的高度
        float parentHeight = rectTransform.rect.height;
        
        // 计算子物体的总高度（包括间距）
        float totalHeight = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            totalHeight += rectChildren[i].rect.height + spacing;  // 每个子物体高度加上间距
        }
        
        // 如果子物体的总高度超过父物体的高度，按比例缩放
        if (totalHeight > parentHeight)
        {
            float scale = parentHeight / totalHeight;
            float adjustedSpacing = spacing * scale;

            // 调整间距（可以选择性地缩放间距）
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                child.sizeDelta = new Vector2(child.sizeDelta.x, child.rect.height * scale);
            }
            
            spacing = adjustedSpacing;  // 调整间距
        }
    }

    // 重写父类的 `SetLayoutHorizontal` 方法，来按照水平方向排列子物体
    public override void SetLayoutHorizontal()
    {
        //base.SetLayoutHorizontal();

        float xOffset = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            
            // 设置子物体的位置
            child.anchoredPosition = new Vector2(xOffset, child.anchoredPosition.y);
            
            // 更新下一个子物体的位置
            xOffset += child.rect.width + spacing;
        }
    }

    // 重写父类的 `SetLayoutVertical` 方法，来按照垂直方向排列子物体
    public override void SetLayoutVertical()
    {
        //base.SetLayoutVertical();

        float yOffset = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            
            // 设置子物体的位置
            child.anchoredPosition = new Vector2(child.anchoredPosition.x, yOffset);
            
            // 更新下一个子物体的位置
            yOffset += child.rect.height + spacing;
        }
    }
}
