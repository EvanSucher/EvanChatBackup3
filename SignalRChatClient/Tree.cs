using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    class Tree
    {
        int globalID = 1; //increments by 1 anytime a new Node is added so that no id numbers are the same
        DateTime lastUpdate = new DateTime(1666, 6, 9, 4, 2, 0); //Arbitrary date, new empty tree should not be the most recent copy
        List<Node> allNodes = new List<Node>();                  //of a tree. Essentially a NULL value for the DateTime for comparing
        List<Node> baseNodes = new List<Node>();

        public DateTime GetLastUpdate()
        {
            return lastUpdate;
        }

        public List<Node> GetAllNodes()
        {
            return allNodes;
        }

        public Node GetNodeByID(int InputID)
        {
            foreach (Node n in allNodes)
            {
                if (n.id == InputID)
                {
                    return n;
                }
            }
            return null;
        }

        public List<Node> GetNodesByName(string inputString)
        {
            List<Node> tempNodeList = new List<Node>();
            foreach (Node n in allNodes)
            {
                if (n.DisplayNodeName() == inputString)
                {
                    tempNodeList.Add(n);
                }
            }
            return tempNodeList;
        }

        public void AddNode(string inputName, int inputParentID)
        {
            Node parentNode = GetNodeByID(inputParentID);
            int inputID = 1;
            foreach (Node n in allNodes)
            {
                if (n.id > inputID)
                {
                    inputID = n.id;
                }
            }

            if (parentNode != null)
            {
                int tempDepth = parentNode.depth + 8;
                Node tempNode = new Node(globalID, inputName, tempDepth);
                tempNode.SetParentNode(parentNode);
                allNodes.Add(tempNode);
                globalID++;
            }
            else
            {
                int tempDepth = 0;
                Node tempNode = new Node(globalID, inputName, tempDepth);
                allNodes.Add(tempNode);
                baseNodes.Add(tempNode);
                globalID++;
            }
            //AssignParentNodes();
            lastUpdate = DateTime.Now;

        }

        public void DeleteNodeAndBranch(int inputID)
        {
            List<Node> nodesToDelete = GetNodeAndBranch(inputID);
            
            foreach (Node n in nodesToDelete)
            {
                allNodes.Remove(n);
            }

        }

        public List<Node> GetNodeAndBranch(int inputID)
        {
            List<Node> tempList = new List<Node>();
            Node baseNode = GetNodeByID(inputID);

            foreach (Node n in allNodes)
            {
                if (n.GetParentNode() == baseNode)
                {
                    foreach (Node x in GetNodeAndBranch(n.id))
                    {
                        tempList.Add(x);
                    }
                }
            }
            tempList.Add(baseNode);
            return tempList;
        }

        public void ResetIDs()
        {
            globalID = 1;
            foreach (Node n in allNodes)
            {
                n.id = globalID;
                globalID++;
            }
        }

        // ==============================================================================================
        //                                  Tree and Node Creation Functions
        // ==============================================================================================
        public void ConvertStringArrayToTree(string[] inputArray)
        {
            // Takes in string array and defines the baseNodes, and all of the child nodes within
            FillOutNodeList(inputArray);
            AssignParentNodes();


            foreach (Node n in allNodes)
            {
                Console.WriteLine(n.DisplayNodeInfo());
            }
            lastUpdate = DateTime.Now;
        }

        public void FillOutNodeList(string[] inputArray)
        {

            foreach (string s in inputArray)
            {
                int tempDepth = CountWhitespacesBeforeString(s);
                string tempString = RemoveWhitespacesBeforeString(s);

                Node tempNode = new Node(globalID, tempString, tempDepth);
                allNodes.Add(tempNode);
                globalID++;
            }
        }

        public void AssignParentNodes()
        {
            baseNodes = new List<Node>();
            for (int i = 1; i < allNodes.Count(); i++)
            {
                if (allNodes[i].depth > 0)
                {
                    int parentIndex = FindParent(i, i-1);
                    allNodes[i].SetParentNode(allNodes[parentIndex]);
                }
                else
                {
                    baseNodes.Add(allNodes[i]);
                }
                
            }

        }

        public int FindParent(int inputIndex, int compareIndex)
        {
            if (allNodes[inputIndex].depth <= allNodes[compareIndex].depth) // If This node is not a child (has less or equal depth) to the previous
            {
                return FindParent(inputIndex, compareIndex - 1);
            }
            else // If compareIndex is a parent
            {
                return compareIndex;
            }

        }

        public string RemoveWhitespacesBeforeString(string s)
        {
            string outputString = "";
            char[] tempCharArray = s.ToCharArray();

            for (int j = 0; j < s.Count(); j++)
            {
                if (tempCharArray[j] != '\t' && tempCharArray[j] != ' ')
                {
                    outputString = outputString + tempCharArray[j];
                }
            }

           return outputString;
        }

        public int CountWhitespacesBeforeString(string s)
        {
            int numberOfSpaces = 0;
            char[] tempCharArray = s.ToCharArray();

            for (int j = 0; j < s.Count(); j++)
            {
                if (tempCharArray[j] == '\t') //counts the number of tabs
                {
                    numberOfSpaces += 8;
                }
                else if (tempCharArray[j] == ' ')//counts the number of spaces
                {
                    numberOfSpaces += 1;
                }
                else // Instantly returns the number of spaces when getting a non whitespace character, in case someone has a space in their name
                {
                    return numberOfSpaces;
                }

            }
            return numberOfSpaces;
        }


    }
}
