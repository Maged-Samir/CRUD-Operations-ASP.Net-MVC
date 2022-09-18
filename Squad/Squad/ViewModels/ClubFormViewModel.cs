using Squad.Models;
using System.ComponentModel.DataAnnotations;

namespace Squad.ViewModels
{
    public class ClubFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(250)]
        public string? Name { get; set; }

        [Required, StringLength(100)]
        public string? ShortName { get; set; }

        [Required, StringLength(100)]
        public string? WebSite { get; set; }

        [Required, StringLength(100)]
        public string? Stadium { get; set; }

        public int Year { get; set; }
        [Range(1, 10)]

        public double Rate { get; set; }

        [Required, StringLength(2500)]
        public string? Titles { get; set; }

       
        public byte[]? Logo { get; set; }

        [Display(Name="League")]
        public byte LeagueId { get; set; }

        //this properity for our Dropdown list 
        public IEnumerable<League>? Leagues { get; set; }
    }
}
