using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TELEGRAM_BOT;

class Bot_Main
{
    private static ITelegramBotClient botClient;
    private static CancellationTokenSource cts;
    private static string botToken = string.Empty;
    private static int lastUpdateId = 0;
    private static readonly string lastUpdateIdFilePath = "lastUpdateId.txt";
    private static long? chatIdToNotify = null;
    private static bool isOnlineMessageSent = false;

    static async Task Main()
    {
        Json json = new Json();

        while (true)
        {
            if (System.IO.File.Exists("api_key.json"))
            {
                botToken = json.Json_Get();
                Console.WriteLine(botToken);
                break;
            }
            else
            {
                Console.WriteLine("ApiKey file not found.");
                json.Json_Create();
            }
        }

        Console.WriteLine("MENU");
        Console.WriteLine("===============");
        Console.WriteLine("1.Start Bot");
        Console.WriteLine("2.Chnage API Key");
        string option = Console.ReadLine();
        switch (option)
        {
            case "1":
                // Initialize the bot client with your API key
                botClient = new TelegramBotClient(botToken);

                cts = new CancellationTokenSource();

                // Load the lastUpdateId from the file
                LoadLastUpdateId();

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
                };

                // Start receiving updates
                botClient.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken: cts.Token);

                var me = await botClient.GetMeAsync();
                Console.WriteLine($"Start listening for @{me.Username}");

                try
                {
                    // Keep the program running until the cancellation token is triggered
                    await Task.Delay(Timeout.Infinite, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // Handle the cancellation gracefully
                    Console.WriteLine("Bot has been shut down.");
                }
                break;
            case "2":
                string NewApiKey = Console.ReadLine();
                json.Json_Set(NewApiKey);
                break;
            default:
                break;
        }
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Skip updates that were processed previously
        if (update.Id <= lastUpdateId)
            return;

        // Update the last processed update ID
        lastUpdateId = update.Id;
        SaveLastUpdateId();

        // Handle only messages
        if (update.Type != UpdateType.Message)
            return;

        var message = update.Message;

        // Get chat ID and send online message if not sent already
        if (chatIdToNotify == null)
        {
            chatIdToNotify = message.Chat.Id;
            if (!isOnlineMessageSent)
            {
                await SendOnlineMessage();
                isOnlineMessageSent = true;
            }
        }

