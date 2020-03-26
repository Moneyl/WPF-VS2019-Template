using System.Collections.Generic;
using System.IO;
using System.Linq;
using OGMM.Utility;
using RfgTools.Formats.Packfiles;

namespace OGMM.Types
{
    public class ModManager
    {
        private List<RfgMod> _modList;
        public IReadOnlyList<RfgMod> ModList => _modList;
        public string DataFolderPath { get; set; }

        public ModManager()
        {
            _modList = new List<RfgMod>();
        }

        //Todo: Consider rescanning the folder whenever it changes for easy modding / fixing errors
        public void ScanModsFolder(string modsFolderPath)
        {
            _modList.Clear();
            
            var subFolders = Directory.GetDirectories(modsFolderPath);

            foreach (var subFolder in subFolders)
            {
                string maybeModInfoPath = subFolder + @"\modinfo.xml";
                if (File.Exists(maybeModInfoPath))
                {
                    var folderMod = new RfgMod(maybeModInfoPath);
                    //Todo: Handle parse failure, maybe just mark mod as invalid in gui and have a tooltip or dropdown with the problem
                    folderMod.ParseMetadata();
                    _modList.Add(folderMod);
                }
            }
        }

        //Todo: Support editing files within str2_pc files instead of requiring full replaces
        //Todo: Look into the possibility of a binary patching option for large mods
        public void ActivateSelectedMods(IList<bool> activatedMods)
        {
            foreach (var mod in _modList.ToList())
            {
                //Parse changes again in case the files was edited. Name, Author, Desc, and Input not re-parsed.
                var result = mod.ParseChanges();
                if (!result.Successful)
                {
                    WindowLogger.Log($"Failed to parse modinfo for \"{mod.Name}\". Removing. Error message: \"{result.Message}\"");
                    _modList.Remove(mod);
                }
            }
            
            //Todo: Come up with some kind of batching code so each vpp/str2 is only unpacked/packed once
            var changedVpps = new List<string>();
            //Scan all activated mod changes and make list of changed vpps
            foreach (var mod in _modList)
            {
                foreach (var edit in mod.Edits)
                {
                    changedVpps.AddIfUnique(edit.TopPackfileName);
                }
                foreach (var replacement in mod.Replacements)
                {
                    changedVpps.AddIfUnique(replacement.TopPackfileName);
                }
            }

            //Make backups of each changed vpp before changing them
            foreach (var changedVpp in changedVpps)
            {
                string fullPath = $"{DataFolderPath}\\{changedVpp}";
                string backupFullPath = $"{fullPath}.OGMM_BACKUP";
                File.Copy(fullPath, backupFullPath);
            }

            //For each changed vpp get a list of changed and replaced files, note edit-replace mismatches which might make things incompatible
            //Todo: ^Ideally should mark incompatibility before even pressing the activate button 

            //Unpack changed vpps
            string mmTestFolderPath = $"{DataFolderPath}\\OGMM_TEST\\";
            Directory.CreateDirectory(mmTestFolderPath);

            foreach (var changedVpp in changedVpps)
            {
                string fullPath = $"{DataFolderPath}\\{changedVpp}";
                var packfile = new Packfile(false);
                packfile.ReadMetadata(fullPath);
                packfile.ExtractFileData($"{mmTestFolderPath}{changedVpp}\\OrigUnpack\\");
            }

            //Perform edits and replacements
            //Todo: Figure out if replacements or edits should be done first. Test how current MM does this
            

            //Pack changed vpps

            //Restore files so we're working on vanilla copies
            RestoreFiles();

        }

        public void RestoreFiles()
        {
            //Todo: Might be better just to store a list of changed files and cache them with the mod manager
            //Todo: For now just keep it simple and make a copy of the file with the _Restore prefix, find any with this prefix, strip & restore
            foreach (var file in Directory.GetFiles(DataFolderPath))
            {
                var fileSplit = file.Split('.');
                if(fileSplit.Length != 3 || fileSplit[1] != ".vpp_pc" || fileSplit[2] != ".OGMM_BACKUP")
                    continue; //Todo: Maybe should log an error here

                string srcFilePath = $"{DataFolderPath}\\{file}";
                string dstFilePath = $"{DataFolderPath}\\{fileSplit[0]}{fileSplit[1]}";
                File.Copy(srcFilePath, dstFilePath, true);
            }
        }
    }
}