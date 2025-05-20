using Avalonia;
using Avalonia.Controls;

namespace PhotoApp.Views
{
    public partial class Skeleton : UserControl
    {
        public static readonly new StyledProperty<double> CornerRadiusProperty =
            AvaloniaProperty.Register<Skeleton, double>(nameof(CornerRadius), 4.0);

        public new double CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Skeleton()
        {
            InitializeComponent();
        }
    }
} 