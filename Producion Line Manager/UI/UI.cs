namespace Producion_Line_Manager.UI
{
    public class GraphNode
    {
        public int TaskId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public int Level { get; set; } // Horizontal position
        public int Row { get; set; }   // Vertical position
    }

    public class GraphEdge
    {
        public int FromTaskId { get; set; }
        public int ToTaskId { get; set; }
        public bool IsComplete { get; set; } // If true, the line is green
    }

    public class ProductionGraphData
    {
        public List<GraphNode> Nodes { get; set; } = new();
        public List<GraphEdge> Edges { get; set; } = new();
    }
}