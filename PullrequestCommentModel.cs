using System;

class PullRequestCommentModel
{
    public int PullRequestId { get; set; }

    public string CreatedBy { get; set; }

    public string Content { get; set; }

    public DateTime PublishDate { get; set; }
}
