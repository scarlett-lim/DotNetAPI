namespace DotnetAPI.Dtos;

public partial class PostToAddDto
{
    public PostToAddDto()
    {
        PostTitle ??= "";
        PostContent ??= "";
    }

    public string PostTitle { get; set; }
    public string PostContent { get; set; }
}