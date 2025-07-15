using System;
using cfg.blobstruct;
using Unity.Entities;

namespace Main
{
    public struct GlobalConfigData : IComponentData, IDisposable
    {
        public BlobAssetReference<GenGenBlobAssetReference.ConfigData> value;

        public void Dispose()
        {
            value.Dispose();
        }
    }
}