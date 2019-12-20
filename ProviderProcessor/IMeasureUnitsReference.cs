using ProviderProcessing.References;

namespace ProviderProcessing
{
    public interface IMeasureUnitsReference
    {
        MeasureUnit FindByCode(string measureUnitCode);
    }
}