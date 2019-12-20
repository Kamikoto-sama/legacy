using System;
using ProviderProcessing.ProviderDatas;

namespace ProviderProcessing
{
    public interface IRepository
    {
        ProviderData FindByProviderId(Guid providerId);
        void RemoveById(Guid id);
        void Save(ProviderData data);
        void Update(ProviderData existingData);
    }
}