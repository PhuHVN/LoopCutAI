using AutoMapper;
using LoopCut.Application.DTOs.UserMembershipDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class UserMembershipService : IUserMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserMembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserMembershipResponse> AssignMembershipToUser(UserMembershipRequest request)
        {
            if (request.StartDate >= request.EndDate)
            {
                throw new ArgumentException("StartDate must be earlier than EndDate.");
            }
            if(request.EndDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("EndDate must be in the future.");
            }
            var user = await _unitOfWork.GetRepository<Accounts>()
                .FindAsync(x => x.Id == request.UserId && x.Status == StatusEnum.Active );
            if (user == null)
            {
                throw new KeyNotFoundException("User not found or inactive.");
            }
            var membership = await _unitOfWork.GetRepository<Membership>()
                .FindAsync(x => x.Id == request.MembershipId && x.Status == StatusEnum.Active);
            if (membership == null)
            {
                throw new KeyNotFoundException("Membership not found or inactive.");
            }
            var existingUserMembership = await _unitOfWork.GetRepository<UserMembership>()
                .FindAsync(x => x.UserId == request.UserId 
                && x.MembershipId == request.MembershipId 
                && x.Status == MembershipStatusEnum.Active);
            if(existingUserMembership != null)
            {
                throw new InvalidOperationException("User already has an active membership of this type.");
            }
            var userMembership = new UserMembership
            {
                UserId = request.UserId,
                MembershipId = request.MembershipId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedAt = DateTime.UtcNow,
                Status = MembershipStatusEnum.Active
            };
            await _unitOfWork.GetRepository<UserMembership>().InsertAsync(userMembership);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserMembershipResponse>(userMembership);
        }

        public async Task<BasePaginatedList<UserMembershipResponse>> GetUserMemberships(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<UserMembership>().Entity
                .Include(x => x.User).Include(x => x.Membership).OrderByDescending(x => x.StartDate);
            var rs = await _unitOfWork.GetRepository<UserMembership>().GetPagging(query, pageIndex, pageSize);
            return _mapper.Map<BasePaginatedList<UserMembershipResponse>>(rs);
        }

        public async Task<UserMembershipResponse> ExpireMembershipFromUser(string userId, string membershipId)
        {
            var userMembership = await _unitOfWork.GetRepository<UserMembership>()
                .FindAsync(x => x.UserId == userId && x.MembershipId == membershipId);
            if (userMembership == null)
            {
                throw new KeyNotFoundException("User membership not found.");
            }
            userMembership.Status = MembershipStatusEnum.Expired;
            await _unitOfWork.GetRepository<UserMembership>().UpdateAsync(userMembership);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserMembershipResponse>(userMembership);

        }

        public async Task<UserMembershipResponse> UpdateMembershipToUser(UserMembershipRequest request)
        {
            var userMembership = await _unitOfWork.GetRepository<UserMembership>()
                .FindAsync(x => x.UserId == request.UserId && x.MembershipId == request.MembershipId);
            if (userMembership == null)
            {
                throw new KeyNotFoundException("User membership not found.");
            }
            _mapper.Map(request, userMembership);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserMembershipResponse>(userMembership);
        }

        public async Task<UserMembershipResponse> ActiveMembershipFromUser(string userId, string membershipId)
        {
            var userMembership = await _unitOfWork.GetRepository<UserMembership>()
                .FindAsync(x => x.UserId == userId && x.MembershipId == membershipId);
            if (userMembership == null)
            {
                throw new KeyNotFoundException("User membership not found.");
            }
            userMembership.Status = MembershipStatusEnum.Active;
            await _unitOfWork.GetRepository<UserMembership>().UpdateAsync(userMembership);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserMembershipResponse>(userMembership);
        }

        public async Task<UserMembershipResponse> RenewMembershipToUser(RenewRequest request)
        {
            var userMembership = await _unitOfWork.GetRepository<UserMembership>()
                .FindAsync(x => x.UserId == request.UserId && x.Status == MembershipStatusEnum.Active);
            if (userMembership == null)
            {
                throw new InvalidOperationException("User has no active membership to renew.");
            }
            
            var newStartDate = userMembership.EndDate > DateTime.UtcNow ? userMembership.EndDate : DateTime.UtcNow;
            var newEndDate = newStartDate.AddMonths(request.MonthAmount);
            //make membership not active before renewing
            userMembership.Status = MembershipStatusEnum.Expired;
            userMembership.EndDate = DateTime.UtcNow;
            await _unitOfWork.GetRepository<UserMembership>().UpdateAsync(userMembership);
            //renew membership
            var renewedMembership = new UserMembership
            {
                UserId = userMembership.UserId,
                MembershipId = userMembership.MembershipId,
                StartDate = newStartDate,
                EndDate = newEndDate,
                CreatedAt = DateTime.UtcNow,
                Status = MembershipStatusEnum.Active
            };
            await _unitOfWork.GetRepository<UserMembership>().InsertAsync(renewedMembership);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserMembershipResponse>(renewedMembership);
        }

        public async Task<UserMembershipDetail> GetActiveMembershipByUserId(string userId)
        {
            var userMembership = await _unitOfWork.GetRepository<UserMembership>().Entity
                .Include(x => x.User)
                .Include(x => x.Membership)
                .Where(x => x.UserId == userId && x.Status == MembershipStatusEnum.Active)
                .OrderByDescending(x => x.EndDate)
                .FirstOrDefaultAsync();
            return _mapper.Map<UserMembershipDetail>(userMembership);
        }
    }
}
