using Pharmacy.System.Services.ApplicatoinServicesConfig;
using System.Runtime.CompilerServices;

namespace Pharmacy.System.UnitTests
{
    internal static class TestAssemblyInitializer
    {
        [ModuleInitializer]
        public static void Init() => MappingProfiles.Configure("https://localhost");
    }
}