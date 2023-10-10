using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Models;
using Microsoft.Extensions.Logging;

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
        public IActionResult Get()
        {
            var db = new ToDoDbContext();

            var activity = from a in db.Activities select a;
            if (!activity.Any()) return NoContent();

            return Ok(activity);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(uint id)
        {
            var db = new ToDoDbContext();

            var activity = db.Activities.Find(id);
            if (activity == null) return NoContent();

            return Ok(activity);
        }

        [HttpPost]
        public IActionResult Post([FromBody] DTOs.Activity data)
        {
            var db = new ToDoDbContext();

            var activity = new Models.Activities();
            activity.Name = data.Name;
            activity.When = data.When;
            activity.IdUser = data.IdUser;

            db.Activities.Add(activity);
            db.SaveChanges();

            return Ok(new { detail = "Activity added successfully." });
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Put(uint id, [FromBody] DTOs.Activity data)
        {
            ToDoDbContext db = new();

            IQueryable<Activities> activities = from act in db.Activities
                                                where act.IdActivity == id
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
        public IActionResult Delete(uint id)
        {
            ToDoDbContext db = new();

            IQueryable<Activities> activities = from act in db.Activities
                                               where act.IdActivity == id
                                               select act;
            Activities? activity = activities.FirstOrDefault();
            if (activity == null) return NotFound(new { detail = "can't find the activity" });

            db.Activities.Remove(activity);
            db.SaveChanges();

            string detailInfo = String.Format("Activity id: {0} is deleted.", id);
            return Ok(new { detail = detailInfo });
        }
    }

}
