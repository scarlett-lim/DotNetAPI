using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("fsgs")]
public class UserJobssController : ControllerBase
{
    DataContextDapper _dapper;

    // Contructor name must follow file name
    public UserJobssController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT [UserId]," +
                                "[FirstName], " +
                                "[LastName], " +
                                "[Email], " +
                                "[Gender], " +
                                "[Active] " +
                        "FROM TUTORIALAPPSCHEMA.USERS";

        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    // Function name no need follow API name
    [HttpGet("GetSingleUser/{userID}")]
    public User GetSingleUserFromDatabase(int userID)
    {
        string sql = @"SELECT [UserId]," +
                               "[FirstName], " +
                               "[LastName], " +
                               "[Email], " +
                               "[Gender], " +
                               "[Active] " +
                       "FROM TUTORIALAPPSCHEMA.USERS " +
                       "WHERE Userid = " + userID.ToString();
        User user = _dapper.LoadDataSingle<User>(string.Format(sql));
        return user;
    }

    // IActionResult => return http result
    // [FromBody] as the param will accept any type of payload from the request sent to this API
    // If specify param like User user, then this API can only accept the json that exist in the modal User 
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
                "', [LastName] = '" + user.LastName +
                "', [Email] = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active +
            "' WHERE UserId = " + user.UserId;

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }


    //Dto = Data Transfer Object
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (" +
                "'" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active +
            "')";

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }

}