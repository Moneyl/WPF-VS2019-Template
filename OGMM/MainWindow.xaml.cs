using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using OGMM.Types;
using OGMM.Utility;
using OGMM.ViewModels;
using ReactiveUI;
using Label = System.Windows.Controls.Label;

namespace OGMM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<AppViewModel>
    {
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AppViewModel)value;
        }
        public AppViewModel ViewModel { get; set; }

        public MainWindow()
        {
            //SettingsManager.Init();

            InitializeComponent();
            ViewModel = new AppViewModel();
            WindowLogger.SetLogPanel(LogBox);

            this.WhenActivated(disposable =>
            {
                this.BindCommand(ViewModel, vm => vm.SelectGameFolderCommand, v => v.MenuItemSelectGameFolder).DisposeWith(disposable);
                this.BindCommand(ViewModel, vm => vm.ActivateModsCommand, v => v.MenuItemActivateMods).DisposeWith(disposable);
                this.BindCommand(ViewModel, vm => vm.RestoreFilesCommand, v => v.MenuItemRestoreFiles).DisposeWith(disposable);
                this.BindCommand(ViewModel, vm => vm.RunGameCommand, v => v.MenuItemRunGame).DisposeWith(disposable);
                this.BindCommand(ViewModel, vm => vm.ShowAboutMessageCommand, v => v.MenuItemAbout).DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.Mods, v => v.ModList.ItemsSource).DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.SelectedItems, v => v.ModList.SelectedItems).DisposeWith(disposable);

                this.Bind(ViewModel, vm => vm.SelectedIndex, v => v.ModList.SelectedIndex).DisposeWith(disposable);
            });
        }

        private void MenuExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ModList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateModInfoPanel();
        }

        //Todo: Replace this with data bindings with ViewModel data
        private void UpdateModInfoPanel()
        {
            ModInfoPanel.Children.Clear();

            if (ViewModel.SelectedMod == null)
                return;

            AddSectionHeaderToModInfoPanel(ViewModel.SelectedMod.Name, 16.0);
            AddValueToModInfoPanel("Author:", ViewModel.SelectedMod.Author);
            AddValueToModInfoPanel("Description:", ViewModel.SelectedMod.Description, false);
        }

        private void AddSectionHeaderToModInfoPanel(string label, double fontSize)
        {
            ModInfoPanel.Children.Add(new Label { Content = new TextBlock { Text = label, FontSize = fontSize, FontWeight = FontWeights.Bold} });
        }

        private void AddValueToModInfoPanel(string label, string value, bool sameLine = true)
        {
            if (sameLine)
            {
                var wrapPanel = new WrapPanel();
                var nameLabel = new Label { Content = label, FontWeight = FontWeights.Bold };
                var valueLabel = new TextBlock { Text = value, Margin = new Thickness(0, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };

                wrapPanel.Children.Add(nameLabel);
                wrapPanel.Children.Add(valueLabel);
                ModInfoPanel.Children.Add(wrapPanel);
            }
            else
            {
                var nameLabel = new Label { Content = label, FontWeight = FontWeights.Bold };
                var valueLabel = new Label { Content = new TextBlock { Text = value, Margin = new Thickness(0, 0, 0, 0), TextWrapping = TextWrapping.Wrap } };

                ModInfoPanel.Children.Add(nameLabel);
                ModInfoPanel.Children.Add(valueLabel);
            }

        }

        private void MenuDebugInit_OnClick(object sender, RoutedEventArgs e)
        {
#if DEBUG
            ViewModel.DebugInit();
            UpdateModInfoPanel();
#endif
        }
    }
}
