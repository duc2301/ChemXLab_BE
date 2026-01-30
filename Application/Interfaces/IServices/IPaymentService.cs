using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.ResponseDTOs.Payment;

namespace Application.Interfaces.IServices
{
    public interface IPaymentService
    {
 
        Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentDTO request);
        Task<bool> ConfirmPaymentAsync(SePayWebhookDTO dto);
        Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync();
        Task<bool> CancelPaymentAsync(Guid id);
    }
}
