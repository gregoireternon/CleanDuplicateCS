using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

        Processor _processor;
        

        public MainWindow()
        {
            InitializeComponent();
            _processor = new Processor(this);
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
            ComputeRefHashes.IsEnabled = false;
            Thread t = new Thread(_processor.ProcessFolder);
            t.Start();
        }

        public void RefFolderPRocessed()
        {
            Dispatcher.BeginInvoke(new Action(() => {
                ComputeRefHashes.IsEnabled = true;
            }));
            
        }

        public void AdaptRefFileCount(int count)
        {
            Dispatcher.BeginInvoke(new Action(()=> {
                indexedFilesCounter.Content = "Nb de fichiers en référence: " + count;
            }));
            
        }
      
    }
}
