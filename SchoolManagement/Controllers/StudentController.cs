using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public readonly StudentContext _studentContext;
        public readonly IConfiguration _configuration;
        public StudentController(IConfiguration configuration,StudentContext studentContext)
        {
            _configuration = configuration; 
            _studentContext = studentContext;
        }


        [HttpPost("Register")]
        public ActionResult Register(StudentDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            Student s= new Student();
            s.StudentId = request.StudentId;    
            s.StudentName=request.StudentName;
            s.PasswordHash = passwordHash;
            s.PasswordSalt = passwordSalt;

            _studentContext.Students.Add(s);
            _studentContext.SaveChanges();
            return Ok(s);
              
        }

        [HttpPost("Login")]
        public ActionResult Login(StudentDTO request)
        {
            Student s=_studentContext.Students.FirstOrDefault(x=>x.StudentName==request.StudentName);
            if (s.StudentName != request.StudentName)
            {
                return BadRequest("User Not Found");
            }
            if (!VerifyPasswordHash(request.Password, s.PasswordHash, s.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }
            return Ok(CreateToken(s));
        }

        private string CreateToken(Student student)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,student.StudentName),
                new Claim(ClaimTypes.Role,"Admin")
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.
                GetBytes(_configuration.GetSection("AppSettings:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt=new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
