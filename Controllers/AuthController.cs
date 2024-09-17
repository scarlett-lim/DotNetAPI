using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = string.Format("SELECT Email FROM TutorialAppSchema.Auth WHERE Email='{0}'", userForRegistration.Email);

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

                if (existingUsers.Count() == 0)
                {
                    #region generate random passwordsalt
                    byte[] passwordSalt = new byte[128 / 8];

                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }
                    #endregion

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES (@Email, @PasswordHash, @PasswordSalt)";

                    //DECLARE THE PARAMETERS TO BE PASSED INTO DB
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter emailParameter = new SqlParameter("@Email", SqlDbType.VarChar);
                    emailParameter.Value = userForRegistration.Email;

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(emailParameter);
                    sqlParameters.Add(passwordHashParameter);
                    sqlParameters.Add(passwordSaltParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (" +
                                "'" + userForRegistration.FirstName +
                                "', '" + userForRegistration.LastName +
                                "', '" + userForRegistration.Email +
                                "', '" + userForRegistration.Gender +
                                "', 1)";

                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");

                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Password Mismatch!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT 
                [PasswordHash],
                [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" +
            userForLogin.Email + "'";

            UserForLoginConfirmationDto userForLoginConfirmationDto = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmationDto.PasswordSalt);
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForLoginConfirmationDto.PasswordHash[i])
                {
                    return StatusCode(401, "Unauthorised access!");
                }
            }

            string userIdSql = string.Format("SELECT UserId FROM TutorialAppSchema.Users WHERE Email='{0}'", userForLogin.Email);

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>
            {
                {"token",_authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = @"SELECT USERID FROM TUTORIALAPPSCHEMA.USERS WHERE USERID =" + userId;

            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>
            {
                {"token",_authHelper.CreateToken(userIdFromDB)}
            });
        }
    }
}