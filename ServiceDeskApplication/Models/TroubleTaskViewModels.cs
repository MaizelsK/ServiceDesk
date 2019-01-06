using System;
using System.ComponentModel.DataAnnotations;

// ViewModel классы для работы с клиентом
namespace ServiceDeskApplication.Models
{
    public class TroubleTaskCreateViewModel
    {
        [Required(ErrorMessage = "Describe your problem.")]
        [Display(Name = "Task text")]
        public string Text { get; set; }
    }

    public class TroubleTaskEditViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Task creator")]
        public string CreatorFullName { get; set; }

        [Display(Name = "Date")]
        public DateTime GeneratedDate { get; set; }

        [Display(Name = "Task text")]
        public string Text { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Assigned")]
        public string AssignedFullName { get; set; }
    }

    public class TroubleTaskIndexViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Task creator")]
        public string CreatorFullName { get; set; }

        [Display(Name = "Date")]
        public DateTime GeneratedDate { get; set; }

        [Display(Name = "Task text")]
        public string Text { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Assigned")]
        public string AssignedFullName { get; set; }
    }

    public class TroubleTaskAssignViewModel
    {
        [Display(Name = "Task creator")]
        public string CreatorFullName { get; set; }

        [Display(Name = "Date")]
        public DateTime GeneratedDate { get; set; }

        [Display(Name = "Task text")]
        public string Text { get; set; }
    }
}