using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IMapper _mapper;
    IUserRepository _userRepository;

    // Contructor name must follow file name
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _userRepository.GetUsers();
    }

    // Function name no need follow API name
    [HttpGet("GetSingleUser/{userID}")]
    public User GetSingleUserFromDatabase(int userID)
    {
        return _userRepository.GetSingleUserFromDatabase(userID);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDB = _userRepository.GetSingleUserFromDatabase(user.UserId);

        if (userDB != null)
        {
            userDB.Active = user.Active;
            userDB.Email = user.Email;
            userDB.FirstName = user.FirstName;
            userDB.LastName = user.LastName;
            userDB.Gender = user.Gender;

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        #region Without Automapper
        // User userDB = new User();

        // userDB.Active = user.Active;
        // userDB.Email = user.Email;
        // userDB.FirstName = user.FirstName;
        // userDB.LastName = user.LastName;
        // userDB.Gender = user.Gender;
        #endregion

        #region With Automapper
        User userDB = _mapper.Map<User>(user);
        #endregion

        _userRepository.AddEntity<User>(userDB);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Insert User");

    }


    [HttpDelete("DeleteUser/{USERID}")]
    public IActionResult DeleteUser(int UserId)
    {
        User? userDB = _userRepository.GetSingleUserFromDatabase(UserId);

        if (userDB != null)
        {
            // _entityFramework.Users.Remove(userDB);

            _userRepository.RemoveEntity<User>(userDB);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        }
        throw new Exception("Failed to Delete User");
    }
}
