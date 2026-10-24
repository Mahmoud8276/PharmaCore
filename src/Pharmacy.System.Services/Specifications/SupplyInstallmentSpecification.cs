using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Services.Specifications
{
    public class SupplyInstallmentSpecification : BaseSpecification<SupplyInstallment, int>
    {
        public SupplyInstallmentSpecification(int installmentId) : base(x=>x.Id ==  installmentId)
        {
            AddInclude(x => x.Payments);
        }
    }
}
