using System;

namespace BranchOfficeBackend
{
    public class ConfigurationService : IConfigurationService
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ConfigurationService)); 
        private string hqServerUrl;
        private int branchOfficeId;
        private int synchroFreqSeconds;
        private bool startSynchro;
    
        public ConfigurationService()
        {
            int defaultBoid = 0;
            string boidEnv = "IAP_BO_BRANCH_OFFICE_ID";
            string boidFromEnv = Environment.GetEnvironmentVariable(boidEnv);
            if (boidFromEnv == null || boidFromEnv == "")
            {
                _log.DebugFormat("{0} not set, setting default value: {1}", boidEnv, defaultBoid);
                this.branchOfficeId = defaultBoid;
            } else {
                this.branchOfficeId = Int32.Parse(boidFromEnv);
            }

            string defaultServerUrl = "http://localhost:8000";
            string serverUrlEnv = "IAP_BO_HQ_SERVER_URL";
            string serverUrlFromEnv = Environment.GetEnvironmentVariable(serverUrlEnv);
            if (serverUrlFromEnv == null || serverUrlFromEnv == "")
            {
                _log.DebugFormat("{0} not set, setting default value: {1}", serverUrlEnv, defaultServerUrl);
                this.hqServerUrl = defaultServerUrl;
            } else {
                this.hqServerUrl = serverUrlFromEnv;
            }

            bool defaultStartSynchro = true;
            string synchroEnv = "IAP_BO_HQ_SYNCHRO";
            string synchroFromEnv = Environment.GetEnvironmentVariable(synchroEnv);
            if (synchroFromEnv == null || synchroFromEnv == "")
            {
                _log.DebugFormat("{0} not set, setting default value: {1}", synchroEnv, defaultStartSynchro);
                this.startSynchro = defaultStartSynchro;
            } else {
                this.startSynchro = Boolean.Parse(synchroFromEnv);
            }

            int defaultSynchroFreq = 5;
            string synchroFreqEnv = "IAP_BO_SYNCHRONIZATION_FREQUENCY";
            string synchroFreqFromEnv = Environment.GetEnvironmentVariable(synchroFreqEnv);
            if (synchroFreqFromEnv == null || synchroFreqFromEnv == "")
            {
                _log.DebugFormat("{0} not set, setting default value: {1}", synchroFreqEnv, defaultSynchroFreq);
                this.synchroFreqSeconds = defaultSynchroFreq;
            } else {
                this.synchroFreqSeconds = Int32.Parse(synchroFreqFromEnv);
            }
        }
        public int GetBranchOfficeId()
        {
            return this.branchOfficeId;
        }
        public void SetBranchOfficeId(int id)
        {
            this.branchOfficeId = id;
        }

        public string GetHQServerUrl()
        {
            return this.hqServerUrl;
        }

        public void SetHQServerUrl(string serverUrl)
        {
            this.hqServerUrl = serverUrl;
        }

        public bool GetStartSynchronizationLoopWithHQ()
        {
            return this.startSynchro;
        }

        public void SetStartSynchro(bool startSynchro)
        {
            this.startSynchro = startSynchro;
        }

        public int GetSynchronizationFrequency()
        {
            return synchroFreqSeconds;
        }
    }   
}