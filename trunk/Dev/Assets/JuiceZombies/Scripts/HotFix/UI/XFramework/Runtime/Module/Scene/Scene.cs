using Cysharp.Threading.Tasks;
using Main;

namespace XFramework
{
    public class Scene : XObject, IUpdate, ILateUpdate, IFixedUpdate
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string Name { get; protected set; }

        protected SceneObject sceneObject;

        /// <summary>
        /// �����Ƿ�������
        /// </summary>
        protected bool isCompleted;

        public void Init(string name, SceneObject sceneObject)
        {
            this.Name = name;
            this.sceneObject = sceneObject;
            this.isCompleted = false;
            this.WaitForCompleted().ToCoroutine();
        }

        protected sealed override void OnStart()
        {
            base.OnStart();
        }

        public void Update()
        {
            if (!this.isCompleted)
                return;

            this.OnUpdate();
        }

        public void LateUpdate()
        {
            if (!this.isCompleted)
                return;

            this.OnLateUpdate();
        }

        public void FixedUpdate()
        {
            if (!this.isCompleted)
                return;

            this.OnFixedUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }

        protected virtual void OnFixedUpdate()
        {
        }

        /// <summary>
        /// �ȴ������������
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask WaitForCompleted()
        {
            var tagId = this.TagId;
            await SceneResManager.WaitForCompleted(this.sceneObject);
            if (tagId != this.TagId)
                return;

            this.isCompleted = true;
            this.OnCompleted();
        }

        /// <summary>
        /// �������������ʱִ��
        /// </summary>
        protected virtual void OnCompleted()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.sceneObject = null;
            this.isCompleted = false;
            UIHelper.Clear();
            ResourcesManager.UnloadUnusedAssets();
            UnityHelper.BeginTime();
        }
    }
}