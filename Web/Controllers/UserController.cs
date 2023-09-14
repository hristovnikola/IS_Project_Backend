using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Domain;
using Domain.Dto;
using Domain.Identity;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.Interface;
using Service.Interface;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IMapper mapper, IUserRepository userRepository,
        IConfiguration configuration)
    {
        _userService = userService;
        _mapper = mapper;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
    public IActionResult GetAllProducts()
    {
        var users = _mapper.Map<List<UserDto>>(_userService.GetAll());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(users);
    }

    [HttpPost("importUsers/")]
    [Authorize("AdminPolicy")]
    public IActionResult ImportUsers(IFormFile file)
    {
        //make a copy
        string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";


        using (FileStream fileStream = System.IO.File.Create(pathToUpload))
        {
            file.CopyTo(fileStream);

            fileStream.Flush();
        }

        //read data from uploaded file

        List<UserDto> users = GetUsersFromExcelFile(file.FileName);

        // Call the ImportAllUsers action within the same controller
        var result = ImportAllUsers(users);


        return RedirectToAction("GetAllProducts", "User");
    }

    [Authorize("AdminPolicy")]
    private List<UserDto> GetUsersFromExcelFile(string fileName)
    {
        string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        List<UserDto> userList = new List<UserDto>();

        using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                while (reader.Read())
                {
                    userList.Add(new UserDto
                    {
                        Username = reader.GetValue(0).ToString(),
                        FirstName = reader.GetValue(1).ToString(),
                        LastName = reader.GetValue(2).ToString(),
                        Email = reader.GetValue(3).ToString(),
                        Password = reader.GetValue(4).ToString()
                    });
                }
            }
        }

        return userList;
    }

    
    [HttpPost("importAllUsers/")]
    [Authorize("AdminPolicy")]
    public IActionResult ImportAllUsers(List<UserDto> model)
    {
        bool status = true;

        foreach (var item in model)
        {
            var userCheck = _userRepository.GetByUsername(item.Username);

            if (userCheck == null)
            {
                var result = Register(item);

                if (!(result is OkResult))
                {
                    status = false;
                }
            }
            else
            {
                continue;
            }
        }

        return Json(new { success = status });
    }


    [AllowAnonymous]
    private async Task<ActionResult<User>> Register(UserDto request)
    {
        // try
        // {
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

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
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
}