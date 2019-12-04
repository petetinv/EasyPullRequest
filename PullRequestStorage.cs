using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

class PullRequestStorage
{
    protected readonly IEnumerable<PullRequestModel> model;

    public PullRequestStorage(IEnumerable<PullRequestModel> model)
    {
        this.model = model;
    }

    protected void AddValues(IXLRow row, IEnumerable<object> values)
    {
        int i = 1;
        foreach (var value in values)
        {
            row.Cell(i++).Value = value;
        }
    }

    protected IEnumerable<object> ExtractValues(PullRequestModel pr)
    {
        yield return pr.CreationDate;
        yield return pr.ClosedDate;
        yield return pr.Duration;
    }

    protected void AddModel(IXLRow row)
    {
        foreach (var item in this.model)
        {
            var values = ExtractValues(item);
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

            var header = new string[] { "CreationDate", "ClosedDate", "Duration" };
            AddValues(ws.Row(1), header);
            AddModel(ws.Row(2));

            ws.Columns().AdjustToContents();
            ws.RangeUsed().CreateTable();
            
            workbook.SaveAs(path);
        }
    }
}