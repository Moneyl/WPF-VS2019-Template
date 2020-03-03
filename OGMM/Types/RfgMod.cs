using System;
using System.Xml.Linq;

namespace OGMM.Types
{
    public class RfgMod : ICloneable
    {
        private readonly string _path;
        public string Path => _path;

        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public XElement UserInput { get; set; }
        public XElement Changes { get; set; } //Todo: Should reload this each time mods are activated

        public RfgMod(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Parse metadata values used by the mod manager, including the user input.
        /// </summary>
        public void ParseMetadata()
        {
            var modInfo = XElement.Load(Path);
            Name = modInfo.Attribute("Name")?.Value;
            Author = modInfo.Element("Author")?.Value;
            Description = modInfo.Element("Description")?.Value;
            UserInput = modInfo.Element("UserInput");
        }

        /// <summary>
        /// Parse changes block of the mod info (edits & replacements).
        /// </summary>
        public void ParseChanges()
        {
            var modInfo = XElement.Load(Path);
            Changes = modInfo.Element("Changes");
        }

        public object Clone()
        {
            return new RfgMod(Path)
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
