using System.Windows;
using OGMM.ViewModels;
using ReactiveUI;
using Splat;

namespace OGMM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<AppViewModel>));
        }
    }
}
