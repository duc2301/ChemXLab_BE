using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.RequestDTOs.Sepay;
using Application.DTOs.ResponseDTOs.Payment;
using Application.ExceptionMidleware;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Web;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for handling payment transactions and integration with the SePay gateway.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly SePaySettings _sePaySettings;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptions<SePaySettings> sePayOptions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sePaySettings = sePayOptions.Value;
        }

        /// <summary>
        /// Initiates a new payment transaction and generates a QR code URL.
        /// </summary>
        /// <param name="request">The payment creation details.</param>
        /// <returns>The created payment transaction details including the QR URL.</returns>
        public async Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentDTO request)
        {
            var paymentId = Guid.NewGuid();
            var transactionCode = $"ChemXLab_{paymentId}";
            var package = await _unitOfWork.PackageRepository.GetByIdAsync(request.PackageId);

            var payment = new PaymentTransaction
            {
                Id = paymentId,
                UserId = request.UserId,
                PackageId = request.PackageId,
                Amount = (int)package.Price.Value,
                Currency = "VND",
                PaymentMethod = "Bank Transfer",
                Status = "PENDING",
                TransactionCode = transactionCode,
                Description = $"Thanh toan goi {request.PackageId}",
                CreatedAt = DateTime.Now,
            };

            payment.Qrurl = GenerateSePayQr(payment);

            await _unitOfWork.PaymentRepository.CreateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentResponseDTO>(payment);
        }

        /// <summary>
        /// Generates the SePay QR code URL based on transaction details.
        /// </summary>
        private string GenerateSePayQr(PaymentTransaction payment)
        {
            var encodedDesc = HttpUtility.UrlEncode(payment.TransactionCode);

            return $"{_sePaySettings.QrBaseUrl}" +
                   $"?bank={_sePaySettings.BankCode}" +
                   $"&acc={_sePaySettings.AccountNumber}" +
                   $"&amount={payment.Amount}" +
                   $"&des={encodedDesc}" +
                   $"&template=compact";
        }

        /// <summary>
        /// Verifies and confirms a payment based on the Webhook data received from SePay.
        /// </summary>
        /// <param name="dto">The webhook data payload.</param>
        /// <returns>True if the payment is successfully confirmed, otherwise False.</returns>
        public async Task<bool> ConfirmPaymentAsync(SePayWebhookDTO dto)
        {
            PaymentTransaction? matchedPayment = null;
            var match = Regex.Match(dto.Content, @"ChemXLab_?([a-zA-Z0-9-]+)", RegexOptions.IgnoreCase);            

            if (match.Success)
            {
                var extractedIdString = match.Groups[1].Value;
                if (Guid.TryParse(extractedIdString, out Guid paymentId))
                {
                    matchedPayment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                }
            }

            if (matchedPayment == null)
            {
                var potentialPayments = await _unitOfWork.PaymentRepository.GetPendingPaymentsByAmountAsync(dto.TransferAmount);

                if (potentialPayments.Any())
                {
                    matchedPayment = potentialPayments.OrderBy(p => p.CreatedAt).First();
                }
            }

            if (matchedPayment == null)
            {
                return false;
            }

            if (matchedPayment.Status == "PAID") return true;

            matchedPayment.Status = "PAID";
            matchedPayment.PaidAt = dto.TransactionDate; 

            _unitOfWork.PaymentRepository.Update(matchedPayment);
            await _unitOfWork.SaveChangesAsync();

            // Kích hoạt dịch vụ
            await ActivateSubscription(matchedPayment);

            return true;
        }

        /// <summary>
        /// Retrieves the history of all payment transactions.
        /// </summary>
        /// <returns>A collection of payment records.</returns>
        public async Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            return payments.Select(p => _mapper.Map<PaymentResponseDTO>(p));
        }

        /// <summary>
        /// Cancels a pending payment transaction.
        /// </summary>
        /// <param name="id">The unique identifier of the payment to cancel.</param>
        /// <returns>True if cancellation was successful, otherwise False.</returns>
        public async Task<bool> CancelPaymentAsync(Guid id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null) return false;

            payment.Status = "CANCELLED";

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Activates the user's subscription upon successful payment.
        /// </summary>
        private async Task ActivateSubscription(PaymentTransaction payment)
        {
            // Logic to activate subscription (currently commented out)
            //var package = await _unitOfWork.PackageRepository
            //    .GetByIdAsync(payment.PackageId);

            //var subscription = new Subscription
            //{
            //    Id = Guid.NewGuid(),
            //    UserId = payment.UserId,
            //    PackageId = payment.PackageId,
            //    StartDate = DateTime.Now,
            //    EndDate = DateTime.Now.AddDays(package.DurationDays),
            //    IsActive = true
            //};

            //await _unitOfWork.SubscriptionRepository.CreateAsync(subscription);
            //await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetMyTransaction(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ApiExceptionResponse("User ID cannot be empty.");
            }
            var payment = await _unitOfWork.PaymentRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<PaymentResponseDTO>>(payment);
        }
    }
}