using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Birthminder.Models
{

    public class UserRolesViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public int TeamId { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}