using LoopCut.Application.DTOs.MembershipDtos;
using LoopCut.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IMembershipService
    {
        Task<MembershipResponse> CreateMembership(MembershipRequest membership);
        Task<MembershipResponse> UpdateMembership(string id, MembershipUpRes membership);
        Task<MembershipResponse> GetMembershipById(string id);
        Task<BasePaginatedList<MembershipResponse>> GetAllMemberships(int pageIndex, int pageSize);
        Task<MembershipResponse> GetMembershipByName(string name);
        Task DeleteMembership(string id);
    }
}
