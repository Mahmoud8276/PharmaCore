using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace Pharmacy.System.Services.Specifications
{
    public class SupplierSpecification : BaseSpecification<Supplier, int>
    {
        public SupplierSpecification(ISupplierSpecParams specParams, bool isCount = false) : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
                AddOrderBy(x => x.Name);
                AddInclude(x => x.PhoneNumbers);
            }

        }

        public SupplierSpecification(int supplierId) : base(x=>x.Id == supplierId)
        {
            AddInclude(x => x.PhoneNumbers);
        }

        private static Expression<Func<Supplier, bool>> BuildCriteria(ISupplierSpecParams specParams)
        {
            var name = specParams.Name;
            var description = specParams.Description;
            var email = specParams.Email;
            var address = specParams.Address;
            var phone = specParams.PhoneNumber;

            return x => (
            (string.IsNullOrEmpty(specParams.Name) || x.Name.ToLower().Contains(name)) &&
            (string.IsNullOrEmpty(specParams.Description) || x.Description.ToLower().Contains(description)) &&
            (string.IsNullOrEmpty(specParams.Email) || x.Email.ToLower().Contains(email)) &&
            (string.IsNullOrEmpty(specParams.Address) || x.Address.ToLower().Contains(address)) &&
            (string.IsNullOrEmpty(specParams.PhoneNumber) || x.PhoneNumbers.Any(ph => ph.Number.Contains(phone))));
        }
    }
}
