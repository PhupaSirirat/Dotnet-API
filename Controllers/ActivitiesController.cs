using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoAPI.Models;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ILogger<ActivitiesController> _logger;

        public ActivitiesController(ILogger<ActivitiesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult Get()
        {
            ToDoDbContext db = new();

            string IdUser;
            try {
                IdUser = getUserId();
            }
            catch (Exception)
            {
                return StatusCode(403, new { detail = "Can't get user id from Token."});
            }

            IQueryable<Activities> activities = from act in db.Activities
                                              where act.IdUser.Equals(IdUser)
                                              select act;
            if (!activities.Any()) return NoContent();

            return Ok(activities);
        }

        [Route("{id}")]
        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult Get(uint id)
        {
            ToDoDbContext db = new();
            string IdUser;
            try
            {
                IdUser = getUserId();
            }
            catch (Exception)
            {
                return StatusCode(403, new { detail = "Can't get user id from Token." }); //forbid
            }

            IQueryable<Activities> activities = from act in db.Activities
                                              where act.IdActivity == id && act.IdUser.Equals(IdUser)
                                              select act;
            Activities? activity = activities.FirstOrDefault();
            if (activity == null) return NoContent();

            return Ok(activity);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public IActionResult Post([FromBody] DTOs.Activity data)
        {
            ToDoDbContext db = new();
            string IdUser;
            try
            {
                IdUser = getUserId();
            }
            catch (Exception)
            {
                return StatusCode(403, new { detail = "Can't get user id from Token." }); //forbid
            }

            Activities activity = new Models.Activities();
            activity.Name = data.Name;
            activity.When = data.When;
            activity.IdUser = IdUser;

            db.Activities.Add(activity);
            db.SaveChanges();

            return Ok(new { detail = "Activity added successfully." });
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize(Roles = "user")]
        public IActionResult Put(uint id, [FromBody] DTOs.Activity data)
        {
            ToDoDbContext db = new();
            string IdUser;
            try
            {
                IdUser = getUserId();
            }
            catch (Exception)
            {
                return StatusCode(403, new { detail = "Can't get user id from Token." }); //forbid
            }

            IQueryable<Activities> activities = from act in db.Activities
                                                where act.IdActivity == id && act.IdUser.Equals(IdUser)
                                                select act;
            Activities? activity = activities.FirstOrDefault();
            if (activity == null) return NotFound(new { detail = "can't find the activity" });

            activity.Name = data.Name;
            activity.When = data.When;
            db.SaveChanges();

            string detailInfo = String.Format("Activity id: {0} is updated.", id);
            return Ok(new { detail = detailInfo });

        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "user")]
        public IActionResult Delete(uint id)
        {
            ToDoDbContext db = new();
            string IdUser;
            try
            {
                IdUser = getUserId();
            }
            catch (Exception)
            {
                return StatusCode(403, new { detail = "Can't get user id from Token." }); //forbid
            }

            IQueryable<Activities> activities = from act in db.Activities
                                               where act.IdActivity == id && act.IdUser.Equals(IdUser)
                                               select act;
            Activities? activity = activities.FirstOrDefault();
            if (activity == null) return NotFound(new { detail = "can't find the activity" });

            db.Activities.Remove(activity);
            db.SaveChanges();

            string detailInfo = String.Format("Activity id: {0} is deleted.", id);
            return Ok(new { detail = detailInfo });
        }

        private string getUserId()
        {
            // Get the Authorization header from the HTTP request
            if (HttpContext.User.Identity is not ClaimsIdentity identity) throw new Exception("Claims identity not found");

            Claim? claim = identity.FindFirst(ClaimTypes.Name) ?? throw new Exception("The identity has no claim");
            return claim.Value;
        }
    }

}
