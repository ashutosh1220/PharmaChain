using PharmaChain.Application.DTOs;

namespace PharmaChain.Application.Interfaces
{
    public interface IMedicineService
    {
        Task<MedicineResponse> CreateMedicine(MedicineRequest request);
        Task<MedicineResponse> UpdateMedicine(MedicineRequest request);
        Task <MedicineResponse> DeleteMedicine(string id);
        Task<MedicineListResponse> GetMedicinesAsync(int page, int size);
        Task<MedicineResponse> ToggleActiveMedicine(string id);
        Task<SingleMedicineListResponse> GetMedicineByIdAsync(string id);
    }
}
