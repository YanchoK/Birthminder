using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Birthminder.Models
{

    public class ManageUserRolesViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}