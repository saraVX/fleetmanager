using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FleetManager.Views;

public partial class VehiclesView : UserControl
{
    public VehiclesView()
    {
        InitializeComponent();
    }
    
    public List<string> StatusList => new() { "Disponible", "En maintenance", "Hors service" };
}
