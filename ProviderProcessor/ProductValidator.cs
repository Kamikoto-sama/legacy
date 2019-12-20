using System.Collections.Generic;
using System.Linq;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;
using ProviderProcessing.References;

namespace ProviderProcessing
{
    public class ProductValidator
    {
        private readonly IProductsReferenceBuilder productsReferenceBuilder;
        private readonly IMeasureUnitsReferenceBuilder measureUnitsReferenceBuilder;

        public ProductValidator(IProductsReferenceBuilder productsReferenceBuilder,
            IMeasureUnitsReferenceBuilder measureUnitsReferenceBuilder)
        {
            this.productsReferenceBuilder = productsReferenceBuilder;
            this.measureUnitsReferenceBuilder = measureUnitsReferenceBuilder;
        }
        
        public IList<ProductValidationResult> ValidateProduct(ProductData product)
        {
            return ValidateName(product)
                .Concat(ValidatePrice(product))
                .Concat(ValidateMeasureUnitCode(product))
                .ToList();
        }

        private IEnumerable<ProductValidationResult> ValidateMeasureUnitCode(ProductData product)
        {
            var reference = measureUnitsReferenceBuilder.GetInstance();
            if (reference.FindByCode(product.MeasureUnitCode) == null)
                yield return new ProductValidationResult(product,
                    "Bad units of measure", ProductValidationSeverity.Warning);
        }

        private IEnumerable<ProductValidationResult> ValidatePrice(ProductData product)
        {
            if (product.Price <= 0)
                yield return new ProductValidationResult(product,
                    "Bad price", ProductValidationSeverity.Warning);
        }

        private IEnumerable<ProductValidationResult> ValidateName(ProductData product)
        {
            var reference = productsReferenceBuilder.GetInstance();
            if (!reference.FindCodeByName(product.Name).HasValue)
                yield return new ProductValidationResult(product,
                    "Unknown product name", ProductValidationSeverity.Error);
        }
    }
}