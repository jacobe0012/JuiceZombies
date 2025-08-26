//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-18 17:15:42
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main
{
    public class SwitchSceneAuthoring : MonoBehaviour
    {
        public WeakObjectSceneReference scene;
        public Scene myscene;

        public class SwitchSceneAuthoringBaker : Baker<SwitchSceneAuthoring>
        {
            public override void Bake(SwitchSceneAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SwitchSceneData
                {
                    scene = authoring.scene,
                    startedLoad = false,
                    myscene = authoring.myscene,
                });
            }
        }
    }

    public struct StartToLoadSceneTag : IComponentData
    {
    }
}