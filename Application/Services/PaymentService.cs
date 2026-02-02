using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.RequestDTOs.Sepay;
using Application.DTOs.ResponseDTOs.Payment;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Options;
using PayOS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var transactionCode = $"CX_{Guid.NewGuid():N}".Substring(0, 10);

            var payment = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PackageId = request.PackageId,
                Amount = request.Amount,
                Currency = "VND",
                PaymentMethod = "SEPAY",
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
            var payment = await _unitOfWork.PaymentRepository
                .GetByIdAsync(dto.TransactionId);

            if (payment == null) return false;
            if (payment.Status == "SUCCESS") return true;
            if (payment.Amount != dto.Amount) return false;
            if (dto.Status != "SUCCESS") return false;

            payment.Status = "SUCCESS";
            payment.PaidAt = dto.TransactionDate;
            //payment.TransactionCode = dto.TransactionId;

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            await ActivateSubscription(payment);

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
    }
}