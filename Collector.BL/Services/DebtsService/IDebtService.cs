using System.Collections.Generic;
using System.Threading.Tasks;
using Collector.BL.Models.Debt;

namespace Collector.BL.Services.DebtsService
{
    public interface IDebtService
    {
        Task<DebtReturnDTO> AddDebtAsync(DebtAddDTO model);
        Task<IList<DebtReturnDTO>> GetAllDebtsAsync(DebtSearchObjectDTO model);
        Task RemoveDebtAsync(long debtId);
        Task<DebtReturnDTO> UpdateDebtAsync(DebtUpdateDTO model);
        Task<DebtReturnDTO> GetDebtByIdAsync(long id);
        Task<ChangesReturnDTO> GetDebtChangesByIdAsync(ChangeSearchDTO model);
        Task DebtPayAsync(DebtPayDTO model);
        Task<IList<PayNotificationReturnDTO>> GetPayNotificationsAsync();
        Task AcceptPayNotificationAsync(long id);
        Task DenyPayNotificationAsync(long id);
        Task<IList<CurrencyReturnDTO>> GetCurrenciesAsync();
        Task<IList<PaymentDTO>> GetPaymentsAsync();

    }
}