using AutoMapper;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Inventory
{
    public class UnitofMeasureService
    {
        private readonly IUnitofMeasureRepository _unitofMeasureRepository;
        private readonly IMapper _mapper;
        public UnitofMeasureService(IUnitofMeasureRepository unitofMeasureRepository, IMapper mapper)
        {
            _unitofMeasureRepository = unitofMeasureRepository;
            _mapper = mapper;
        }
        private async Task ValidateUnitOfMeasureId(int unitOfMeasureID)
        {
            if (unitOfMeasureID <= 0)
            {
                throw new ArgumentException("Invalid Unit Of Measure Id. It must be a positive integer.");
            }
            bool doesItemClassExist = await _unitofMeasureRepository.DoesUnitOfMeasureExist(unitOfMeasureID);
            if (!doesItemClassExist)
            {
                throw new ItemNotFoundException($"Unit Of Measure with with Id {unitOfMeasureID} not found.");
            }
        }
        public async Task<UnitOfMeasureDto> CreateUpdateUnitOfMeasureAsync(UnitOfMeasureDto unitOfMeasureDto)
        {

            UnitOfMeasure? unitOfMeasure = null;

            if (unitOfMeasureDto.UnitOfMeasureID == 0)//Create
            {
                unitOfMeasure = await _unitofMeasureRepository.CreateUnitOfMeasureAsync(_mapper.Map<UnitOfMeasure>(unitOfMeasureDto));
            }
            else//update
            {
                unitOfMeasure = await _unitofMeasureRepository.UpdateUnitOfMeasureAsync(_mapper.Map<UnitOfMeasure>(unitOfMeasureDto));
            }
            if (unitOfMeasure == null)
            {
                throw new ActionFailedException("Could not Create/Update Item Category.");
            }
            return _mapper.Map<UnitOfMeasureDto>(unitOfMeasure);
        }

        public async Task<UnitOfMeasureDto> GetUnitOfMeasureDetailsAsync(int unitOfMeasureID)
        {
            await ValidateUnitOfMeasureId(unitOfMeasureID);
            UnitOfMeasure? unitOfMeasure = await _unitofMeasureRepository.GetUnitOfMeasureDetailsAsync(unitOfMeasureID);
            if (unitOfMeasure == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<UnitOfMeasureDto>(unitOfMeasure);
        }

        public async Task DeleteUnitOfMeasureAsync(int unitOfMeasureID)
        {
            await ValidateUnitOfMeasureId(unitOfMeasureID);
            bool isItemCategoryDeleted = await _unitofMeasureRepository.DeleteUnitOfMeasureAsync(unitOfMeasureID);
            if (!isItemCategoryDeleted)
            {
                throw new ActionFailedException("Could not delete Item Category. Try again later.");
            }
        }
    }
}
