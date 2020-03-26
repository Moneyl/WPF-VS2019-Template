using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using RfgTools.Helpers;

namespace OGMM.Types
{
    public class RfgMod : ICloneable
    {
        private List<ModEdit> _edits = new List<ModEdit>();
        private List<ModReplace> _replacements = new List<ModReplace>();

        public string ModInfoPath { get; }

        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public XElement UserInput { get; set; }
        public XElement Changes { get; set; } //Todo: Should reload this each time mods are activated

        public IReadOnlyList<ModEdit> Edits => _edits;
        public IReadOnlyList<ModReplace> Replacements => _replacements;

        public RfgMod(string modInfoPath)
        {
            ModInfoPath = modInfoPath;
        }

        /// <summary>
        /// Parse metadata values used by the mod manager, including the user input.
        /// </summary>
        public ParseResult ParseMetadata()
        {
            var modInfo = XElement.Load(ModInfoPath);
            Name = modInfo.Attribute("Name")?.Value;
            Author = modInfo.Element("Author")?.Value;
            Description = modInfo.Element("Description")?.Value;
            UserInput = modInfo.Element("UserInput");
            if(modInfo.Element("Changes") == null)
                return new ParseResult(false, "<Changes> node not present. Required for a mod to be valid.");

            return new ParseResult(true);
        }

        /// <summary>
        /// Parse changes block of the mod info (edits & replacements).
        /// </summary>
        public ParseResult ParseChanges()
        {
            var modInfo = XElement.Load(ModInfoPath);
            Changes = modInfo.Element("Changes");
            if(Changes == null)
                return new ParseResult(false, "<Changes> node not present. Required for a mod to be valid.");

            //Todo: Should be able to handle both RFGR and RFG:SE vpp paths. Maybe just have different modes/profiles or detect which version is at the game path
            //Todo: For now just assume they're RFGR paths

            //Parse edits
            foreach (var edit in Changes.Elements("Edit"))
            {
                //Todo: Look into if there are other LIST_ACTION types other than COMBINE_BY_FIELD
                string file = edit.GetRequiredAttributeValue("File");
                string listAction = edit.GetOptionalAttributeValue("LIST_ACTION", ""); //Todo: Parse this
                //Todo: Handle these on subvalues. Apparently this is a thing? Only seen so far in gibbeds reconstructorSP mod. Example:
                //<Edit File="data\misc.vpp\upgrades.xtbl">

                //<Upgrade>
                //  <Name>super_gauss</Name>
                //  <Type>Weapon</Type>
                //  <Levels LIST_ACTION="COMBINE_BY_FIELD:Value">
                //    <Level>
                //      <Cost>0</Cost>
                //      <Value>0</Value>

                var fileSplit = new string[3];
                if(file.Contains('\\', StringComparison.CurrentCulture))
                    fileSplit = file.Split('\\');
                else if(file.Contains('/', StringComparison.CurrentCulture))
                    fileSplit = file.Split('/');

                if (fileSplit.Length != 3)
                    return new ParseResult(false, $"Improperly formatted edit file path: \"{file}\"");

                string topPackfileName = fileSplit[1];
                string editFilename = fileSplit[2];

                _edits.Add(new ModEdit(topPackfileName, editFilename, listAction, edit));
            }

            //Parse replacements
            foreach (var replacement in Changes.Elements("Replace"))
            {
                string file = replacement.GetRequiredAttributeValue("File");
                string newFile = replacement.GetRequiredAttributeValue("NewFile");
                string newFileFullPath = $"{Path.GetDirectoryName(ModInfoPath)}\\{newFile}";

                if (!File.Exists(newFileFullPath))
                    return new ParseResult(false, $"Replace node NewFile path doesn't exist. Path: \"{newFileFullPath}\"");

                var fileSplit = file.Split('\\');
                if (fileSplit.Length != 3) //Todo: Check if current MM just ignores these or what. Probably better to stop regardless of what current MM does
                    return new ParseResult(false, $"Improperly formatted replace file path: \"{file}\"");

                string topPackfileName = fileSplit[1];
                string replaceFilename = fileSplit[2];

                _replacements.Add(new ModReplace(topPackfileName, replaceFilename, newFileFullPath));
            }

            return new ParseResult(true);
        }

        public object Clone()
        {
            return new RfgMod(ModInfoPath)
            {
                Author = Author,
                Changes = Changes,
                Description = Description,
                Name = Name,
                UserInput = UserInput
            };
        }
    }
}
