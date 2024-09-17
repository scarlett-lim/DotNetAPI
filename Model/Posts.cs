using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Model;
// {
public partial class Posts
{
    public Posts()
    {
        PostTitle ??= "";
        PostContent ??= "";
    }

    public int PostId { get; set; }
    public int UserId { get; set; }
    public string PostTitle { get; set; }
    public string PostContent { get; set; }
    public DateTime PostCreated { get; set; }
    public DateTime PostUpdated { get; set; }

}
// }