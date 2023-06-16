using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Birthminder.Data
{
    public partial class Wish
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [StringLength(100)]
        public string Link { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int PriorityIndex { get; set; }
        public bool? Gifted { get; set; }
        public bool? IsDeleted { get; set; }
        public int UserId { get; set; }
        [StringLength(200)]
        public string ImageLink { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Wishes")]
        public virtual User User { get; set; }
    }
}
