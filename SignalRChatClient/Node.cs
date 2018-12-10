using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    class Node : INode
    {
        public int id { get; set; }
        public string nameString { get; set; }

        public int depth { get; set; }
        public List<Node> children { get; set; }
        public Node parent { get; set; }

        public Node(int inputID, string inputString, int inputDepth)
        {
            id = inputID;
            nameString = inputString;
            depth = inputDepth;
        }

        public string DisplayNodeInfo()
        {
            string OutputString = "ID:  " + id + "  Name: " + nameString + "  Depth:  " + depth;
            if (parent != null)
            {
                OutputString = OutputString + "  " + parent.DisplayNodeName();
            }
            return OutputString;
        }

        public string DisplayNodeName()
        {
            string OutputString = nameString;
            return OutputString;
        }

        public void SetParentNode(Node inputNode)
        {
            parent = inputNode;
        }

        public void AddChildNode(Node inputChildNode)
        {
            children.Add(inputChildNode);
        }

        public bool HasParent()
        {
            if (parent != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Node GetParentNode()
        {
            if ( parent != null)
            {
                return parent;
            }
            else
            {
                return null;
            }
        }

        public List<Node> GetChildrenNodes()
        {
            return children;
        }

    }
}
