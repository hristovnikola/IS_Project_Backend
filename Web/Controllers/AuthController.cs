using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Domain.Dto;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Repository.Interface;
using Service.Interface;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, IUserService userService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var username = _userService.GetMyName();
            return Ok(username);
        }

        [HttpPost("register/")]
        public async Task<ActionResult<User>> Register(RegisterUserDtoFromForm request)
        {
            // try
            // {
            var user = _userRepository.GetByUsername(request.Username);

            if (user != null)
            {
                return BadRequest("User with this username already exists");
            }
            
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RefreshToken = GenerateRefreshToken().Token,
                ShoppingCart = new ShoppingCart(),
                Role = "User"
            };

            _userRepository.Add(newUser);
            _userRepository.SaveChanges();

            return Ok(newUser);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(StatusCodes.Status500InternalServerError, "Error registering user");
            // }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDto request)
        {
            try
            {
                var user = _userRepository.GetByUsername(request.Username);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Wrong password.");
                }

                string token = CreateToken(user);

                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(user, refreshToken);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error logging in");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                var user = _userRepository.GetByRefreshToken(refreshToken);

                if (user == null)
                {
                    return Unauthorized("Invalid Refresh Token");
                }
                else if (user.TokenExpires < DateTime.Now)
                {
                    return Unauthorized("Token expired");
                }

                string token = CreateToken(user);
                var newRefreshToken = GenerateRefreshToken();
                SetRefreshToken(user, newRefreshToken);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error refreshing token");
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(User user, RefreshToken newRefreshToken)
        {
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOption);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;

            _userRepository.Update(user);
            _userRepository.SaveChanges();
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}