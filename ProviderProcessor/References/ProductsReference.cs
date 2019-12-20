using System;

namespace ProviderProcessing.References
{
    public class ProductsReference : IProductsReference
    {
        private static ProductsReference instance;

        public static ProductsReference GetInstance()
        {
            return instance ?? (instance = LoadReference());
        }

        public static ProductsReference LoadReference()
        {
            throw new NotImplementedException("Долгая-долгая инициализация справочника.");
        }

        public int? FindCodeByName(string name)
        {
            throw new NotImplementedException("Работа со справочником");
        }

        // Прочие методы
    }
}