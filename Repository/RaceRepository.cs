using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _context;

        public RaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Race race)
        {
            await _context.AddAsync(race);  // Use AddAsync for async behavior
            return await Save();
        }

        public async Task<bool> Delete(Race race)
        {
            _context.Remove(race);
            return await Save();  // Async save
        }

        public async Task<IEnumerable<Race>> GetAll()
        {
            return await _context.Races.ToListAsync();
        }

        public async Task<IEnumerable<Race>> GetAllRacesByCity(string city)
        {
         return await _context.Races.Where(c => c.Address.City.Contains(city)).ToListAsync();   
        }

        public async Task<Race> GetByIdAsync(int id)
        {
            return await _context.Races.Include(i => i.Address)
                                       .FirstOrDefaultAsync(r => r.Id == id);  // Use id to filter
        }


        public async Task<Race> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Races.Include(i => i.Address).AsNoTracking().FirstOrDefaultAsync();

        }



        public async Task<bool> Update(Race race)
        {
            _context.Update(race);
            return await Save();  // Async save
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();  // Use SaveChangesAsync for async behavior
            return saved > 0;  // Return true if changes are saved successfully
        }

    }
}
