using System;

class PullRequestModel
{
    public string Title { get; set; }

    public DateTime CreationDate { get; set; }
    
    public DateTime ClosedDate { get; set; }

    public int Id { get; set; }

    // improve Duration calculation (suppress off moments than weekend, lunch time, etc.)
    public TimeSpan Duration { get => ClosedDate - CreationDate; }
}
