using DirectorySizeWPFasync.Models;
using DirectorySizeWPFasync.Observer;
using System;
using System.IO;
using System.Security;

namespace DirectorySizeWPFasync.Tree
{
    internal class DirTreeNode : TreeNode<DirData>, ISubject<DirData>
    {
        private static IProgress<int> progress;
        private static Action checkCancellation;

        public ISubscriber<DirData> Subscriber { get; set; }

        private readonly DirectoryInfo dirInfo;        

        public DirTreeNode(DirectoryInfo dirInfo)
            : base(new DirData(dirInfo.Name))
        {
            this.dirInfo = dirInfo;            
        }

        public DirTreeNode(DirData data) : base(data) { }

        public DirTreeNode AddChild(DirectoryInfo dirInfo)
        {
            DirTreeNode childNode = new DirTreeNode(dirInfo);            
            Children.Add(childNode);
            Subscriber.AddChild(childNode);

            return childNode;
        }

        public new DirTreeNode AddChild(DirData data)
        {
            DirTreeNode childNode = new DirTreeNode(data);            
            Children.Add(childNode);
            Subscriber.AddChild(childNode);

            return childNode;
        }

        public void CountSize(Action checkCancellation, IProgress<int> progress)
        {
            DirTreeNode.checkCancellation = checkCancellation;
            DirTreeNode.progress = progress;

            ProcessCountSize(isRoot: true);
        }

        private long ProcessCountSize(bool isRoot = false)
        {
            checkCancellation();

            long size = 0;

            try
            {
                DirectoryInfo[] subDirectories = dirInfo.GetDirectories();

                //size of all subdirectories is calculated recursively
                size += CountSubDirectoriesSize(subDirectories, isRoot);

                FileInfo[] dirFiles = dirInfo.GetFiles();

                bool addFilesSizeItem = (subDirectories.Length > 0);
                size += CountFilesSize(dirFiles, addFilesSizeItem);

                Data.Size = new SizeInfo(size);
                Subscriber.AddSize(Data);
            }
            catch (SecurityException)
            {
                Subscriber.AddAccessDenied();
            }
            catch (UnauthorizedAccessException)
            {
                Subscriber.AddAccessDenied();
            }

            return size;
        }

        private long CountSubDirectoriesSize(DirectoryInfo[] subDirectories, bool isRoot)
        {
            long resultSize = 0;

            for (int i = 0; i < subDirectories.Length; i++)
            {
                DirectoryInfo subDirInfo = subDirectories[i];

                DirTreeNode childNode = AddChild(subDirInfo);                

                resultSize += childNode.ProcessCountSize();

                if (isRoot)
                {
                    //max value = number of subdirectories in the root
                    //+1 because of files in the root itself
                    ReportProgress(i + 1, subDirectories.Length + 1);
                }
#if (DEBUG)
                System.Threading.Thread.Sleep(100);
#endif
            }

            return resultSize;
        }

        private void ReportProgress(int part, int whole)
        {
            int percentComplete = (int)Math.Floor(part / (float)whole * 100);
            progress.Report(percentComplete);
        }

        private long CountFilesSize(FileInfo[] dirFiles, bool addSizeItem)
        {
            long resultSize = 0;

            foreach (FileInfo file in dirFiles)
            {
                try
                {
                    resultSize += file.Length;
                }
                catch (IOException)
                {
                    DirData data = new DirData(file.Name);

                    DirTreeNode childNode = AddChild(data);                    
                    childNode.Subscriber.AddSize(data);
                }
            }

            if (resultSize > 0 && addSizeItem)
            {
                DirData data = new DirData();
                data.Size = new SizeInfo(resultSize);
                data.FilesOnly = true;

                DirTreeNode childNode = AddChild(data);
                childNode.Subscriber.AddSize(data);
            }

            return resultSize;
        }        
    }
}