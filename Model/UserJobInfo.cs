namespace DotnetAPI.Model
{
    public partial class UserJobInfo
    {
        public UserJobInfo()
        {
            JobTitle ??= "";

            Department ??= "";
        }


        public int UserId { get; set; }

        public string JobTitle { get; set; }

        public string Department { get; set; }
    }

}