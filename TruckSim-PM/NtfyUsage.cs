﻿using ntfy.Requests;
using ntfy;
using System.Reflection;
using System.IO;

namespace TruckSim_PM
{
    internal class NtfyUsage
    {
        public static async void SendUsage(string titletext, string messagetext)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

            // Create a new client
            var client = new Client("https://ntfy.sh");

            // Publish a message to the topic
            var message = new SendingMessage
            {
                Title = titletext,
                Message = string.Format("{0}\nVersion: {1} ({2})", messagetext, version.ToString(), buildDate.ToString())
            };
            await client.Publish("3cQsIJnRdqimEACu", message);
        }
    }
}
