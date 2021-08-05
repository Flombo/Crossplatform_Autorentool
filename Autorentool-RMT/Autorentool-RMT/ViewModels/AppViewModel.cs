//public ICommand AddCommand { get; private set; }
//public ICommand MinusCommand { get; private set; }

//public ViewModel ()
//{
//    AddCommand = new Command(() => {
//        CurrentValue = CurrentValue + 1;
//    });

//    MinusCommand = new Command(() => {
//        CurrentValue = CurrentValue - 1;
//    });
//}

//public int CurrentValue { get; set; } // You'll need to handle the PropertyChanged events here

using System;
using System.ComponentModel;

namespace Autorentool_RMT.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public AppViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
