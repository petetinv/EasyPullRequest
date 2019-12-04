using System;

class PullRequestModel
{
    public DateTime CreationDate { get; set; }
    
    public DateTime ClosedDate { get; set; }

    public TimeSpan Duration { get => ClosedDate - CreationDate; }
}
