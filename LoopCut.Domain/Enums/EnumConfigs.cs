using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Enums
{
    public enum RoleEnum
    {
        [Display(Name = "Admin")]
        Admin,
        [Display(Name = "Customer")]
        Customer,
        [Display(Name = "Seller")]
        Seller 
    }
    public enum StatusEnum
    {
        Inactive = 0,
        Active = 1,
    }

}
