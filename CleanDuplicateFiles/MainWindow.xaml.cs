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

        public void AdaptRefFileCount(int count)
        {
            Dispatcher.BeginInvoke(new Action(()=> {
                indexedFilesCounter.Content = "Nb de fichiers en référence: " + count;
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
                _processor.ComputeDuplicate();
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
                indexedFilesCounter.Content = "Nb de fichiers en référence: " + count;
                ToggleBtnEnable(true);
            }));
        }

        private void CleanDuplicate_Click(object sender, RoutedEventArgs e)
        {
            ToggleBtnEnable(false);
            try
            {
                _processor.CleanDuplicate();
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
    }
}
