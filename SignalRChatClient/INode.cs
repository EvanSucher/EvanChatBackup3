using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    interface INode
    {
        int id { get; set; }
        string nameString { get; set; }

        int depth { get; set; }
        List<Node> children { get; set; }
        Node parent { get; set; }
    }
}
