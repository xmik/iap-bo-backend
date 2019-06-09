namespace BranchOfficeBackend
{
    public interface IConfigurationService
    {
         int GetBranchOfficeId();
         string GetHQServerUrl();

        /// <summary>
        /// If true, then the synchronization loop with HQ will be started automatically
        /// </summary>
        /// <returns></returns>
         bool GetStartSynchronizationLoopWithHQ();

        /// <summary>
        /// Returns integer number of seconds that we'll sleep after 1 synchronization finishes 
        /// </summary>
        /// <returns></returns>
         int GetSynchronizationFrequency();
    }
}