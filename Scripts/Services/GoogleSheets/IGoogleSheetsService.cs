using Google.Apis.Sheets.v4;

namespace Services.GoogleSheets
{
    public interface IGoogleSheetsService
    {
        SheetsService Service { get; }
    }
}