using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.SupplierDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.Helpers;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierService> _logger;
        public SupplierService(
            IUnitOfWork unitOfWork,
            ILogger<SupplierService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<Response> CreateAsync(SupplierDto dto)
        {
            if(await _unitOfWork.SupplierRepository.AnyAsync(x=>x.Name == dto.Name))
            {
                return Response.Fail(
                    message: "There is already a supplier with this name!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            if(dto.Email != null &&
                await _unitOfWork.SupplierRepository.AnyAsync(x=>x.Email == dto.Email))
            {
                return Response.Fail(
                    message: "There is already a supplier with this email!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            if(dto.PhoneNumbers.Count > 0 &&
                await _unitOfWork.SupplierRepository.AnyAsync(
                x=>x.PhoneNumbers.Any(
                    p => dto.PhoneNumbers.Contains(p.Number))))
            {
                return Response.Fail(
                    message: "One or more phone numbers already belong to a supplier!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }


            var supplier = dto.Adapt<Supplier>();
            supplier.Logo = null;

            if (dto.Logo != null)
            {
                try
                {
                    supplier.Logo = await FileHelper.UploadFileAsync(dto.Logo, "SupplierImages");
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to upload logo file for the created supplier.");
                    return Response.Fail(
                        message: "Failed to upload the supplier logo!",
                        statusCode: (int)HttpStatusCode.BadRequest);
                }
            }

            try
            {
                await _unitOfWork.SupplierRepository.AddAsync(supplier);
                await _unitOfWork.CompleteAsync();
            }
            catch
            {
                if(supplier.Logo != null)
                {
                    try
                    {
                        await FileHelper.DeleteFileAsync(supplier.Logo, "SupplierImages");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to upload logo file for the created supplier.");
                    }
                }
                throw;
            }

            return Response.Success(
                message: "Supplier created successfully",
                statusCode: (int)HttpStatusCode.Created,
                data: supplier.Adapt<SupplierDetailsDto>());
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return Response.Fail(
                    message: "Supplier Not Found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }

            if (await _unitOfWork.SupplyRepository.AnyAsync(x => x.SupplierId == id))
            {
                return Response.Fail(
                    message: "Cannot delete this supplier, because it is associated with one or more supplies!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            var logo = supplier.Logo;

            try
            {
                _unitOfWork.SupplierRepository.Delete(supplier);
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Delete blocked by FK for supplier {Id}", id);
                return Response.Fail(
                    message: "Cannot delete this supplier, because it is referenced by other records!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            if (logo != null)
            {
                try
                {
                    await FileHelper.DeleteFileAsync(logo, "SupplierImages");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete logo file {Logo} for supplier {Id}", logo, id);
                }
            }

            return Response.Success(message: "Supplier deleted successfully");
        }
        
        public async Task<Response> GetAllAsync(SupplierSpecParams specParams)
        {
            var countSpec = new SupplierSpecification(specParams, true);
            var count = await _unitOfWork.SupplierRepository.GetCountWithSpecAsync(countSpec);

            var spec = new SupplierSpecification(specParams);
            var data = await _unitOfWork.SupplierRepository.GetAllWithSpecAsync(spec);

            var pagination = new Pagination(
                specParams.PageIndex,
                specParams.PageSize, 
                count,
                data.Adapt<List<SupplierDetailsDto>>());

            return Response.Success(
                message: "Suppliers data retrieved successfully",
                data: pagination);
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetWithSpecAsync(
                new SupplierSpecification(id));
            if (supplier == null)
            {
                return Response.Fail(
                    message: "Supplier Not Found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }

            return Response.Success(
                message: "Supplier data retrieved Successfully",
                data: supplier.Adapt<SupplierDetailsDto>());
        }

        public async Task<Response> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            if (await _unitOfWork.SupplierRepository.AnyAsync(x => x.Id != id && x.Name == dto.Name))
            {
                return Response.Fail(
                    message: "There is already a supplier with this name!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            if (dto.Email != null &&
                await _unitOfWork.SupplierRepository.AnyAsync(x => x.Id != id && x.Email == dto.Email))
            {
                return Response.Fail(
                    message: "There is already a supplier with this email!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            if (dto.PhoneNumbers.Count > 0 &&
                await _unitOfWork.SupplierRepository.AnyAsync(
                x => x.Id != id && x.PhoneNumbers.Any(
                    p => dto.PhoneNumbers.Contains(p.Number))))
            {
                return Response.Fail(
                    message: "One or more phone numbers already belong to a supplier!",
                    statusCode: (int)HttpStatusCode.Conflict);
            }


            var supplier = await _unitOfWork.SupplierRepository.GetWithSpecAsync(
                new SupplierSpecification(id));
            if (supplier == null)
            {
                return Response.Fail(
                    message: "Supplier Not Found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }

            supplier.Name = dto.Name.Trim();
            supplier.Description = dto.Description?.Trim();
            supplier.Email = dto.Email?.Trim();
            supplier.Address = dto.Address?.Trim();

            var incoming = dto.PhoneNumbers
                .Select(p => p.Trim())
                .Where(n => n.Length > 0)
                .Distinct()
                .ToList();

            supplier.PhoneNumbers.RemoveAll(p => !incoming.Contains(p.Number));

            var existing = supplier.PhoneNumbers.Select(p => p.Number).ToHashSet();
            foreach (var number in incoming.Where(n => !existing.Contains(n)))
            {
                supplier.PhoneNumbers.Add(new SupplierPhoneNumber { Number = number });
            }

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Failed to update supplier {Id}", id);
                return Response.Fail(
                    message: "Could not update the supplier. Check for duplicate data.",
                    statusCode: (int)HttpStatusCode.Conflict);
            }

            return Response.Success(
                message: "Supplier updated successfully",
                data: supplier.Adapt<SupplierDetailsDto>());
        }
        
        public async Task<Response> UpdateLogoAsync(int id, UpdateSupplierLogoDto dto)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return Response.Fail(
                    message: "Supplier Not Found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }

            var oldLogo = supplier.Logo;

            var newLogo = await FileHelper.UploadFileAsync(dto.Logo, "SupplierImages");
            supplier.Logo = newLogo;

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch
            {
                await FileHelper.DeleteFileAsync(newLogo, "SupplierImages");
                throw;
            }

            if (oldLogo != null)
            {
                await FileHelper.DeleteFileAsync(oldLogo, "SupplierImages");
            }

            return Response.Success(message: "Supplier Image Updated Successfully");
        }

        public async Task<Response> DeleteLogoAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);
            if(supplier == null)
            {
                return Response.Fail(
                    message: "Supplier Not Found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }

            if(supplier.Logo == null)
            {
                return Response.Fail(
                    message: "No Supplier Logo Image!");
            }

            var oldLogo = supplier.Logo;

            supplier.Logo = null;
            await _unitOfWork.CompleteAsync();

            try
            {
                await FileHelper.DeleteFileAsync(oldLogo, "SupplierImages");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete logo file {Logo} for supplier {Id}", oldLogo, id);
            }

            return Response.Success(message: "Supplier Logo Deleted Successfully");
        }
    }
}
