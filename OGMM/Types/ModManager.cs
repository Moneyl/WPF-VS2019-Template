using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

/* Example modinfo 1:
 * 
 * <RfgMod Name="All Missions Playable">
       <Author>SimpleArrows</Author>
       <Description>All missions are playable from the start.</Description>
       <Changes>
           <Replace File="build\pc\cache\misc.vpp\mission_unlock.xtbl" NewFile="file\mission_unlock.xtbl" />
       </Changes>
   </RfgMod>
*/

namespace OGMM.Types
{
    public class ModManager
    {
        private List<RfgMod> _modList;
        public IReadOnlyList<RfgMod> ModList => _modList;

        public ModManager()
        {
            _modList = new List<RfgMod>();
        }

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
                    folderMod.ParseMetadata();
                    _modList.Add(folderMod);
                }
            }
        }

        public void ActivateSelectedMods(IEnumerable<bool> activatedMods)
        {
            foreach (var mod in _modList)
            {
                //Parse changes again in case the files was edited. Name, Author, Desc, and Input not re-parsed.
                mod.ParseChanges();
            }
            //Todo: Come up with some kind of batching code so each vpp/str2 is only unpacked/packed once
        }

        public void RestoreFiles()
        {

        }
    }
}