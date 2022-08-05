using UnityEngine;

namespace Configs
{
    public class GoogleSheetsConfig : ScriptableObject
    {
        public string ApplicationName;
        public string CredentialsPath;
        public string LayersSpreadsheetId;
    }
}