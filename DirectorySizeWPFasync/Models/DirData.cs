namespace DirectorySizeWPFasync.Models
{
    internal class DirData
    {
        public string Name { get; set; }
        public SizeInfo? Size { get; set; }
        public bool FilesOnly { get; set; }        

        public DirData(string name = null)
        {
            Name = name;
            Size = null;
            FilesOnly = false;
        }
    }
}