using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CleanDuplicateFiles
{
    public class Processor
    {
        public string RefPath { get; set; }


        HashSet<string> _fileHashes = null;
        bool locked = false;
        IProcessObserver _observer;

        public Processor(IProcessObserver observer)
        {
            _observer = observer;
        }

        public void ProcessFolder()
        {
            if (locked) { throw new Exception("Already locked"); }
            locked = true;
            _fileHashes = new HashSet<string>();
            ProcessFolder(RefPath);
            File.WriteAllText("backup.data", JsonConvert.SerializeObject(new BackupEntity()
            {
                RefUrl=RefPath,
                Refs=_fileHashes
            }));
            locked = false;
            _observer.RefFolderPRocessed();
        }

        private void ProcessFolder(string path)
        {
            Console.WriteLine("processing folder: " + path);
            string[] currentPAthFiles = Directory.GetFiles(path);
            foreach (string f in currentPAthFiles)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(f))
                    {
                        string b64Res = Convert.ToBase64String(md5.ComputeHash(stream));
                        _fileHashes.Add(b64Res);
                        Console.WriteLine(f + ":" + b64Res);
                    }
                }
            }
            foreach (string d in Directory.GetDirectories(path))
            {
                ProcessFolder(d);
            }
            _observer.AdaptRefFileCount(_fileHashes.Count);
        }
    }
}
