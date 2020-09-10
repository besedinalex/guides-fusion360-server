using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuidesFusion360Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GuidesFusion360Server.Data.Repositories
{
    /// <inheritdoc />
    public class GuidesRepository : IGuidesRepository
    {
        private readonly DataContext _context;

        public GuidesRepository(DataContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Task<Guide> GetGuide(int guideId) =>
            _context.Guides.FirstOrDefaultAsync(x => x.Id == guideId);

        /// <inheritdoc />
        public Task<List<Guide>> GetAllGuides() =>
            _context.Guides.ToListAsync();

        /// <inheritdoc />
        public Task<List<Guide>> GetAllPublicGuides() =>
            _context.Guides.Where(x => x.Hidden == "false").ToListAsync();

        /// <inheritdoc />
        public Task<List<Guide>> GetAllHiddenGuides() =>
            _context.Guides.Where(x => x.Hidden == "true").ToListAsync();

        /// <inheritdoc />
        public Task<PartGuide> GetPartGuide(int partGuideId) =>
            _context.PartGuides.FirstOrDefaultAsync(x => x.Id == partGuideId);

        /// <inheritdoc />
        public Task<List<PartGuide>> GetPartGuides(int guideId) =>
            _context.PartGuides.Where(x => x.GuideId == guideId).ToListAsync();

        /// <inheritdoc />
        public async Task<int> CreateGuide(Guide guide)
        {
            await _context.Guides.AddAsync(guide);
            await _context.SaveChangesAsync();
            return guide.Id;
        }

        /// <inheritdoc />
        public async Task<int> CreatePartGuide(PartGuide partGuide)
        {
            await _context.PartGuides.AddAsync(partGuide);
            await _context.SaveChangesAsync();
            return partGuide.Id;
        }

        /// <inheritdoc />
        public Task UpdateGuide(Guide guide)
        {
            _context.Guides.Update(guide);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task UpdateGuides(List<Guide> guides)
        {
            _context.UpdateRange(guides);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task UpdatePartGuide(PartGuide partGuide)
        {
            _context.PartGuides.Update(partGuide);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task UpdatePartGuides(List<PartGuide> partGuides)
        {
            _context.PartGuides.UpdateRange(partGuides);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task RemoveGuide(Guide guide)
        {
            var partGuides = await _context.PartGuides.Where(x => x.GuideId == guide.Id).ToListAsync();
            _context.PartGuides.RemoveRange(partGuides);
            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task RemovePartGuide(PartGuide partGuide)
        {
            _context.PartGuides.Remove(partGuide);
            return _context.SaveChangesAsync();
        }
    }
}