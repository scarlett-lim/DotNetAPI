using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;

    // Contructor name must follow file name
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("Select GETDATE()");
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        // The 1st sql query will causing the error if exceed certain character count, 
        // take dapper as example, it has the maximum 4k count.
        // for longer query, better to write in 2nd query

        # region 1st query
        // string sql = @"SELECT  [UserId],
        //                         [FirstName],
        //                         [LastName],
        //                         [Email],
        //                         [Gender],
        //                         [Active] 
        //                 FROM TUTORIALAPPSCHEMA.USERS";
        #endregion 

        #region 2nd query
        string sql = @"SELECT [UserId]," +
                                "[FirstName], " +
                                "[LastName], " +
                                "[Email], " +
                                "[Gender], " +
                                "[Active] " +
                        "FROM TUTORIALAPPSCHEMA.USERS";
        #endregion

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

}