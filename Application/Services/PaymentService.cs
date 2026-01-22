using Application.DTOs.RequestDTOs.Payment;
using Application.DTOs.ResponseDTOs.Payment;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PaymentResponseDTO> ConfirmPaymentAsync(Guid paymentId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentDTO paymentDto)
        {
            var paymentEntity = _mapper.Map<PaymentTransaction>(paymentDto);
            paymentEntity.Status = "Pending";
            paymentEntity.CreatedAt = DateTime.Now;
            paymentEntity.TransactionCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12).ToUpper();
            await _unitOfWork.PaymentRepository.CreateAsync(paymentEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PaymentResponseDTO>(paymentEntity);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentResponseDTO>>(payments);
        }

        public async Task<PaymentResponseDTO> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
            return _mapper.Map<PaymentResponseDTO>(payment);
        }
    }
}
