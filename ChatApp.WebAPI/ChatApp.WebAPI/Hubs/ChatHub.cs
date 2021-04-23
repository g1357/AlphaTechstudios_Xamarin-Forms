using ChatApp.Managers.Interfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Концентраторы Web API приложения
/// </summary>
namespace ChatApp.WebAPI.Hubs
{
    /// <summary>
    /// Концентратор бесед
    /// </summary>
    public class ChatHub : Hub
    {
        private readonly IUsersManager usersManager;
        private readonly IConversationsManager conversationsManager;

        /// <summary>
        /// Конструктор концентратора бесед
        /// </summary>
        /// <param name="usersManager"></param>
        /// <param name="conversationsManager"></param>
        public ChatHub(IUsersManager usersManager, IConversationsManager conversationsManager)
        {
            this.usersManager = usersManager;
            this.conversationsManager = conversationsManager;
        }

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string userId, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", userId, message);
        }

        /// <summary>
        /// Отправить личное сообщение
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendPrivateMessage(string userEmail, string message)
        {

            var senderUser = usersManager.GetUserByConnectionId(Context.ConnectionId);
            var friend = usersManager.GetUserByEmail(userEmail);
            var friendConnections = friend.Connections.Where(x => x.IsConnected);
            foreach (var connection in friendConnections)
            {
                await Clients.Client(connection.ConnectionID).SendAsync("ReceivePrivateMessage", userEmail, message);
            }
            // Inser in to database..
            var conversationModel = conversationsManager.GetConversationByUsersId(senderUser.ID, friend.ID);

            if (conversationModel == null)
            {
                var conversationId = conversationsManager.AddOrUpdateConversation(senderUser.ID, friend.ID);
                conversationsManager.AddReply(message, conversationId, senderUser.ID);
            }
            else
            {
                conversationsManager.AddReply(message, conversationModel.ID, senderUser.ID);
            }
        }

        /// <summary>
        /// Вызывается при установке соединения.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task OnConnect(string userEmail)
        {
            var user = usersManager.GetUserByEmail(userEmail);
            usersManager.AddUserConnections(new ConnectionModel
            {
                ConnectionID = Context.ConnectionId,
                IsConnected = true,
                UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"],
                UserID = user.ID
            });

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Вызывается при прекращении соединения.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task OnDisconnect(string userEmail)
        {
            var user = usersManager.GetUserByEmail(userEmail);
            usersManager.UpdateUserConnectionsStatus(user.ID, false, Context.ConnectionId);
            await base.OnDisconnectedAsync(null);
        }
    }
}

