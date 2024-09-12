using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetAPI.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        public UserForLoginConfirmationDto()
        {
            PasswordHash ??= new byte[0];

            PasswordSalt ??= new byte[0];
        }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

    }
}