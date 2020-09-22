namespace CleanDuplicateFiles
{
    public interface IProcessObserver
    {
        void RefFolderPRocessed();
        void AdaptRefFileCount(int count);
        void ToCompareComptationFinished(int count);
        void ToCleanFinished(int count);
    }
}