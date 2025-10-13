 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Malama.Models
{
    [Table("ContainerType")]
    public class ContainerType : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TemperatureFromRange { get; set; }

        public int TemperatureToRange { get; set; }
        public string TemperatureUnit { get; set; } = "C";
    }

    [Table("Container")]
    public class Container : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        public string EventId { get; set; } // assume Event table exists


        public string ContainerName { get; set; } = string.Empty;


        [ForeignKey("ContainerType")]
        public long ContainerTypeId { get; set; }
        public ContainerType? ContainerType { get; set; }


        public DateTime StartDateTimeUtc { get; set; }


        // initial temperature when container added
        public decimal InitialTemperature { get; set; }


        public string CurrentStatus { get; set; } = "Normal"; // Normal | OutOfRange


        // scheduling
        public DateTime NextExpectedReadingUtc { get; set; }
        public int MonitoringIntervalMinutes { get; set; } = 120; // default 2 hours
        public int EscalationIntervalMinutes { get; set; } = 15; // default 15 minutes


        // consecutive normal readings counter (used to resume 2-hour cadence)
        public int ConsecutiveNormalReadings { get; set; } = 0;
    }

    [Table("ContainerTemperatureReading")]
    public class ContainerTemperatureReading : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey("Container")]
        public long ContainerId { get; set; }
        public Container? Container { get; set; }


        public DateTime ReadingTimeUtc { get; set; }
        public decimal Temperature { get; set; }
        public bool IsOutOfRange { get; set; }
        public string? Comment { get; set; }


        // attempt number since the last in-range reading (1,2,...)
        public int AttemptNumber { get; set; }
    }
}
