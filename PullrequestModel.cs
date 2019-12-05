using System;

class PullRequestModel
{
    public DateTime CreationDate { get; set; }
    
    public DateTime ClosedDate { get; set; }

    // improve Duration calculation (suppress off moments than weekend, lunch time, etc.)
    public TimeSpan Duration { get => ClosedDate - CreationDate; }
}
