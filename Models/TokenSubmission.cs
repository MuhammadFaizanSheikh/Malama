namespace Malama.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;

    [Index(nameof(Token), IsUnique = true)]
    public class SubmissionTokenRecord
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
