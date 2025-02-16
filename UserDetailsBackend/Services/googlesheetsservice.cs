using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UserDetailsBackend.Services
{
    public class GoogleSheetsService
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "UserDetailsBackend";
        private static readonly string SpreadsheetId = "1woE-bcRHh_NAKQdr2Url6beKVjnPRlkbjbWJRjYqllE"; // Replace with your Google Sheet ID
        private static readonly string SheetName = "user data"; // Replace with your sheet name

        // Marking _sheetsService as nullable to prevent CS8618 warning
        private SheetsService? _sheetsService;

        // Constructor
        public GoogleSheetsService()
        {
            InitializeService();
        }

        // Initializes the Google Sheets service
        private void InitializeService()
        {
            try
            {
                var credential = GetCredentialsFromFile();
                _sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize Google Sheets service.", ex);
            }
        }

        // Retrieves credentials from the credentials.json file
        private GoogleCredential GetCredentialsFromFile()
        {
            string credentialsPath = GetCredentialsFilePath();

            if (!File.Exists(credentialsPath))
            {
                throw new FileNotFoundException("The credentials.json file is missing. Please ensure it is added to the project and configured correctly.", credentialsPath);
            }

            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }
        }

        // Gets the full path to the credentials.json file
        private string GetCredentialsFilePath()
        {
            // Combine the base directory with the file name
            return Path.Combine(AppContext.BaseDirectory, "credentials.json");
        }

        // Appends user details to the Google Sheets document
        public async Task AppendUserDetailsAsync(string name, string email, string phone)
        {
            if (_sheetsService == null)
            {
                throw new InvalidOperationException("Google Sheets service is not initialized.");
            }

            try
            {
                var range = $"{SheetName}!A:C"; // Define the range (columns A to C)
                var valueRange = new ValueRange
                {
                    Values = new[] { new[] { name, email, phone } }
                };

                var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                var response = await appendRequest.ExecuteAsync();
                Console.WriteLine("Data appended successfully.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to append user details to Google Sheets.", ex);
            }
        }
    }
}