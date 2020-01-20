using DirectorySizeWPFasync.Tree;
using DirectorySizeWPFasync.UI;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DirectorySizeWPFasync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WarningTitle = "Warning";
        private const string ErrorTitle = "Error";

        private readonly VistaFolderBrowserDialog directoryDialog = new VistaFolderBrowserDialog();

        private CancellationTokenSource cancelTokenSource;

        public MainWindow()
        {
            InitializeComponent();

            InitializeDirectoryDialog();

            UIHandler.SetBrushes();
        }

        private void InitializeDirectoryDialog()
        {
            directoryDialog.Description = "Select Folder";

            //this applies to the Vista style dialog only, not the old dialog
            directoryDialog.UseDescriptionForTitle = true;
        }

        private void ShowDirectoryDialog(object sender, RoutedEventArgs e)
        {
            if ((bool)directoryDialog.ShowDialog(this))
            {
                pathTextBox.Text = directoryDialog.SelectedPath;                
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            const int maxPathLength = 260;

            if (string.IsNullOrWhiteSpace(pathTextBox.Text))
            {
                MessageBox.Show(this, "The folder path is not set.", WarningTitle);
                return;
            }

            if (pathTextBox.Text.Length > maxPathLength)
            {
                MessageBox.Show(this, "The specified path is too long.", WarningTitle);
                return;
            }

            if (!Directory.Exists(pathTextBox.Text))
            {
                MessageBox.Show(this, "The folder does not exist at the specified path.", WarningTitle);
                return;
            }

            OnStart();

            try
            {
                await StartCalculationAsync();
            }
            catch (SecurityException)
            {
                MessageBox.Show(this, "Access denied.", WarningTitle);
            }
            catch (Exception exception)
            {
                string message;
#if (DEBUG)
                message = exception.ToString();
#else
                message = "Impossible to determine the size of the specified folder.";
#endif
                MessageBox.Show(this, message, ErrorTitle);
            }
            finally
            {
                OnStop();
            }
        }

        private async Task StartCalculationAsync()
        {
            string rootPath = pathTextBox.Text;
            DirectoryInfo root = new DirectoryInfo(rootPath);
            DirTreeNode rootNode = new DirTreeNode(root);
            DirTreeViewItem rootItem = new DirTreeViewItem(rootNode);

            directoriesTreeView.Items.Add(rootItem);
            rootItem.ExpandSubtree();

            cancelTokenSource = new CancellationTokenSource();
            Action checkCancellation = () => cancelTokenSource?.Token.ThrowIfCancellationRequested();

            IProgress<int> progress = new Progress<int>(value => { progressBar.Value = value; });

            try
            {
                await Task.Run(() => rootNode.CountSize(checkCancellation, progress), cancelTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                //the task was canceled by user
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (cancelTokenSource!.IsCancellationRequested == false)
            {
                cancelTokenSource.Cancel();                
            }                
        }

        private void OnStart()
        {
            directoriesTreeView.Items.Clear();

            hintLabel.Visibility = Visibility.Hidden;

            startButton.Visibility = Visibility.Hidden;

            directoryDialogButton.IsEnabled = false;
            pathTextBox.IsEnabled = false;

            stopButton.Visibility = Visibility.Visible;

            progressBar.Value = 0;
            progressBar.Visibility = Visibility.Visible;
        }

        private void OnStop()
        {
            cancelTokenSource.Dispose();

            startButton.Visibility = Visibility.Visible;

            directoryDialogButton.IsEnabled = true;
            pathTextBox.IsEnabled = true;

            stopButton.Visibility = Visibility.Hidden;

            progressBar.Visibility = Visibility.Hidden;
        }        
    }
}