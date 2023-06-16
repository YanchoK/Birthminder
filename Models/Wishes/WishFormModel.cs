using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Birthminder.Models.Wishes
{
    public class WishFormModel
    {

        [Required(ErrorMessage = "Please enter a title")]
        [Display(Name = "Title")]
        [StringLength(15, ErrorMessage = "The Title must be max 15 characters long.")]
        public string Title { get; set; }


        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Wish Image")]
        public IFormFile WishImage { get; set; }

        public string ImagePath { get; set; }

    }
}