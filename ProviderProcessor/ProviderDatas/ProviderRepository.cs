using System;

namespace ProviderProcessing.ProviderDatas
{
    public class ProviderRepository: IRepository
    {
        public ProviderData FindByProviderId(Guid providerId)
        {
            throw new NotImplementedException("Работа с базой данных");
        }

        public void RemoveById(Guid id)
        {
            throw new NotImplementedException("Работа с базой данных");
        }

        public void Save(ProviderData data)
        {
            throw new NotImplementedException("Работа с базой данных");
        }

        public void Update(ProviderData existingData)
        {
            throw new NotImplementedException("Работа с базой данных");
        }
    }
}