        if (message.Type == MessageType.Text)
        {
            Console.WriteLine($"Received a text message from chat: {message.Text}");

            if (message.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Hello! I'm your friendly C# bot. How can I assist you today?",
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                switch (message.Text.ToLower())
                {
                    case "/img":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Sending a image...",
                            cancellationToken: cancellationToken
                        );
                        SendImage(botClient, message.Chat.Id, cancellationToken);
                        break;
                    case "/sticker":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Sending a sticker...",
                            cancellationToken: cancellationToken
                        );
                        SendSticker(botClient, message.Chat.Id, cancellationToken);
                        break;
                    case "/audio":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Sending a audio...",
                            cancellationToken: cancellationToken
                        );
                        SendAudio(botClient, message.Chat.Id, cancellationToken);
                        break;
                    case "/video":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Sending a video...",
                            cancellationToken: cancellationToken
                        );
                        SendVideo(botClient, message.Chat.Id, cancellationToken);
                        break;
                    case "/document":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Sending a document...",
                            cancellationToken: cancellationToken
                        );
                        SendDocument(botClient, message.Chat.Id, cancellationToken);
                        break;
                    case "/kill":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Shutting down...",
                            cancellationToken: cancellationToken
                        );
                        cts.Cancel(); // Signal cancellation
                        break;
                    default:
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Not a valid option. Please use a valid command.",
                            cancellationToken: cancellationToken
                        );
                        break;
                }
            }
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }

    static void SaveLastUpdateId()
    {
        try
        {
            System.IO.File.WriteAllText(lastUpdateIdFilePath, lastUpdateId.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save last update ID: {ex.Message}");
        }
    }

    static void LoadLastUpdateId()
    {
        try
        {
            if (System.IO.File.Exists(lastUpdateIdFilePath))
            {
                var lastUpdateIdText = System.IO.File.ReadAllText(lastUpdateIdFilePath);
                if (int.TryParse(lastUpdateIdText, out int savedUpdateId))
                {
                    lastUpdateId = savedUpdateId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load last update ID: {ex.Message}");
        }
    }

    static async Task SendOnlineMessage()
    {
        try
        {
            if (chatIdToNotify.HasValue)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatIdToNotify.Value,
                    text: "Bot is now online!",
                    cancellationToken: cts.Token
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send online message: {ex.Message}");
        }
    }

    static async Task SendImage(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var imageUrl = "https://telegrambots.github.io/book/docs/photo-ara.jpg";
        var tempFileName = Path.GetTempFileName();

        using (var httpClient = new HttpClient())
        {
            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            await System.IO.File.WriteAllBytesAsync(tempFileName, imageBytes);
        }

        using (var fileStream = System.IO.File.OpenRead(tempFileName))
        {
            var inputFile = new InputFileStream(fileStream);
            await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: inputFile,
                caption: "Here is your image.",
                cancellationToken: cancellationToken
            );
        }

        System.IO.File.Delete(tempFileName);
    }

    static async Task SendSticker(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var stickerUrl = "https://telegrambots.github.io/book/docs/sticker-dali.webp";
        var tempFileName = Path.GetTempFileName();

        using (var httpClient = new HttpClient())
        {
            var stickerBytes = await httpClient.GetByteArrayAsync(stickerUrl);
            await System.IO.File.WriteAllBytesAsync(tempFileName, stickerBytes);
        }

        using (var fileStream = System.IO.File.OpenRead(tempFileName))
        {
            var inputFile = new InputFileStream(fileStream);
            await botClient.SendStickerAsync(
                chatId: chatId,
                sticker: inputFile,
                cancellationToken: cancellationToken
            );
        }

        System.IO.File.Delete(tempFileName);
    }

    static async Task SendAudio(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var AudiURL = "https://telegrambots.github.io/book/docs/audio-guitar.mp3";

        var tempFileName = Path.GetTempFileName();

        using (var httpClient = new HttpClient())
        {
            var audioBytes = await httpClient.GetByteArrayAsync(AudiURL);
            await System.IO.File.WriteAllBytesAsync(tempFileName, audioBytes);
        }

        using (var fileStream = System.IO.File.OpenRead(tempFileName))
        {
            var inputFile = new InputFileStream(fileStream);
            await botClient.SendAudioAsync(
                chatId: chatId,
                audio: inputFile,
                cancellationToken: cancellationToken
            );
        }

        System.IO.File.Delete(tempFileName);
    }

    static async Task SendVideo(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var videoURL = "https://telegrambots.github.io/book/docs/video-countdown.mp4";

        var tempFileName = Path.GetTempFileName();

        using (var httpClient = new HttpClient())
        {
            var videoBytes = await httpClient.GetByteArrayAsync(videoURL);
            await System.IO.File.WriteAllBytesAsync(tempFileName, videoBytes);
        }

        using (var fileStream = System.IO.File.OpenRead(tempFileName))
        {
            var inputFile = new InputFileStream(fileStream);
            await botClient.SendVideoAsync(
                chatId: chatId,
                video: inputFile,
                cancellationToken: cancellationToken
            );
        }

        System.IO.File.Delete(tempFileName);
    }
     
    static async Task SendDocument(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var documentURL = "https://telegrambots.github.io/book/docs/photo-ara.jpg";

        var tempFileName = Path.GetTempFileName();

        using (var httpClient = new HttpClient())
        {
            var documentBytes = await httpClient.GetByteArrayAsync(documentURL);
            await System.IO.File.WriteAllBytesAsync(tempFileName, documentBytes);
        }

        using (var fileStream = System.IO.File.OpenRead(tempFileName))
        {
            var inputFile = new InputFileStream(fileStream);
            await botClient.SendVideoAsync(
                chatId: chatId,
                video: inputFile,
                cancellationToken: cancellationToken
            );
        }

        System.IO.File.Delete(tempFileName);
    }
}