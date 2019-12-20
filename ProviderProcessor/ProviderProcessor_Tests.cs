using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FakeItEasy;
using Newtonsoft.Json;
using NUnit.Framework;
using ProviderProcessing.ProviderDatas;
using ProviderProcessing.References;
using Samples.Reporters;

namespace ProviderProcessing
{
    [TestFixture]
    public class ProviderProcessor_Tests
    {
        private IRepository providerRepository;
        private ProviderData providerData;
        private IProductsReference productsReference;
        private IProductsReferenceBuilder productsReferenceBuilder;
        private ProviderProcessor providerProcessor;
        private IMeasureUnitsReference measureUnitsReference;
        private IMeasureUnitsReferenceBuilder measureUnitsReferenceBuilder;
        private ProductData goodProduct;
        private ProductData productWithInvalidName;
        private ProductData productWithInvalidMeasureUnit;
        private ProductData productWithInvalidPrice;

        private Dictionary<string, ProductData> products;

        [SetUp]
        public void SetUp()
        {
            providerRepository = A.Fake<IRepository>();
            providerData = new ProviderData
            {
                Id = Guid.Parse("666f730d-876b-42e4-a59b-254aba887f9d"),
                ProviderId = Guid.Parse("268798db-72c9-4246-98bb-6be06d88090c"),
                Products = new ProductData[0],
                Timestamp = DateTime.FromFileTimeUtc(1576674756L)
            };
            A.CallTo(() => providerRepository.FindByProviderId(Guid.Empty))
                .WithAnyArguments()
                .Returns(providerData);
            A.CallTo(() => providerRepository.RemoveById(Guid.Empty))
                .WithAnyArguments();
            A.CallTo(() => providerRepository.Save(providerData))
                .WithAnyArguments();
            A.CallTo(() => providerRepository.Update(providerData))
                .WithAnyArguments();

            productsReference = A.Fake<IProductsReference>();
            A.CallTo(() => productsReference.FindCodeByName(""))
                .WithAnyArguments()
                .Returns(null);

            productsReferenceBuilder = A.Fake<IProductsReferenceBuilder>();
            A.CallTo(() => productsReferenceBuilder.GetInstance())
                .Returns(productsReference);
            
            measureUnitsReference = A.Fake<IMeasureUnitsReference>();
            A.CallTo(() => measureUnitsReference.FindByCode(null))
                .WithAnyArguments()
                .Returns(null);
            measureUnitsReferenceBuilder = A.Fake<IMeasureUnitsReferenceBuilder>();
            A.CallTo(() => measureUnitsReferenceBuilder.GetInstance())
                .Returns(measureUnitsReference);
            A.CallTo(() => productsReference.FindCodeByName("Banana"))
                .Returns(42);
            A.CallTo(() => measureUnitsReference.FindByCode("u"))
                .Returns(new MeasureUnit());
            
            goodProduct = new ProductData
            {
                Id = Guid.Parse("4279c077-b7fb-4aed-bf35-5794ab327ce7"),
                MeasureUnitCode = "u",
                Name = "Banana",
                Price = 10.000m
            };
            
            productWithInvalidName = new ProductData
            {
                Id = Guid.Parse("4279c077-b7fb-4aed-bf35-5794ab327ce7"),
                MeasureUnitCode = "u",
                Name = "Apple",
                Price = 10.000m
            };

            productWithInvalidMeasureUnit = new ProductData
            {
                Id = Guid.Parse("4279c077-b7fb-4aed-bf35-5794ab327ce7"),
                MeasureUnitCode = "m",
                Name = "Banana",
                Price = 10.000m
            };
            
            productWithInvalidPrice = new ProductData
            {
                Id = Guid.Parse("4279c077-b7fb-4aed-bf35-5794ab327ce7"),
                MeasureUnitCode = "u",
                Name = "Banana",
                Price = -10.000m
            };
            
            products = new Dictionary<string, ProductData>
            {
                {"good", goodProduct},
                {"name", productWithInvalidName},
                {"unit", productWithInvalidMeasureUnit},
                {"price", productWithInvalidPrice}
            };
            
            providerProcessor = new ProviderProcessor(providerRepository, 
                productsReferenceBuilder, 
                measureUnitsReferenceBuilder, 
                new ProductValidator(productsReferenceBuilder, measureUnitsReferenceBuilder));
        }

        
        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void Fail_IfDataNotExists()
        { 
            var providerData = new ProviderData();
            var serializedData = JsonConvert.SerializeObject(providerData);
            
            var processReport = providerProcessor.ProcessProviderData(serializedData);
            Approvals.Verify(processReport);
        }

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void Success_IfDataExists()
        {
            var serializedData = JsonConvert.SerializeObject(providerData);

            var processReport = providerProcessor.ProcessProviderData(serializedData);
            Approvals.Verify(processReport);
        }
        
        
        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void SucceedsWithProduct()
        {
            providerData.Products = new[] {goodProduct};
            var serializedData = JsonConvert.SerializeObject(providerData);
            var processReport = providerProcessor.ProcessProviderData(serializedData);
            Approvals.Verify(processReport);
        }

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void FailsProductNameInvalid()
        {
            providerData.Products = new[] {productWithInvalidName};
            var serializedData = JsonConvert.SerializeObject(providerData);
            var processReport = providerProcessor.ProcessProviderData(serializedData);
            Approvals.Verify(processReport);
        }
        
        
        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void FailsProductMeasureUnitInvalid()
        {
            providerData.Products = new[] {productWithInvalidMeasureUnit};
            var serializedData = JsonConvert.SerializeObject(providerData);
            var processReport = providerProcessor.ProcessProviderData(serializedData);
            Approvals.Verify(processReport);
        }
        
        
        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void FailsProductPriceInvalid()
        {
            providerData.Products = new[] {productWithInvalidPrice};
            var serializedData = JsonConvert.SerializeObject(providerData);
            var processReport = providerProcessor.ProcessProviderData(serializedData);
            Approvals.Verify(processReport);
        }

        [Test, Combinatorial]
        [UseReporter(typeof(DiffReporter))]
        public void CombinationsTest(
            [Values("good", "name", "unit", "price")] string a,
            [Values("good", "name", "unit", "price")] string b,
            [Values("good", "name", "unit", "price")] string c)
        {
            using (ApprovalResults.ForScenario(a, b, c))
            {
                var productA = products[a];
                var productB = products[b];
                var productC = products[c];
                providerData.Products = new[] {productA, productB, productC};
                var serializedData = JsonConvert.SerializeObject(providerData);
                var processReport = providerProcessor.ProcessProviderData(serializedData);
                Approvals.Verify(processReport);
            }
        }
    }
}