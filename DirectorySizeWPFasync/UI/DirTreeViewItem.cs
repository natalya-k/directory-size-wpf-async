using DirectorySizeWPFasync.Models;
using DirectorySizeWPFasync.Observer;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace DirectorySizeWPFasync.UI
{
    internal class DirTreeViewItem : TreeViewItem, ISubscriber<DirData>
    {
        private const int MinMByteSizeToMark = 500;

        public DirTreeViewItem(ISubject<DirData> subject)
        {
            subject.Subscriber = this;

            if (!string.IsNullOrEmpty(subject.Data.Name))
            {
                Header = subject.Data.Name;
            }
            else if (subject.Data.FilesOnly)
            {
                Header = "<files>";
            }
            else
            {
                Header = string.Empty;
            }
        }

        public void AddSize(DirData data)
        {
            UIHandler.Execute(new Action(() => {
                if (!data.Size.HasValue)
                {
                    Header += " [size undefined]";
                    Background = UIHandler.PaleRed;

                    return;
                }

                SizeInfo sizeInfo = data.Size.Value;

                Header += $" [{sizeInfo.ToString()}]";

                if (data.FilesOnly)
                {
                    Background = UIHandler.PaleGreen;
                }
                else if (sizeInfo.Size == 0)
                {
                    Background = Brushes.LightGray;
                }
                else if (sizeInfo.Unit == SizeInfo.SizeUnit.MBYTE && sizeInfo.Size >= MinMByteSizeToMark)
                {
                    Background = Brushes.LightBlue;
                }
                else if (sizeInfo.Unit == SizeInfo.SizeUnit.GBYTE)
                {
                    Background = Brushes.Thistle;
                }
            }));
        }

        public void AddAccessDenied()
        {
            UIHandler.Execute(new Action(() => {
                Header += " [access denied]";
                Background = UIHandler.PaleRed;
            }));
        }

        public void AddChild(ISubject<DirData> subject)
        {
            UIHandler.Execute(new Action(() => {
                DirTreeViewItem childItem = new DirTreeViewItem(subject);
                Items.Add(childItem);
            }));
        }
    }
}