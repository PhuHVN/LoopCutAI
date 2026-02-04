using AutoMapper;
using LoopCut.Application.DTOs.MembershipDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using LoopCut.Domain.Enums.EnumConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    internal class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;    
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<MembershipResponse> CreateMembership(MembershipRequest membership)
        {
            var existMembership = await _unitOfWork.GetRepository<Membership>()
                .FindAsync(m => m.Name.ToLower() == membership.Name.ToLower() || m.Code.ToLower() == membership.Code.ToLower());
            if (existMembership != null)
            {
                throw new Exception("Membership with the same name or code already exists.");
            }
            var newMembership = new Membership
            {               
                Name = membership.Name,
                Code = membership.Code,
                Price = membership.Price,
                Description = membership.Description,
                Status = StatusEnum.Active
            };
            await _unitOfWork.GetRepository<Membership>().InsertAsync(newMembership);
            await _unitOfWork.SaveChangesAsync();
            return mapper.Map<MembershipResponse>(newMembership);
        }

        public async Task DeleteMembership(string id)
        {
            var membership = await _unitOfWork.GetRepository<Membership>()
                .FindAsync(m => m.Id == id && m.Status == StatusEnum.Active);
            if (membership == null)
            {
                throw new Exception("Membership not found ");
            }
            membership.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Membership>().UpdateAsync(membership);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BasePaginatedList<MembershipResponse>> GetAllMemberships(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Membership>().Entity.Where(m => m.Status == StatusEnum.Active);
            var rs = await _unitOfWork.GetRepository<Membership>()
                .GetPagging(query, pageIndex, pageSize);
            return mapper.Map<BasePaginatedList<MembershipResponse>>(rs);
        }

        public async Task<MembershipResponse> GetMembershipById(string id)
        {
            var membership = await _unitOfWork.GetRepository<Membership>()
                .FindAsync(m => m.Id == id && m.Status == StatusEnum.Active);
            if (membership == null)
            {
                throw new Exception("Membership not found");
            }
            return mapper.Map<MembershipResponse>(membership);
        }
        public async Task<MembershipResponse> GetMembershipByName(string name)
        {
            var membership = await _unitOfWork.GetRepository<Membership>()
                .FindAsync(m => m.Name.ToLower() == name.ToLower() && m.Status == StatusEnum.Active);
            if (membership == null)
            {
                throw new Exception("Membership not found");
            }
            return mapper.Map<MembershipResponse>(membership);
        }
        public async Task<MembershipResponse> UpdateMembership(string id, MembershipUpRes membership)
        {
            var existingMembership = await _unitOfWork.GetRepository<Membership>()
                .FindAsync(m => m.Id == id && m.Status == StatusEnum.Active);
            if (existingMembership == null)
            {
                throw new Exception("Membership not found");
            };
            var isUpdate = false;
            if(!string.IsNullOrEmpty(membership.Name)  && existingMembership.Name != membership.Name)
            {
                existingMembership.Name = membership.Name;
                isUpdate = true;
            }
            if(!string.IsNullOrEmpty(membership.Code) && existingMembership.Code != membership.Code)
            {
                existingMembership.Code = membership.Code;
                isUpdate = true;
            }
            if(membership.Price > 0 && existingMembership.Price != membership.Price)
            {
                existingMembership.Price = membership.Price; 
                isUpdate = true;
            }
            if(!string.IsNullOrEmpty(membership.Description) && existingMembership.Description != membership.Description)
            {
                existingMembership.Description = membership.Description;
                isUpdate = true;
            }
            if (!isUpdate)
            {
                throw new Exception("No changes detected to update.");
            }
            await _unitOfWork.GetRepository<Membership>().UpdateAsync(existingMembership);
            await _unitOfWork.SaveChangesAsync();
            return mapper.Map<MembershipResponse>(existingMembership);
        }
    }
}
