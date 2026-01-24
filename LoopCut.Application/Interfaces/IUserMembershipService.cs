using LoopCut.Application.DTOs.UserMembershipDtos;
using LoopCut.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IUserMembershipService
    {
        Task<UserMembershipResponse> AssignMembershipToUser(UserMembershipRequest request);
        Task<UserMembershipResponse> ExpireMembershipFromUser(string userId, string membershipId);
        Task<UserMembershipResponse> ActiveMembershipFromUser(string userId, string membershipId);
        Task<BasePaginatedList<UserMembershipResponse>> GetUserMemberships(int pageIndex,int pageSize);
        Task<UserMembershipResponse> UpdateMembershipToUser(UserMembershipRequest request);
        Task<UserMembershipResponse> RenewMembershipToUser(RenewRequest renew);
        Task<UserMembershipDetail> GetActiveMembershipByUserId(string userId);
    }
}
