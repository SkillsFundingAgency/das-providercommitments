namespace SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse
{
    public class Provider
    {
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool NationalProvider { get; set; }
    }
}