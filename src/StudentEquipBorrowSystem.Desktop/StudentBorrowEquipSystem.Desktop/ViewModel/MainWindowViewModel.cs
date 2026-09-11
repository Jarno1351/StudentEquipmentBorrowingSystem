using System;
using System.Collections.Generic;
using StudentBorrowEquipSystem.Desktop.ViewModels;

namespace StudentBorrowEquipSystem.Desktop.ViewModel
{
    public class MainWindowViewModel
    {
        public BorrowingViewModel BorrowingVM { get; }
        public ActiveBorrowingsViewModel ActiveBorrowingsVM { get; }

        public MainWindowViewModel(BorrowingViewModel borrowingVM, ActiveBorrowingsViewModel activeBorrowingsVM)
        {
            BorrowingVM = borrowingVM;
            ActiveBorrowingsVM = activeBorrowingsVM;
        }
    }
}
