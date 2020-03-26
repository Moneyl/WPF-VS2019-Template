using System.Xml.Linq;

namespace OGMM.Types
{
    public class ModEdit
    {
        public string TopPackfileName { get; }
        public string EditFilename { get; }
        public string ListAction { get; }
        public XElement Edits { get; }

        public ModEdit(string topPackfileName, string editFilename, string listAction, XElement edits)
        {
            TopPackfileName = topPackfileName;
            EditFilename = editFilename;
            ListAction = listAction;
            Edits = edits;
        }
    }
}
