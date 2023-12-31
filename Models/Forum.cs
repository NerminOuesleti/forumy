public class Forum
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>(); // Initialize the list to avoid null reference

    // Additional property for adding a new comment
    public Comment NewComment { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Content { get; set; }
}

public class CommentViewModel
{
    public int ForumId { get; set; }
    public string ForumTitle { get; set; }
    public Comment Comment { get; set; }
}
