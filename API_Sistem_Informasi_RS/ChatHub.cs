using System;
using System.Web;
using Microsoft.AspNet.SignalR;
namespace API_Sistem_Informasi_RS
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
    }
}