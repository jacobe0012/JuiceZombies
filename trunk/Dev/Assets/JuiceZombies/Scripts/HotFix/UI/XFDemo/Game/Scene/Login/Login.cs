using System.Collections.Generic;

namespace XFramework
{
    public class Login : LoadingScene
    {
        public override void GetObjects(ICollection<string> objKeys)
        {
        }

        async protected override void OnCompleted()
        {
            UIHelper.CreateAsync(UIType.UIPanel_Login);
            //LoadDllMono.
            //Log.Error($"Login");
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}