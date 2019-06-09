namespace BranchOfficeBackend.Tests
{
    public static class CommonHelpers
    {
        public static string baseUrl = "http://localhost:1234";
        public static IConfigurationService MockConfServ(bool startSynchro=false){
            var cs = new Moq.Mock<IConfigurationService>();
            cs.Setup(m => m.GetHQServerUrl()).Returns(baseUrl);
            cs.Setup(m => m.GetStartSynchronizationLoopWithHQ()).Returns(startSynchro);
            return cs.Object;
        }
    }
}