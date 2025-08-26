//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Resource)]
    internal sealed class UICommon_ResourceEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_Resource;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Resource>();
        }
    }

    public partial class UICommon_Resource : UI, IAwake<string>
    {
        //��ʾ�����ʱ��
        public float KeepTime = 4.0f;

        public void Initialize(string str)
        {
            CreateProperty(str);
        }

        private async void CreateProperty(string str)
        {
            //��������
            var container = GetFromReference(KContainer);
            //��������λ�ô�����ʾ��
            var ui = await UIHelper.CreateAsync(this, UIType.UICommon_Prompt, str, container.GameObject.transform);
            //UnicornTweenHelper.SetEaseAlphaAndPosLtoR(container, 0, 500);
            //ά����ʾ�������ر�UICommom_Resource
            await UniTask.Delay((int)(1000 * KeepTime));
            Close();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}