//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using TMPro;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_ParasItem)]
    internal sealed class UISubPanel_ParasItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_ParasItem;

        public override bool IsFromPool => false;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_ParasItem>();
        }
    }

    public partial class UISubPanel_ParasItem : UI, IAwake<int>
    {
        public void Initialize(int index)
        {
            //黄金国101-200


            //伤害数字相关
            if (index >= 101 && index <= 200)
            {
                huangjinguoInit(index);
            }
        }

        private void huangjinguoInit(int i)
        {
            switch (i)
            {
                case 101:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("初始化大小");
                    break;
                case 102:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("最大大小");
                    break;
                case 103:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("消失时大小");
                    break;
                case 104:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("从开始到最大的时间");
                    break;
                case 105:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("维持最大状态的时间");
                    break;
                case 106:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("从最大到消失的时间");
                    break;
                case 107:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("初始化透明度");
                    break;
                case 108:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("最大时的透明度");
                    break;
                case 109:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("消失前的透明度");
                    break;
                case 110:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("随机生成的半径(范围0-1000)");
                    break;
                case 111:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("横向scale");
                    break;
                case 112:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("纵向scale");
                    break;
                case 113:
                    GetFromReference(KTxt_ItemName).GetTextMeshPro().SetTMPText("字间距参数(范围0-1000，初始为450)");
                    break;
            }
        }

        public string GetInputTxt()
        {
            var inputField_ = GetFromReference(KInputField_);

            //inputTxt_.GetTextMeshPro().SetTMPText(str);

            return inputField_.GetComponent<TMP_InputField>().text;
        }

        public void SetInputTxt(string str)
        {
            var inputField_ = GetFromReference(KInputField_);

            //inputTxt_.GetTextMeshPro().SetTMPText(str);
            inputField_.GetComponent<TMP_InputField>().text = str;


            //inputField_.
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}