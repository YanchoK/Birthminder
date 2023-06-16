using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Birthminder.Data
{
    public partial class Team
    {
        public Team()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }

        [InverseProperty(nameof(User.Team))]
        public virtual ICollection<User> Users { get; set; }
    }
}
