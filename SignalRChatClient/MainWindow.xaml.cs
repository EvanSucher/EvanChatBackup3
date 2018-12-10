#region snippet_MainWindowClass
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatClient
{
    public partial class MainWindow : Window
    {
        HubConnection connection;
        Tree nameTree = new Tree();
        bool isUpdated = false;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                //WHEN DONE TESTING LOCALLY...
                //MAKE SURE NEXT LINE ENDS WITH THE URL AND HUB NAME THAT MATCHES
                //OF YOUR PUBLISHED CHATROOM PROJECT.

                .WithUrl("https://evansucherchatroom.azurewebsites.net/chat")
                //.WithUrl("http://localhost:5000/chat")
                .Build();

            #region snippet_ClosedRestart
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };
            #endregion
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            #region snippet_ConnectionOn
            connection.On<string, string>("broadcastMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                   var newMessage = $"{user}: {message}";
                   messagesList.Items.Add(newMessage);
                });
            });

            connection.On<string, int>("addNodeToAll", (name, parentID) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    nameTree.AddNode(name, parentID);
                    messagesList.Items.Add("Node Added: "+nameTree.GetAllNodes()[nameTree.GetAllNodes().Count() - 1].DisplayNodeInfo());
                });
            });

            connection.On<int>("removeNodeToAll", (deleteID) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    nameTree.DeleteNodeAndBranch(deleteID);
                    nameTree.ResetIDs();
                    messagesList.Items.Add("Node and Branch Deleted");
                });
            });

            connection.On<string, int>("changeNodeToAll", (newName, nodeID) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    nameTree.GetNodeByID(nodeID).nameString = newName;
                    messagesList.Items.Add("Node Changed: " + nameTree.GetNodeByID(nodeID).DisplayNodeInfo());
                });
            });

            connection.On<string>("getTreeDates", (clientID) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    connection.InvokeAsync("sendTreeDateToClient", clientID, nameTree.GetLastUpdate());
                    //messagesList.Items.Add("ClientID: " + clientID + " has is lookin for the latest tree");
                });
            });

            connection.On<string, DateTime>("sendTreeDateToClient", (clientID, clientTime) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    
                    if (DateTime.Compare(clientTime, nameTree.GetLastUpdate()) > 0) // If the client time is more recent
                    {
                        nameTree = new Tree();
                        connection.InvokeAsync("getTreeFromClient", clientID);
                    }
                    //messagesList.Items.Add("ClientID: " + clientID + " ClientTime: "+clientTime);
                });
            });

            connection.On<string>("getTreeFromClient", (clientID) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    messagesList.Items.Add("ClientID: " + clientID + " is tryin to get my tree!");

                    List<Node> outputNodes = nameTree.GetAllNodes();

                    foreach (Node n in outputNodes)
                    {
                        if (n.HasParent())
                        {
                            int parentID = n.GetParentNode().id;
                            connection.InvokeAsync("addNodeToClient", clientID, n.DisplayNodeName(), parentID);
                        }
                        else
                        {
                            connection.InvokeAsync("addNodeToClient", clientID, n.DisplayNodeName(), 0);
                        }
                        
                    }

                });
            });

            #endregion

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                connectButton.IsEnabled = false;
                nodeContentBox.IsEnabled = true;
                ParentIDBox.IsEnabled = true;
                deleteChangeIDBox.IsEnabled = true;
                displayButton.IsEnabled = true;
                getTreeButton.IsEnabled = true;
                UpdateTree();
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void addNodeButton_Click(object sender, RoutedEventArgs e)
        {

            #region snippet_ErrorHandling
            try
            {
                if (isUpdated)
                {
                    string inputNameString = nodeContentBox.Text;
                    int inputID = 0;
                    int.TryParse(ParentIDBox.Text, out inputID);

                    //messagesList.Items.Add("..."+inputID);
                    #region snippet_InvokeAsync
                    await connection.InvokeAsync("addNodeToAll", inputNameString, inputID);
                    #endregion
                    //messagesList.Items.Add("Now is when I add the node");
                    isUpdated = false;
                }
                else
                {
                    messagesList.Items.Add("You must first update the tree to add a node!");
                }
                
            }
            catch (Exception ex)
            {                
                messagesList.Items.Add(ex.Message);                
            }
            #endregion
        }

        private async void deleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isUpdated)
                {
                    int inputID = 0;
                    int.TryParse(deleteChangeIDBox.Text, out inputID);
                    bool validInput = false;

                    foreach (Node n in nameTree.GetAllNodes())
                    {
                        if (n.id == inputID)
                        {
                            validInput = true;
                            break;
                        }
                    }

                    if (validInput)
                    {
                        #region snippet_InvokeAsync
                        await connection.InvokeAsync("removeNodeToAll", inputID);
                        #endregion
                        isUpdated = false;
                    }
                    else
                    {
                        messagesList.Items.Add("Invalid ID entered to remove a node!");
                    }

                }
                else
                {
                    messagesList.Items.Add("You must first update the tree to remove a node!");
                }

            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void changeNodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isUpdated)
                {
                    string inputNameString = nodeContentBox.Text;
                    int inputID = 0;
                    int.TryParse(deleteChangeIDBox.Text, out inputID);
                    bool validInput = false;

                    foreach (Node n in nameTree.GetAllNodes())
                    {
                        if (n.id == inputID)
                        {
                            validInput = true;
                            break;
                        }
                    }

                    if (validInput)
                    {
                        #region snippet_InvokeAsync
                        await connection.InvokeAsync("changeNodeToAll", inputNameString, inputID);
                        #endregion
                        isUpdated = false;
                    }
                    else
                    {
                        messagesList.Items.Add("Invalid ID entered to change a node!");
                    }

                }
                else
                {
                    messagesList.Items.Add("You must first update the tree to change a node!");
                }

            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void findNodeButton_Click(object sender, RoutedEventArgs e)
        {
            string inputString = nodeContentBox.Text;
            List<Node> outputNodes = nameTree.GetNodesByName(inputString);
            if (outputNodes.Count() > 0) //At least one matching node in the list
            {
                
                foreach (Node n in outputNodes)
                {
                    messagesList.Items.Add(n.DisplayNodeInfo());
                }
                /*
                for (int i = 0; i < outputNodes.Count(); i++)
                {
                    messagesList.Items.Add(nameTree.GetNodesByName(nodeContentBox.Text)[i].DisplayNodeInfo());
                }*/
            }
            else //no matching nodes
            {
                messagesList.Items.Add("-NO MATCHING NODES-");
            }
        }

        private async void displayButton_Click(object sender, RoutedEventArgs e)
        {
            List<Node> outputNodes = nameTree.GetAllNodes();
            foreach (Node n in outputNodes)
            {
                messagesList.Items.Add(n.DisplayNodeInfo());
            }

        }

        private async void getTreeButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTree();
        }

        private async void UpdateTree()
        {
            #region snippet_InvokeAsync
            await connection.InvokeAsync("getTreeDates");
            #endregion
            isUpdated = true;
            messagesList.Items.Add("Your tree is Being Updated");
        }


        private void nodeContentBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (nodeContentBox.Text != "") // prevents user from adding node unless there is a name
            {
                addNodeButton.IsEnabled = true;
                findNodeButton.IsEnabled = true;
                changeNodeButton.IsEnabled = true;
            }
            else
            {
                addNodeButton.IsEnabled = false;
                findNodeButton.IsEnabled = false;
                changeNodeButton.IsEnabled = false;
            }
        }

    }
}
#endregion
