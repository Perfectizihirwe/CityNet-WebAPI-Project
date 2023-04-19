using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {

        public IAuthRepository _authRepository;
        public IMapper _mapper;

        public IAuthentication _authentication;
        public AuthenticationController(IAuthentication authentication, IMapper mapper, IAuthRepository authRepository)
        {
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authentication));
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserForEntityDto>> Register(UserForInputDto user)
        {
            if (await _authRepository.UserExistsAsync(user.Username))
            {
                return Conflict(new
                {
                    message = "User already exists"
                });
            }
            _authentication.CreateHashPassword(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new UserForEntityDto()
            {
                Username = user.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            var userEntity = _mapper.Map<Entities.User>(newUser);

            _authRepository.RegisterAsync(userEntity);

            await _authRepository.SaveChangesAsync();

            return Created("User", new
            {
                message = "User created"
            });
        }

        [Route("login")]
        public async Task<ActionResult> Login(UserForInputDto user)
        {
            if (!await _authRepository.UserExistsAsync(user.Username))
            {
                return NotFound(new
                {
                    message = "User doesn't exist"
                });
            }

            var userFromDb = await _authRepository.GetUserAsync(user.Username);

            var userData = _mapper.Map<UserForEntityDto>(userFromDb);

            Console.Write(userData.PasswordHash.GetType() + "----------------------------------------------------------");

            if (!_authentication.VerifyHashPassword(user.Password, userData.PasswordHash, userFromDb.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }

            return Ok(new
            {
                message = "Login successful"
            });
        }
    }
}