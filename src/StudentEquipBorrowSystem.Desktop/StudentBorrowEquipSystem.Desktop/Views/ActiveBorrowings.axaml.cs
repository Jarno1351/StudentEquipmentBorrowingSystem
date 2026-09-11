using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StudentBorrowEquipSystem.Desktop.Views
{
    public partial class ActiveBorrowingsView : UserControl
    {
        public ActiveBorrowingsView()
        {
            InitializeComponent();
            // Resolve ViewModel from the application service provider
            if (App.Services != null)
            {
                this.DataContext = App.Services.GetService(typeof(StudentBorrowEquipSystem.Desktop.ViewModels.ActiveBorrowingsViewModel));
            }
        }
    }
}