namespace AIServer.MCPTools.IG.Models;

public class MarketNode
{
    public string Id { get; set; }
    public string Name { get; set; }
    public MarketNode[] Nodes { get; set; } // Child nodes
    public MarketDetail[] Markets { get; set; } // Markets under this node
}
