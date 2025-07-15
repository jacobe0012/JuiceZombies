using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine.SceneManagement;

namespace Main
{
    public struct SwitchSceneData : IComponentData
    {
        public WeakObjectSceneReference scene;
        public bool startedLoad;
        public Scene myscene;
    }
}