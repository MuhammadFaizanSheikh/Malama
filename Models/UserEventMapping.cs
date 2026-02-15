using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("UserEventMapping")]
    public class UserEventMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string UserId { get; set; }
        public long EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        [JsonIgnore]
        public EventManagement EventManagement { get; set; }
    }
}
