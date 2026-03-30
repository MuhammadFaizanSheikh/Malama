 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Malama.Models
{
    public class CreateContainerDto
    {
        public long EventId { get; set; }
        public string ContainerName { get; set; } = string.Empty;
        public int ContainerTypeId { get; set; }
        public DateTime StartDate { get; set; } // date portion
        //public TimeSpan StartTime { get; set; } // time portion
        public decimal InitialTemperature { get; set; }
        public string? Comment { get; set; }
        public string SubmissionToken { get; set; }
    }

    public class CreateReadingDto
    {
        public long ContainerId { get; set; }
        public decimal? Temperature { get; set; }
        public string? Comment { get; set; }
        public bool IsFinalReading { get; set; }
        public string SubmissionToken { get; set; }
    }
}
