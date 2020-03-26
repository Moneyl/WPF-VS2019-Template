
namespace OGMM.Types
{
    public class ModReplace
    {
        public string TopPackfileName { get; }
        public string ReplaceFilename { get; }
        public string NewFilePath { get; }

        public ModReplace(string topPackfileName, string replaceFilename, string newFilePath)
        {
            TopPackfileName = topPackfileName;
            ReplaceFilename = replaceFilename;
            NewFilePath = newFilePath;
        }
    }
}
