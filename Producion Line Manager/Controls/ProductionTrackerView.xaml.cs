using Producion_Line_Manager.UI;

namespace Producion_Line_Manager.Controls
{
    public partial class ProductionTrackerView : ContentView
    {
        private readonly ProductionGraphDrawable _drawable;

        public ProductionTrackerView()
        {
            InitializeComponent();
            _drawable = new ProductionGraphDrawable();
            GraphCanvas.Drawable = _drawable;
        }

        public static readonly BindableProperty GraphDataProperty =
            BindableProperty.Create(nameof(GraphData), typeof(ProductionGraphData), typeof(ProductionTrackerView), propertyChanged: OnDataChanged);

        public ProductionGraphData GraphData
        {
            get => (ProductionGraphData)GetValue(GraphDataProperty);
            set => SetValue(GraphDataProperty, value);
        }

        private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ProductionTrackerView)bindable;
            control._drawable.Data = (ProductionGraphData)newValue;
            control.GraphCanvas.Invalidate();
        }
    }
}