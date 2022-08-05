using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Constructor.Details;
using Cysharp.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Services.GoogleSheets;
using Services.LocalizationService;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Constructor
{
    public class LayersDataProvider : ILayersDataProvider
    {
        private readonly IGoogleSheetsService googleSheetsService;
        private readonly string layersSpreadsheetId;
        private readonly IUIBlocker uiBlocker;
        private ILocalizationService localizationService;

        [Inject]
        public LayersDataProvider(IGoogleSheetsService googleSheetsService,
            string layersSpreadsheetId, IUIBlocker uiBlocker, ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            this.googleSheetsService = googleSheetsService;
            this.layersSpreadsheetId = layersSpreadsheetId;
            this.uiBlocker = uiBlocker;
        }

        public async UniTask<IReadOnlyList<Layer>> GetLayers()
        {
            var request = googleSheetsService.Service.Spreadsheets.Get(layersSpreadsheetId);
            var response = await request.ExecuteAsync();
            var layers = response.Sheets.Select(GetLayer).ToList();
            return layers;
        }
        
        public async UniTask ExportSheets(List<Layer> layers)
        {
            using var cts = new CancellationTokenSource();
            using var progressSubject = new Subject<float>();
            uiBlocker.Show(progressSubject, cts.Cancel);
            foreach (var layer in layers)
            {
                const int firstRowIndex = 3;
                
                var layerName = layer.Name;
                var lastRowIndex = layer.Details.Count + firstRowIndex - 1;
                var range = $"{layerName}!A{firstRowIndex}:C{lastRowIndex}";
                var rows = new ValueRange
                {
                    Values = new List<IList<object>>()
                };

                foreach (var detail in layer.Details)
                {
                    var row = new List<object>
                    {
                        detail.Name.Value,
                        detail.Rarity.Value + "%"
                    };

                    if (detail is ColorDetail colorDetail)
                    {
                        var color = ColorUtility.ToHtmlStringRGB(colorDetail.Color.Value);
                        row.Add(color);
                    }
                    
                    rows.Values.Add(row);
                }

                var request = googleSheetsService.Service.Spreadsheets.Values
                    .Update(rows, layersSpreadsheetId, range);
                request.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                try
                {
                    await request.ExecuteAsync(cancellationToken: cts.Token);
                }
                finally
                {
                    uiBlocker.Hide();
                    progressSubject.OnCompleted();
                }
            }
        }

        private Layer GetLayer(Sheet sheet)
        {
            const int firstLayerRowIndex = 1;
            
            var layerName = sheet.Properties.Title;
            var maxLayersCount = sheet.Properties.GridProperties.RowCount;
            var lastLayerRowIndex = maxLayersCount + 2;
            
            var request = googleSheetsService.Service.Spreadsheets.Values.Get(layersSpreadsheetId,
                $"{layerName}!A{firstLayerRowIndex}:C{lastLayerRowIndex}");
            var response = request.Execute();

            var layer = new Layer(layerName);
            var rows = response.Values;
            var detailTypeRow = rows[0];
            var detailType = detailTypeRow[1].ToString();
            for (var i = 0; i < rows.Count; i++)
            {
                const int rowCountToExclude = 2;
                
                if(i < rowCountToExclude) continue;
                var row = rows[i];
                var detailName = row[0].ToString();
                var rarity = float.Parse(row[1].ToString().TrimEnd('%'));

                Detail detail;
                switch (detailType)
                {
                    case nameof(FbxDetail):
                        detail = new FbxDetail(null, detailName, rarity, layerName);
                        break;
                    case nameof(TextureDetail):
                        detail = new TextureDetail(detailName, rarity, layerName);
                        break;
                    case nameof(BackgroundDetail):
                        detail = new BackgroundDetail(null, detailName, rarity, layerName);
                        break;
                    case nameof(ColorDetail):
                        var colorString = "#" + row[2];
                        if (!ColorUtility.TryParseHtmlString(colorString, out var color))
                        {
                            localizationService.SetStringVariable("colorString",colorString.TrimStart('#'));
                            localizationService.SetStringVariable("layerName",layerName);
                            localizationService.SetStringVariable("detailName",detailName);
                            Debug.LogError(localizationService.Localize(
                                "You have typed incorrect color values () in Layer (), detail (). Retype color in Hex format."));
                        }
                        detail = new ColorDetail(color, detailName, rarity, layerName);
                        break;
                    default:
                        localizationService.SetStringVariable("detailType",detailType);
                        Debug.LogError(localizationService.Localize(
                            "New type of detail () was found in Google Sheet."));
                        detail = null;
                        break;
                }

                layer.AddDetail(detail);
            }

            return layer;
        }
    }
}