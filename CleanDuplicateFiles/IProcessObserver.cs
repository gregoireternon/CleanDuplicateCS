﻿namespace CleanDuplicateFiles
{
    public interface IProcessObserver
    {
        void RefFolderPRocessed();
        void AdaptRefFileCount(int count);
    }
}