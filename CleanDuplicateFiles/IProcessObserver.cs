namespace CleanDuplicateFiles
{
    public interface IProcessObserver
    {
        void RefFolderPRocessed();
        void AdaptRefFileCount(int countHash,int countFile);
        void ToCompareComptationFinished(int count);
        void ToCleanFinished(int count);
    }
}