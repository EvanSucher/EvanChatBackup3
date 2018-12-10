// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;
using System;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    //The Chat class name becomes 
    public class Chat : Hub
    {
        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }
        
        public void AddNodeToAll(string name, int parentID)
        {
            Clients.All.SendAsync("addNodeToAll", name, parentID);
        }

        public void RemoveNodeToAll(int inputID)
        {
            Clients.All.SendAsync("removeNodeToAll", inputID);
        }

        public void ChangeNodeToAll(string newName, int inputID)
        {
            Clients.All.SendAsync("changeNodeToAll", newName, inputID);
        }

        public void AddNodeToClient(string clientID, string name, int parentID)
        {
            Clients.Client(clientID).SendAsync("addNodeToAll", name, parentID);
        }
        
        public void GetTreeDates()
        {
            Clients.All.SendAsync("getTreeDates", Context.ConnectionId);
        }

        public void sendTreeDateToClient(string clientID, DateTime dateTime)
        {
            Clients.Client(clientID).SendAsync("sendTreeDateToClient", Context.ConnectionId, dateTime);
        }

        public void GetTreeFromClient(string clientID)
        {
            Clients.Client(clientID).SendAsync("getTreeFromClient", Context.ConnectionId);
        }


    }
}
