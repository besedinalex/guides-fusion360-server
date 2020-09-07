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
        public Task<GuideModel> GetGuide(int guideId) =>
            _context.Guides.FirstOrDefaultAsync(x => x.Id == guideId);

        /// <inheritdoc />
        public Task<List<GuideModel>> GetAllGuides() =>
            _context.Guides.ToListAsync();

        /// <inheritdoc />
        public Task<List<GuideModel>> GetAllPublicGuides() =>
            _context.Guides.Where(x => x.Hidden == "false").ToListAsync();

        /// <inheritdoc />
        public Task<List<GuideModel>> GetAllHiddenGuides() =>
            _context.Guides.Where(x => x.Hidden == "true").ToListAsync();

        /// <inheritdoc />
        public Task<PartGuideModel> GetPartGuide(int partGuideId) =>
            _context.PartGuides.FirstOrDefaultAsync(x => x.Id == partGuideId);

        /// <inheritdoc />
        public Task<List<PartGuideModel>> GetPartGuides(int guideId) =>
            _context.PartGuides.Where(x => x.GuideId == guideId).ToListAsync();

        /// <inheritdoc />
        public async Task<int> CreateGuide(GuideModel guide)
        {
            await _context.Guides.AddAsync(guide);
            await _context.SaveChangesAsync();
            return guide.Id;
        }

        /// <inheritdoc />
        public async Task<int> CreatePartGuide(PartGuideModel partGuide)
        {
            await _context.PartGuides.AddAsync(partGuide);
            await _context.SaveChangesAsync();
            return partGuide.Id;
        }

        /// <inheritdoc />
        public Task UpdateGuide(GuideModel guide)
        {
            _context.Guides.Update(guide);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task UpdateGuides(List<GuideModel> guides)
        {
            _context.UpdateRange(guides);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task UpdatePartGuide(PartGuideModel partGuide)
        {
            _context.PartGuides.Update(partGuide);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task UpdatePartGuides(List<PartGuideModel> partGuides)
        {
            _context.PartGuides.UpdateRange(partGuides);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task RemoveGuide(GuideModel guide)
        {
            var partGuides = await _context.PartGuides.Where(x => x.GuideId == guide.Id).ToListAsync();
            _context.PartGuides.RemoveRange(partGuides);
            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task RemovePartGuide(PartGuideModel partGuide)
        {
            _context.PartGuides.Remove(partGuide);
            return _context.SaveChangesAsync();
        }
    }
}