using FoodSafety.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodSafety.Domain.Models
{
    public class Premises
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Town { get; set; } = string.Empty;

        [Required]
        public RiskRating RiskRating { get; set; }

        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}
