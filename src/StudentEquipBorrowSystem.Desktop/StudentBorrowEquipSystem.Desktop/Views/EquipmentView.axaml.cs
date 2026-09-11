using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StudentBorrowEquipSystem.Desktop.Views
{
    public partial class EquipmentView : UserControl
    {
        public EquipmentView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        }
    }
}