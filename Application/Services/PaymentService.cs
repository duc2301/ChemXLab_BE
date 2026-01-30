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

        public async Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            return payments.Select(p => _mapper.Map<PaymentResponseDTO>(p));
        }

        public async Task<bool> CancelPaymentAsync(Guid id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null) return false;

            payment.Status = "CANCELLED";

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task ActivateSubscription(PaymentTransaction payment)
        {
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
