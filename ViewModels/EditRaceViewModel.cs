using RunGroopWebApp.Data.Enum;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class EditRaceViewModel
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string? URL { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public RaceCategory RaceCategory { get; set; }
    }
}
