namespace Malama.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PostEventDataAnalysisViewModel
    {
        public long EventId { get; set; }
        public string SelectedStation { get; set; }

        public List<ServiceMembersChild> ServiceMembersChild { get; set; } = new();
    }
}
