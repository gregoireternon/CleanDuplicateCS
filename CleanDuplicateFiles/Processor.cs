using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace CleanDuplicateFiles
{
    public class Processor
    {
        public string RefPath { get; set; }
        public string ToComparePath { get; set; }

        public const string BACKUPFILE_LOCATION = "backup.data";


        HashSet<string> _fileHashes = null;
        bool locked = false;
        IProcessObserver _observer;

        public Processor(IProcessObserver observer)
        {
            _observer = observer;
            if (File.Exists(BACKUPFILE_LOCATION))
            {
                BackupEntity bckupEnt = JsonConvert.DeserializeObject<BackupEntity>(File.ReadAllText(BACKUPFILE_LOCATION));
                _fileHashes = bckupEnt.Refs;
                RefPath = bckupEnt.RefUrl;
            }
        }

        public void ProcessFolder()
        {
            if (locked) { throw new Exception("Already locked"); }
            locked = true;
            _fileHashes = new HashSet<string>();
            ProcessFolder(RefPath, _fileHashes, _observer.AdaptRefFileCount);
            File.WriteAllText(BACKUPFILE_LOCATION, JsonConvert.SerializeObject(new BackupEntity()
            {
                RefUrl=RefPath,
                Refs=_fileHashes
            }));
            locked = false;
            _observer.RefFolderPRocessed();
        }

        private void ProcessFolder(string path, HashSet<string> fHashes, Action<int> observerMethod)
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
                        fHashes.Add(b64Res);
                        Console.WriteLine(f + ":" + b64Res);
                    }
                }
            }
            foreach (string d in Directory.GetDirectories(path))
            {
                ProcessFolder(d, fHashes, observerMethod);
            }
            if (observerMethod != null)
            {
                observerMethod(fHashes.Count);
            }
        }

        public void ComputeDuplicate()
        {
            if(string.IsNullOrEmpty(RefPath) || !Directory.Exists(RefPath))
            {
                throw new Exception("Vous devez choisir un répertoire de base (cible)");
            }
            if(string.IsNullOrEmpty(ToComparePath) || !Directory.Exists(ToComparePath))
            {
                throw new Exception("Vous devez choisir un répertoire à comparer");
            }
            if (locked) { throw new Exception("Already locked"); }
            locked = true;
            Dictionary<string, string> files = new Dictionary<string,string>();
            CollectToCompareHashes(ToComparePath, files);
            locked = false;

            files=files.Where(a => _fileHashes.Contains(a.Value)).ToDictionary(a=>a.Key,b=>b.Value);

            _observer.ToCompareComptationFinished(files.Count);

        }

        internal void CleanDuplicate()
        {
            if (string.IsNullOrEmpty(RefPath) || !Directory.Exists(RefPath))
            {
                throw new Exception("Vous devez choisir un répertoire de base (cible)");
            }
            if (string.IsNullOrEmpty(ToComparePath) || !Directory.Exists(ToComparePath))
            {
                throw new Exception("Vous devez choisir un répertoire à comparer");
            }
            if (locked) { throw new Exception("Already locked"); }
            locked = true;
            Dictionary<string, string> files = new Dictionary<string, string>();
            CollectToCompareHashes(ToComparePath, files);
            locked = false;

            files = files.Where(a => _fileHashes.Contains(a.Value)).ToDictionary(a => a.Key, b => b.Value);

            foreach(var f in files)
            {
                File.Delete(f.Key);
            }

            _observer.ToCleanFinished(files.Count);
        }

        private void CollectToCompareHashes(string toComparePath, Dictionary<string, string> files)
        {
            Console.WriteLine("processing folder: " + toComparePath);
            string[] currentPAthFiles = Directory.GetFiles(toComparePath);
            foreach (string f in currentPAthFiles)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(f))
                    {
                        string b64Res = Convert.ToBase64String(md5.ComputeHash(stream));
                        files.Add(f, b64Res);
                        Console.WriteLine(f + ":" + b64Res);
                    }
                }
            }
            foreach (string d in Directory.GetDirectories(toComparePath))
            {
                CollectToCompareHashes(d, files);
            }
        }
    }
}
