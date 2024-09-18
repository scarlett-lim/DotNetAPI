namespace DotnetAPI.Dtos;

public partial class PostToEditDto
{
    public PostToEditDto()
    {
        PostTitle ??= "";
        PostContent ??= "";
    }

    public int PostId { get; set; }
    public string PostTitle { get; set; }
    public string PostContent { get; set; }
}
