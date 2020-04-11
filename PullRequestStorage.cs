using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;

class PullRequestStorage
{
    protected readonly IEnumerable<PullRequestModel> prs;
    
    protected readonly IEnumerable<PullRequestCommentModel> comments;

    public PullRequestStorage(IEnumerable<PullRequestModel> prs, IEnumerable<PullRequestCommentModel> comments)
    {
        this.prs = prs;
        this.comments = comments;
    }

    protected void AddValues(IXLRow row, IEnumerable<object> values)
    {
        int i = 1;
        foreach (var value in values)
        {
            row.Cell(i++).Value = value;
        }
    }

    protected IEnumerable<object> ExtractValues(PullRequestModel pr, IEnumerable<PullRequestCommentModel> comments)
    {
        yield return pr.Id;
        yield return pr.RepositoryName;
        yield return pr.Title;
        yield return pr.Description;
        yield return pr.CreationDate;
        yield return pr.ClosedDate;
        yield return pr.Duration;
        yield return pr.CreatedBy;
        yield return pr.ReviewerAsString;
        yield return comments.Count();
    }

    protected void AddModel(IXLRow row)
    {
        foreach (var item in this.prs)
        {
            var values = ExtractValues(item, this.comments.Where(c => item.Id == c.PullRequestId));
            AddValues(row, values);
            row = row.RowBelow();
        }
    }

    public void Save(string path)
    {
        using (var workbook = new XLWorkbook())
        {
            //TODO: improve worksheet title with more information than date or something else
            IXLWorksheet ws = workbook.Worksheets.Add("Pull Request Stat");

            var header = new string[] { "Id", "Repository", "Title", "Description", "CreationDate", "ClosedDate", "Duration", "CreatedBy", "Reviewers", "Comments #" };
            AddValues(ws.Row(1), header);
            AddModel(ws.Row(2));

            ws.Columns().AdjustToContents();
            IXLTable table = ws.RangeUsed().CreateTable();
            table.Sort("CreationDate", XLSortOrder.Descending);

            workbook.SaveAs(path);
        }
    }
}