using Gamificacion_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gamificacion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly BdgamificacionContext _context;
        //public List<RankRule> rankRules;

        public DashboardController(BdgamificacionContext context)
        {
            _context = context;
        }

        [HttpGet("GetBadgeCounts")]
        public IActionResult GetBadgeCounts()
        {
            var badgeCounts = _context.BadgeStudents
                .GroupBy(bs => bs.IdBadgeNavigation.BadgeName)
                .Select(group => new
                {
                    BadgeName = group.Key,
                    Count = group.Count()
                })
                .ToList();

            return Ok(badgeCounts);
        }

        [HttpGet("GetAchievementTypeDistribution")]
        public IActionResult GetAchievementTypeDistribution()
        {
            var achievementTypeCounts = _context.Badges
                .Include(b => b.IdTypeAchivementNavigation) 
                .GroupBy(b => b.IdTypeAchivementNavigation.NameTypeAchievement) 
                .Select(group => new
                {
                    TypeName = group.Key, 
                    Count = group.Count()
                })
                .ToList();

            return Ok(achievementTypeCounts);
        }




        [HttpGet("GetAchievementDistribution")]
        public IActionResult GetAchievementDistribution()
        {
            var distribution = _context.Achievements
                .GroupBy(a => a.Punctuation)
                .Select(group => new
                {
                    Punctuation = group.Key,
                    Count = group.Count()
                })
                .OrderBy(d => d.Punctuation)
                .ToList();

            return Ok(distribution);
        }

        [HttpGet("GetRankHierarchy")]
        public IActionResult GetRankHierarchy()
        {
            var ranks = _context.Ranks
                .Select(r => new
                {
                    r.NameRank,
                    r.NameSubrank,
                    StudentCount = r.Students.Count
                })
                .ToList();

            return Ok(ranks);
        }

        [HttpGet("GetStudentRankDistribution")]
        public IActionResult GetStudentRankDistribution()
        {
            var distribution = _context.Students
                .GroupBy(s => s.IdRankNavigation.NameRank)
                .Select(group => new
                {
                    Rank = group.Key,
                    Count = group.Count()
                })
                .ToList();

            return Ok(distribution);
        }

        [HttpGet("GetSanctionsPerStudent")]
        public IActionResult GetSanctionsPerStudent()
        {
            var sanctionsPerStudent = _context.Sanctions
                .Include(s => s.IdStudentNavigation) 
                .GroupBy(s => new { s.IdStudentNavigation.FirstName, s.IdStudentNavigation.LastName })
                .Select(group => new
                {
                    FullName = group.Key.FirstName + " " + group.Key.LastName, 
                    SanctionCount = group.Count() 
                })
                .ToList();

            return Ok(sanctionsPerStudent);
        }


        [HttpGet("GetSanctionsByAdministrator")]
        public IActionResult GetSanctionsByAdministrator()
        {
            var sanctionsByAdmin = _context.Sanctions
                .GroupBy(s => s.IdAdministrator)
                .Select(group => new
                {
                    AdministratorId = group.Key,
                    SanctionCount = group.Count()
                })
                .ToList();

            return Ok(sanctionsByAdmin);
        }


    }
}
