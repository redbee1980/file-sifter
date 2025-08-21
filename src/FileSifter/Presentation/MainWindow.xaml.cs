using System.Windows;
using FileSifter.Presentation.ViewModels;

namespace FileSifter.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}