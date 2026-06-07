using Microsoft.Maui.Graphics;
using Producion_Line_Manager.UI;

namespace Producion_Line_Manager.Controls
{
    public class ProductionGraphDrawable : IDrawable
    {
        public ProductionGraphData Data { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (Data == null || !Data.Nodes.Any()) return;

            float nodeRadius = 8f;
            float colWidth = 100f;
            float rowHeight = 50f;
            float startX = 30f;
            float startY = 40f;

            PointF GetPoint(int taskId)
            {
                var node = Data.Nodes.First(n => n.TaskId == taskId);
                return new PointF(startX + (node.Level * colWidth), startY + (node.Row * rowHeight));
            }

            // Draw Edges (Lines behind nodes)
            canvas.StrokeSize = 3;
            foreach (var edge in Data.Edges)
            {
                var p1 = GetPoint(edge.FromTaskId);
                var p2 = GetPoint(edge.ToTaskId);
                canvas.StrokeColor = edge.IsComplete ? Colors.LimeGreen : Colors.LightGray;
                canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
            }

            // Draw Nodes & Labels
            foreach (var node in Data.Nodes)
            {
                var center = GetPoint(node.TaskId);

                // Node Circle
                canvas.FillColor = node.IsComplete ? Colors.LimeGreen : Colors.LightGray;
                canvas.FillEllipse(center.X - nodeRadius, center.Y - nodeRadius, nodeRadius * 2, nodeRadius * 2);

                // Text Label
                canvas.FontColor = Colors.Black;
                canvas.FontSize = 12;
                canvas.DrawString(node.Name,
                    center.X - (colWidth / 2), center.Y - nodeRadius - 20,
                    colWidth, 20,
                    HorizontalAlignment.Center, VerticalAlignment.Bottom);
            }
        }
    }
}