using System.Collections.Generic;
using System.Threading.Tasks;
using Collector.BL.Models.Debt;

namespace Collector.BL.Services.DebtsService
{
    public interface IDebtService
    {
        Task<DebtReturnDTO> AddDebtAsync(DebtAddDTO model);
        Task<IList<DebtReturnDTO>> GetAllDebtsAsync();
        Task RemoveDebtAsync(long debtId);
        Task UpdateDebtAsync(DebtUpdateDTO model);
    }
}