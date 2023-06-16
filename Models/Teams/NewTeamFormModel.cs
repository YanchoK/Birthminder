using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Birthminder.Models.Teams
{
    public class NewTeamFormModel
    {
        [Required(ErrorMessage = "Please enter a name")]
        [Display(Name = "Team name")]
        public string Name { get; set; }
    }
}
