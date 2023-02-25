using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolManagement.Models;
using Serilog;
using Serilog.Core;

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        public readonly StudentContext _studentContext;
        public readonly IConfiguration _configuration;
        public StudentsController(IConfiguration configuration, StudentContext studentContext)
        {
            _configuration = configuration;
            _studentContext = studentContext;
        }



        [HttpGet("Student"),Authorize(Roles ="Admin")]
        public ActionResult GetAll()
        {
            Log.Information("................Hello................");
            return Ok(_studentContext.Students);
        }


        [HttpGet("id"), Authorize(Roles = "Admin")]
        public ActionResult Get(int id)
        {

            return Ok(_studentContext.Students.FirstOrDefault(x => x.StudentId == id));
        }



        //[HttpPost("Students")]
        //public ActionResult AddData(Student student)
        //{
        //    if (_studentContext.Students.Any(id => id.StudentId == student.StudentId))
        //    {
        //        return BadRequest("Student Already Exists");
        //    }
        //    Student s = new Student();
        //    s.StudentId = student.StudentId;
        //    s.StudentName = student.StudentName;
        //    //s.StudentAge = student.StudentAge;
        //    //s.StudentSubject = student.StudentSubject;

        //    _studentContext.Students.Add(s);
        //    _studentContext.SaveChanges();
        //    return Ok("Data Added Successfully...");
        //}
    }
}
