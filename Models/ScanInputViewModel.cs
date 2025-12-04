using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StepUp.Models
{
    public class ScanInputViewModel
    {
        public int PoiId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Teamname")]
        public string TeamName { get; set; } = string.Empty;

        public bool Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrWhiteSpace(TeamName))
            {
                modelState.AddModelError(nameof(TeamName), "Teamname darf nicht leer sein.");
            }
            return modelState.IsValid;
        }
    }
}