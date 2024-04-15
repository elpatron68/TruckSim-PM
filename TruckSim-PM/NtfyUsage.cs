using ntfy.Actions;
using ntfy.Requests;
using ntfy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckSim_PM
{
    internal class NtfyUsage
    {
        public static async void SendUsage(string titletext, string messagetext)
        {
            // Create a new client
            var client = new Client("https://ntfy.sh");

            // Publish a message to the "test" topic
            var message = new SendingMessage
            {
                Title = titletext,
                Message = messagetext,
                //Actions = new ntfy.Actions.Action[]
                //        {
                //new Broadcast("label")
                //{
                //},
                //new View("label2", new Uri("https://google.com"))
                //{
                //}
                //        }
            };
            await client.Publish("3cQsIJnRdqimEACu", message);
        }
    }
}
