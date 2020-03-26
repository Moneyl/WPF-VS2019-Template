using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Windows.Forms;
using OGMM.Types;
using OGMM.Utility;
using ReactiveUI;

namespace OGMM.ViewModels
{
    public class AppViewModel : ReactiveObject
    {
        private OpenFileDialog _fileBrowser = new OpenFileDialog();
        private FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
        private ModManager _modManager = new ModManager();

        public ObservableCollection<CheckedListItem<RfgMod>> Mods { get; set; } = new ObservableCollection<CheckedListItem<RfgMod>>();
        public int SelectedIndex { get; set; } = -1;
        public IList SelectedItems { get; set; } = new List<object>();
        public RfgMod SelectedMod =>
            _modManager.ModList.Count <= SelectedIndex && SelectedIndex > -1
                ? null
                : _modManager.ModList[SelectedIndex];

        public readonly ReactiveCommand<Unit, Unit> SelectGameFolderCommand;
        public readonly ReactiveCommand<Unit, Unit> ActivateModsCommand;
        public readonly ReactiveCommand<Unit, Unit> RestoreFilesCommand;
        public readonly ReactiveCommand<Unit, Unit> RunGameCommand;
        public readonly ReactiveCommand<Unit, Unit> ShowAboutMessageCommand;


        public AppViewModel()
        {
            SelectGameFolderCommand = ReactiveCommand.Create(SelectGameFolder);
            ActivateModsCommand = ReactiveCommand.Create(ActivateMods);
            RestoreFilesCommand = ReactiveCommand.Create(RestoreFiles);
            RunGameCommand = ReactiveCommand.Create(RunGame);
            ShowAboutMessageCommand = ReactiveCommand.Create(ShowAboutMessage);

#if DEBUG
            _fileBrowser = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Program Files\SteamLibrary\steamapps\common\Red Faction Guerrilla Re-MARS-tered\"
            };
#else
            _fileBrowser = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Red Faction Guerrilla Re-MARS-tered\"
            };
#endif
        }

#if DEBUG
        public void DebugInit()
        {
            TrySetGameFolder(@"C:\Program Files\SteamLibrary\steamapps\common\Red Faction Guerrilla Re-MARS-tered\");
        }
#endif

        private void SelectGameFolder()
        {
            if (_folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFolderPath = _folderBrowser.SelectedPath;
                TrySetGameFolder(selectedFolderPath);
            }
        }

        private void TrySetGameFolder(string gameFolderPath)
        {
            if (!File.Exists(gameFolderPath + @"\rfg.exe"))
            {
                MessageBox.Show("Invalid game folder! Please make sure that rfg.exe is in the folder you selected.", "Invalid game folder");
                WindowLogger.Log("Invalid game folder! Please make sure that rfg.exe is in the folder you selected.");
                return;
            }
            if (!Directory.Exists(gameFolderPath + @"\mods\"))
            {
                MessageBox.Show("Mods folder not found! Please make sure that your mods folder is in the folder you selected.", "Mods folder not found");
                WindowLogger.Log("Mods folder not found! Please make sure that your mods folder is in the folder you selected.");
                return;
            }
            WindowLogger.Log($"Set game folder to \"{gameFolderPath}\"");

            _modManager.ScanModsFolder(gameFolderPath + @"\mods\");
            _modManager.DataFolderPath = gameFolderPath + @"\data\"; //Todo: Let mod and game folders be in separate locations

            SelectedIndex = 0;
            UpdateModList();
        }

        private void ActivateMods()
        {
            WindowLogger.Log("Activating mods...");
            var selectionState = new List<bool>();
            foreach (var item in Mods)
            {
                selectionState.Add(item.IsChecked);
            }
            _modManager.ActivateSelectedMods(selectionState);
            WindowLogger.Log("Mods activated!");
        }

        private void RestoreFiles()
        {

        }

        private void RunGame()
        {
            //Todo: Have different behavior for GOG and steam versions + re-mars-tered vs steam edition

        }

        private void ShowAboutMessage()
        {
            MessageBox.Show("OGMM is an open source mod manager for Red Faction Guerrilla Re-Mars-tered. " +
                            "For more info, see it's github page: https://github.com/Moneyl/OGMM", "About OGMM",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Todo: Figure out way to make this asynchronous, and show loading indicator
        private void UpdateModList()
        {
            Mods.Clear();
            foreach (var mod in _modManager.ModList)
            {
                RfgMod copy = mod;
                if (mod.UserInput != null)
                    copy.Name += "(UNSUPPORTED)";

                Mods.Add(new CheckedListItem<RfgMod>(copy));
            }
        }
    }
}