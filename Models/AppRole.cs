using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Birthminder.Models
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole(string name) : base(name)
        {
        }
    }
}