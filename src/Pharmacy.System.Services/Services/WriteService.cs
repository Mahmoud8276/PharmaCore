using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using FluentValidation;
using Mapster;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public abstract class WriteService<TModel, TKey, TSpecificationParams, TCreateDto, TUpdateDto, TDetailsDto>
        : ReadService<TModel, TKey, TSpecificationParams, TDetailsDto>,
            IWriteService<TModel, TKey, TCreateDto, TUpdateDto, TSpecificationParams>
        where TModel : BaseModel<TKey>
        where TDetailsDto : class
        where TCreateDto : class
        where TUpdateDto : class
        where TSpecificationParams : IBaseSpecParams
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<TCreateDto>? _createValidator;
        private readonly IValidator<TUpdateDto>? _updateValidator;

        public WriteService(
            IGenericRepository<TModel, TKey> repository,
            IUnitOfWork unitOfWork,
            IValidator<TCreateDto>? createValidator = null,
            IValidator<TUpdateDto>? updateValidator = null)
            : base(repository)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Response> CreateAsync(TCreateDto dto)
        {
            if (_createValidator is not null)
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Response.Fail(
                        message: "Validation failed.",
                        details: string.Join("; ", validationResult.Errors));
                }
            }

            var entity = dto.Adapt<TModel>();

            var result = await BeforeCreateAsync(entity, dto);
            if (!result.IsSuccess)
                return result;


            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterCreateAsync(entity, dto);

            return Response.Created(data: entity.Adapt<TDetailsDto>());
        }

        public async Task<Response> UpdateAsync(TUpdateDto dto, TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Response.NotFound(message: "Resource not found.");

            if (_updateValidator is not null)
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Response.Fail(
                        message: "Validation failed.",
                        details: string.Join("; ", validationResult.Errors));
                }
            }

            var result = await BeforeUpdateAsync(entity, dto);
            if (!result.IsSuccess)
                return result;

            dto.Adapt(entity);

            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();

            await AfterUpdateAsync(entity, dto);

            return Response.Success(data: entity.Adapt<TDetailsDto>());
        }

        public async Task<Response> DeleteAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Response.NotFound(message: "Resource not found.");

            var result = await BeforeDeleteAsync(entity);
            if(!result.IsSuccess)
                return result;

            _repository.Delete(entity);
            await _unitOfWork.CompleteAsync();

            await AfterDeleteAsync(entity);

            return Response.Success(data: null, message: "Deleted successfully.");
        }



        protected virtual async Task<Response> BeforeCreateAsync(TModel entity, TCreateDto dto) => Response.Success();
        protected virtual Task AfterCreateAsync(TModel entity, TCreateDto dto) => Task.CompletedTask;
        protected virtual async Task<Response> BeforeUpdateAsync(TModel entity, TUpdateDto dto) => Response.Success();
        protected virtual  Task AfterUpdateAsync(TModel entity, TUpdateDto dto) => Task.CompletedTask;
        protected virtual async Task<Response> BeforeDeleteAsync(TModel entity) => Response.Success();
        protected virtual Task AfterDeleteAsync(TModel entity) => Task.CompletedTask;
    }
}

