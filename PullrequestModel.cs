using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

class BranchNameComparer : IEqualityComparer<BaseModel>
{
    public bool Equals(BaseModel x, BaseModel y)
    {
        return x?.Name?.Equals(y?.Name) ?? false;
    }

    public int GetHashCode([DisallowNull] BaseModel obj)
    {
        return obj.Name.GetHashCode();
    }
}

class BaseModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }

    public bool Equals(string x, string y)
    {
        throw new NotImplementedException();
    }

    public int GetHashCode([DisallowNull] string obj)
    {
        throw new NotImplementedException();
    }
}

class PullRequestModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string RepositoryId { get; set; }

    public string RepositoryName { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ClosedDate { get; set; }

    // improve Duration calculation (suppress off moments than weekend, lunch time, etc.)
    public TimeSpan Duration { get => ClosedDate - CreationDate; }

    public string CreatedBy { get; set; }

    public IEnumerable<string> Reviewers { get; set; }

    public string ReviewerAsString { get => string.Join(", ", Reviewers ?? Enumerable.Empty<string>()); }

}
