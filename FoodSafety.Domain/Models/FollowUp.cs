using FoodSafety.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodSafety.Domain.Models
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        public int InspectionId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public FollowUpStatus Status { get; set; } = FollowUpStatus.Open;

        public DateTime? ClosedDate { get; set; }

        public Inspection? Inspection { get; set; }
    }
}
