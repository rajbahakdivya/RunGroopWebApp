using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Services;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> race = await _raceRepository.GetAll();

            return View(race);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }

        [HttpGet]

        public IActionResult Create()
        {
            var curUserID = _httpContextAccessor.HttpContext?.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserID };
            return View(createRaceViewModel);
        }

        [HttpPost]
        [Route("race/create")]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = raceVM.AppUserId,
                    Address = new Address
                    {
                        Street = raceVM.Address.Street,
                        City = raceVM.Address.City,
                        State = raceVM.Address.State,
                    }
                };

                await _raceRepository.Add(race);
                return RedirectToAction("Index");
            }

            // Handle invalid model state
            ModelState.AddModelError("", "Photo upload failed");
            return View(raceVM);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null) return View("Error");
            var raceVM = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory,
            };
            return View(raceVM);
        }
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race");
                return View("Edit", raceVM);
            }

            // Get the existing entity (tracked)
            var userRace = await _raceRepository.GetByIdAsync(id);

            if (userRace == null)
            {
                ModelState.AddModelError("", "Race not found");
                return View("Edit", raceVM);
            }

            try
            {
                // Delete the old photo if it exists
                if (!string.IsNullOrEmpty(userRace.Image))
                {
                    await _photoService.DeletePhotoAsync(userRace.Image);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo: " + ex.Message);
                return View("Edit", raceVM);
            }

            // Add the new photo
            var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

            // Update properties of the existing entity instead of creating a new one
            userRace.Title = raceVM.Title;
            userRace.Description = raceVM.Description;
            userRace.Image = photoResult.Url.ToString();
            userRace.AddressId = raceVM.AddressId;
            userRace.Address = raceVM.Address;

            // Update the entity in the repository
            var result = await _raceRepository.Update(userRace); // Now updating the tracked entity

            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update race");
                return View("Edit", raceVM);
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await _raceRepository.GetByIdAsync(id);
            if (clubDetails == null) return View("Error");
            return View(clubDetails);
        }

        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetails = await _raceRepository.GetByIdAsync(id);
            if (clubDetails == null) return View("Error");

            await _raceRepository.Delete(clubDetails);
            return RedirectToAction("Index");
        }


    }


}
