using Models.Management;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.UI;

namespace Producion_Line_Manager.Services
{
    public class ProductionGraphService
    {
        private readonly RestService _restService;

        public ProductionGraphService(RestService restService)
        {
            _restService = restService;
        }

        public async Task<ProductionGraphData> BuildGraphForProductAsync(int productId)
        {
            var graphData = new ProductionGraphData();

            // 1. Fetch raw data for this specific product
            // Note: Adjust these RestService calls to match your exact API endpoints
            var tasks = await _restService.Get<Products,Tasks>(productId, new RequestParameters { /* Filter by ProductId */ });
            var dependencies = await _restService.Get<TaskDependencies>();
            var processes = await _restService.Get<Processes>();

            if (tasks?.Items == null) return graphData;

            // 2. Map Edges (Lines)
            foreach (var dep in dependencies?.Items ?? new List<TaskDependencies>())
            {
                var fromTask = tasks.Items.FirstOrDefault(t => t.Id == dep.DependsOnTaskId);
                if (fromTask != null && tasks.Items.Any(t => t.Id == dep.TaskId))
                {
                    graphData.Edges.Add(new GraphEdge
                    {
                        FromTaskId = dep.DependsOnTaskId,
                        ToTaskId = dep.TaskId,
                        IsComplete = fromTask.IsCompleted
                    });
                }
            }

            // 3. Map Nodes and Calculate Levels (Horizontal positioning)
            var levels = new Dictionary<int, int>();
            foreach (var task in tasks.Items)
            {
                levels[task.Id] = CalculateLevel(task.Id, graphData.Edges);

                graphData.Nodes.Add(new GraphNode
                {
                    TaskId = task.Id,
                    Name = processes?.Items?.FirstOrDefault(p => p.Id == task.ProcessId)?.Type.ToString() ?? $"Task {task.Id}",
                    IsComplete = task.IsCompleted,
                    Level = levels[task.Id]
                });
            }

            // 4. Calculate Rows (Vertical branching)
            var levelCounts = new Dictionary<int, int>();
            foreach (var node in graphData.Nodes.OrderBy(n => n.Level))
            {
                if (!levelCounts.ContainsKey(node.Level)) levelCounts[node.Level] = 0;
                node.Row = levelCounts[node.Level];
                levelCounts[node.Level]++;
            }

            return graphData;
        }

        private int CalculateLevel(int taskId, List<GraphEdge> edges, int currentDepth = 0)
        {
            var incoming = edges.Where(e => e.ToTaskId == taskId).ToList();
            if (!incoming.Any()) return currentDepth;

            int maxDepth = currentDepth;
            foreach (var dep in incoming)
            {
                int depth = CalculateLevel(dep.FromTaskId, edges, currentDepth + 1);
                if (depth > maxDepth) maxDepth = depth;
            }
            return maxDepth;
        }
    }
}