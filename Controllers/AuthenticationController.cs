using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiversion}/auth")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        private readonly IAuthentication _authentication;

        private readonly IConfiguration _configuration;
        public AuthenticationController(IAuthentication authentication,
                                        IMapper mapper,
                                        IAuthRepository authRepository,
                                        IConfiguration configuration)
        {
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authentication));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserProfileDto>> Register(UserProfileDto user)
        {
            if (await _authRepository.UserExistsAsync(user.Username))
            {
                return Conflict(new
                {
                    message = "User already exists"
                });
            }


            var newUser = new UserProfileDto()
            {
                Username = user.Username,
                Password = _authentication.HashPassword(user.Password)
            };

            var userEntity = _mapper.Map<Entities.User>(newUser);

            _authRepository.RegisterAsync(userEntity);

            await _authRepository.SaveChangesAsync();

            return Created("User", new
            {
                message = "User created"
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserProfileDto user)
        {
            if (!await _authRepository.UserExistsAsync(user.Username))
            {
                return NotFound(new
                {
                    message = "User doesn't exist"
                });
            }

            var userFromDb = await _authRepository.GetUserAsync(user.Username);

            if (userFromDb == null)
            {
                return NotFound(new
                {
                    message = "User doesn't exist"
                });
            }

            var userData = _mapper.Map<UserProfileDto>(userFromDb);

            if (!_authentication.ComparePassword(user.Password, userData.Password))
            {
                return BadRequest("Wrong Password");
            }

            // Create token

            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"])
            );

            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256
            );

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", userFromDb.Id.ToString()));
            claimsForToken.Add(new Claim("username", userData.Username));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(2),
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }
    }
}