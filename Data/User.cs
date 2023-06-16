using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Birthminder.Data
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {
            Wishes = new HashSet<Wish>();
        }
        public string FullName { get; set; }
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public int TeamId { get; set; }

        [ForeignKey(nameof(TeamId))]
        [InverseProperty("Users")]
        public virtual Team Team { get; set; }
        [InverseProperty(nameof(Wish.User))]
        public virtual ICollection<Wish> Wishes { get; set; }
    }
}
