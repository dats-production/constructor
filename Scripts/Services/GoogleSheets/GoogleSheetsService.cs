using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEngine;
using Zenject;

namespace Services.GoogleSheets
{
    public class GoogleSheetsService : IGoogleSheetsService, IInitializable, IDisposable
    {
        public SheetsService Service { get; private set; }

        private readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string applicationName;
        private readonly string credentialsPath;

        public GoogleSheetsService(string applicationName, string credentialsPath)
        {
            this.applicationName = applicationName;
            this.credentialsPath = credentialsPath;
        }

        public void Initialize()
        {
            GoogleCredential credentials;
            
            credentials = GoogleCredential.FromJson(Resources.Load<TextAsset>(credentialsPath).text).CreateScoped(scopes);

            Service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = applicationName
            });
        }

        public void Dispose()
        {
            Service?.Dispose();
        }
    }
}