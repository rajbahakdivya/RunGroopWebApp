using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
            return View(createClubViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubVM.Image);
                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = clubVM.AppUserId,
                    Address = new Address
                    {
                        Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State,
                    }
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(clubVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null) return View("Error");

            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId ?? 0, // Safely handle null values
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory,
            };

            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            // Check if the model state is valid
             if (!ModelState.IsValid)
             {
                 ModelState.AddModelError("", "Failed to edit club");
                 return View("Edit", clubVM);
             }  


            var userClub = await _clubRepository.GetByIdAsync(id); // Get the existing entity

            if (userClub == null)
            {
                ModelState.AddModelError("", "Club not found");
                return View("Edit", clubVM);
            }

            try
            {
                // Delete the old photo if it exists
                if (!string.IsNullOrEmpty(userClub.Image))
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo: " + ex.Message);
                return View(clubVM);
            }

            // Add the new photo
            var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

            // Update properties of the existing entity instead of creating a new one
            userClub.Title = clubVM.Title;
            userClub.Description = clubVM.Description;
            userClub.Image = photoResult.Url.ToString();
            userClub.AddressId = clubVM.AddressId;
            userClub.Address = clubVM.Address;

            // Update the entity in the repository
            var result = _clubRepository.Update(userClub); // Now updating the tracked entity

            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update club");
                return View("Edit",clubVM);
            }
        }

        public async Task<IActionResult> Delete (int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if(clubDetails == null) return View("Error");
            return View(clubDetails);
        }

        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync (id);
            if (clubDetails == null) return View("Error");

            _clubRepository.Delete(clubDetails);
            return RedirectToAction("Index");
        }
       
    }
}
