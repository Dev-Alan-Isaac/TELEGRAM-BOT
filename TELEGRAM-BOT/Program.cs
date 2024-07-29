using Newtonsoft.Json;
using System.Text.Json;
using Telegram.Bot;

class Program
{
    private static ITelegramBotClient botClient;
    private static CancellationTokenSource cts;
    private static int lastUpdateId = 0;
    private static readonly string lastUpdateIdFilePath = "lastUpdateId.txt";
    private static long? chatIdToNotify = null;
    private static bool isOnlineMessageSent = false;

    static async Task Main()
    {
        string botToken = string.Empty; // Store the token in a variable

        // Retrieve the API key
        if (File.Exists("API_Key.json"))
        {
            string jsonString = File.ReadAllText("API_Key.json");
            dynamic data = JsonConvert.DeserializeObject(jsonString);

            string apiKey = data.apikey;
            Console.WriteLine($"API Key: {apiKey}");
        }

        // Initialize the bot client with your API key
        botClient = new TelegramBotClient(botToken);
    }
}