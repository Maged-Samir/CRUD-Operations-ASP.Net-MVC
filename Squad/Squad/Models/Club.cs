using System.ComponentModel.DataAnnotations;

namespace Squad.Models
{
    public class Club
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string ShortName { get; set; }

        [Required, MaxLength(100)]
        public string WebSite { get; set; }

        [Required, MaxLength(100)]
        public string Stadium { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        [Required, MaxLength(2500)]
        public string Titles { get; set; }

        [Required]
        public byte[] Logo { get; set; }

        public byte LeagueId { get; set; }

        public League League { get; set; }

    }
}
