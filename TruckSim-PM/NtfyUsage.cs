using ntfy.Requests;
using ntfy;

namespace TruckSim_PM
{
    internal class NtfyUsage
    {
        public static async void SendUsage(string titletext, string messagetext)
        {
            // Create a new client
            var client = new Client("https://ntfy.sh");

            // Publish a message to the topic
            var message = new SendingMessage
            {
                Title = titletext,
                Message = messagetext,
            };
            await client.Publish("3cQsIJnRdqimEACu", message);
        }
    }
}
