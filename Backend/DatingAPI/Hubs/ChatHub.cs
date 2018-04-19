using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DatingAPI.Models;
using Microsoft.AspNet.SignalR;

namespace DatingAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Database _database;

        public ChatHub()
        {
            _database = new Database();
        }

        public void Connect(string userid, string chatid)
        {
            var id = Context.ConnectionId;
            Groups.Add(id, chatid);
        }

        public async Task Send(string message, string chatid, string from)
        {
            await _database.SendMessage(chatid, from, message);
            Clients.Group(chatid).sendMessage(from, message);
        }
    }
}