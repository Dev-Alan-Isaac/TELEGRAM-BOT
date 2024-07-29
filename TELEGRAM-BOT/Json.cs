using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGRAM_BOT
{
    internal class Json
    {
        public void Json_Create()
        {
            // Create an object that matches the structure of your JSON file
            var apiObject = new { Api_Key = "YOUR_API_KEY" };

            // Convert the object to JSON
            string json = JsonConvert.SerializeObject(apiObject, Formatting.Indented);

            // Write the JSON string to a file
            File.WriteAllText("api_key.json", json);
            Console.WriteLine("File created...");
        }

        public string Json_Get()
        {
            // Read the JSON file
            string json = File.ReadAllText("api_key.json");

            // Deserialize the JSON string to an object
            var apiObject = JsonConvert.DeserializeObject<dynamic>(json);

            // Return the API key
            return apiObject.Api_Key;
        }

        public void Json_Set(string newApiKey)
        {
            // Read the JSON file
            string json = File.ReadAllText("api_key.json");

            // Deserialize the JSON string to an object
            var apiObject = JsonConvert.DeserializeObject<dynamic>(json);

            // Update the API key
            apiObject.Api_Key = newApiKey;

            // Convert the object back to a JSON string
            json = JsonConvert.SerializeObject(apiObject, Formatting.Indented);

            // Write the updated JSON string back to the file
            File.WriteAllText("api_key.json", json);
            Console.WriteLine("API change...");
        }

    }
}
