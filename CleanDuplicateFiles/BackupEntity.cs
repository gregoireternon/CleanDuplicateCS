﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDuplicateFiles
{
    public class BackupEntity
    {
        public Dictionary<string, List<string>> Refs { get; set; }

        public string RefUrl { get; set; }
    }
}
