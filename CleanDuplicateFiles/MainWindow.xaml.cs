using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CleanDuplicateFiles
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IProcessObserver
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        Processor _processor;
        

        public MainWindow()
        {
            _log.Info("create Main window");
            InitializeComponent();
            _processor = new Processor(this);
            RefFolderUrl.Text = _processor.RefPath;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fddialog = new FolderBrowserDialog();
            DialogResult result = fddialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                RefFolderUrl.Text = fddialog.SelectedPath;
                _processor.RefPath = fddialog.SelectedPath;
            }
        }

        private void ComputeRefHashes_Click(object sender, RoutedEventArgs e)
        {
            ToggleBtnEnable(false);
            Thread t = new Thread(_processor.ProcessFolder);
            t.Start();
        }

        public void RefFolderPRocessed()
        {
            Dispatcher.BeginInvoke(new Action(() => {
                ToggleBtnEnable(true);
            }));
            
        }

        public void AdaptRefFileCount(int countHash, int countFile)
        {
            Dispatcher.BeginInvoke(new Action(()=> {
                indexedFilesCounter.Content = "Nb de hashs en référence: " + countHash + "\nnb de fichiers total:"+countFile;
            }));
            
        }

        private void ToggleBtnEnable(bool value)
        {
            ComputeRefHashes.IsEnabled = value;
            ChooseToCompare.IsEnabled = value;
            ChooseRefFolder.IsEnabled = value;
            ComputeDuplicate.IsEnabled = value;
            CleanDuplicate.IsEnabled = value;
        }

        private void ChooseToCompare_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fddialog = new FolderBrowserDialog();
            DialogResult result = fddialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ToCleanFolder.Text = fddialog.SelectedPath;
                _processor.ToComparePath = fddialog.SelectedPath;
            }
        }

        private void ComputeDuplicate_Click(object sender, RoutedEventArgs e)
        {
            ToggleBtnEnable(false);
            try
            {
                Thread t = new Thread(_processor.ComputeDuplicate);
                t.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error:" + ex.Message);
                ToggleBtnEnable(true);
            }

        }

        public void ToCompareComptationFinished(int count)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                indexedFilesCounter.Content = "Nb de fichiers en doublon: " + count;
                ToggleBtnEnable(true);
            }));
        }

        private void CleanDuplicate_Click(object sender, RoutedEventArgs e)
        {
            ToggleBtnEnable(false);
            try
            {
                Thread t = new Thread(_processor.CleanDuplicate);
                t.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error:" + ex.Message);
                ToggleBtnEnable(true);
            }
        }

        public void ToCleanFinished(int count)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                indexedFilesCounter.Content = "Nb de fichiers supprimés: " + count;
                ToggleBtnEnable(true);
            }));
        }

        private void ChooseRefFolder_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                RefFolderUrl.Text = files[0];
                _processor.RefPath = files[0];
            }
            _log.Debug("Drop a folder? "+ e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop));
        }

        private void ChooseRefFolder_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            //_log.Debug("Enter? " + e.Data);
            //e.Effects = System.Windows.DragDropEffects.;
        }

        private void ChooseToCompare_DragEnter(object sender, System.Windows.DragEventArgs e)
        {

        }

        private void ChooseToCompare_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                ToCleanFolder.Text = files[0];
                _processor.ToComparePath = files[0];
            }
            _log.Debug("Drop a folder? " + e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop));
        }
    }
}
