using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.ResponseDTOs.Payment;

namespace Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync();
        Task<PaymentResponseDTO> GetPaymentByIdAsync(Guid paymentId);
        Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentDTO paymentDto);
        Task<PaymentResponseDTO> ConfirmPaymentAsync(Guid paymentId);
    }
}
