using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRMS.DbContexts;
using HRMS.DTO.Auth;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly HRMSContext _dbContext;


        public AuthController(HRMSContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Login([FromBody]LoginDto LoginDto)
        {
            var user = _dbContext.Users.FirstOrDefault(x=>x.Username.ToLower() == LoginDto.Username.ToLower());
            if (user == null)
            {
                return Unauthorized(new Exception("Invalid Username or Password")); //401

            }
            if(!BCrypt.Net.BCrypt.Verify(LoginDto.Password,user.HashedPassword))
            {
                return Unauthorized(new Exception("Invalid Username or Password")); //401

            }
            //generate token
            string token = GenerateToken(user);
            return Ok(token);
        }
        private string GenerateToken(User user)
        {
            // Claims ===> User Info
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())); //userid
            claims.Add(new Claim(ClaimTypes.Name, user.Username));

            //Role==> Admin,Hr,developer,manager....
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                // Employee => Position
                var employee = _dbContext.Employees.Include(x => x.Lookup).FirstOrDefault(x => x.UserId == user.Id);
                claims.Add(new Claim(ClaimTypes.Role, employee.Lookup.Name));
            }


            //Secert Key + Signing
            //WHAFWEI#!@S!!112312WQEQW@RWQEQW432 ==> Secert Key هاد هو ال 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WHAFWEI#!@S!!112312WQEQW@RWQEQW432")); //هون اعرف ال Secert Key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            //Token Settings
            var tokenSettings = new JwtSecurityToken(
                 claims: claims,
                 signingCredentials: creds,
                 expires: DateTime.Now.AddDays(1));

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenSettings);

            return token;


        }
    }
}
