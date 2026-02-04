using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.ResponseDTOs.Payment;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service for processing payments and handling transactions.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Initiates a new payment transaction.
        /// </summary>
        /// <returns>The payment transaction details, including QR code or payment link.</returns>
        Task<PaymentResponseDTO> CreatePaymentAsync(Guid userId, int packageId);

        /// <summary>
        /// Processes a payment confirmation webhook from the payment gateway (SePay).
        /// </summary>
        /// <returns>True if the payment was successfully confirmed.</returns>
        Task<bool> ConfirmPaymentAsync(SePayWebhookDTO dto);

        /// <summary>
        /// Retrieves a history of all payment transactions.
        /// </summary>
        /// <returns>A collection of payment records.</returns>
        Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync();

        /// <summary>
        /// Cancels a pending payment transaction.
        /// </summary>
        /// <returns>True if the cancellation was successful.</returns>
        Task<bool> CancelPaymentAsync(Guid id);
        Task<IEnumerable<PaymentResponseDTO>> GetMyTransaction(Guid userId);
    }
}