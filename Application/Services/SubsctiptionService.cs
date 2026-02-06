using Application.DTOs.ResponseDTOs.Subscriptions;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class SubsctiptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubsctiptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> ActiveSubscription(Guid? userId, int? packageId)
        {
            Package? package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId.Value);

            var subscriptions = await _unitOfWork.SubscriptionRepository.getMySubscription(userId.Value);
            var userSubscriptions = _mapper.Map<IEnumerable<Subscription>>(subscriptions);
            foreach (var sub in userSubscriptions)
            {
                if (sub.PackageId == packageId)
                {
                    if (sub.IsActive == true)
                        sub.EndDate = sub.EndDate?.AddDays(package.DurationDays.Value);
                    else
                    {
                        sub.StartDate = DateTime.Now;
                        sub.EndDate = DateTime.Now.AddDays(package.DurationDays.Value);
                        sub.IsActive = true;
                    }
                    _unitOfWork.SubscriptionRepository.Update(sub);
                    return await _unitOfWork.SaveChangesAsync() > 0;
                }
            }

            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId.Value);
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = packageId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(package.DurationDays.Value),
                IsActive = true,
                Package = package,
                User = user
            };
            await _unitOfWork.SubscriptionRepository.CreateAsync(subscription);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task ExspireSubscription()
        {
            await _unitOfWork.SubscriptionRepository.ExspireSubscription();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubscriptionResponseDTO>> MySubscription(Guid? userId)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.getMySubscription(userId.Value);
            return _mapper.Map<IEnumerable<SubscriptionResponseDTO>>(subscription);
        }


    }
}
