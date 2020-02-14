namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DownloadViewModel
    {
        public byte[] Content { get; set; }
        public string Name { get; set; }
        public string ContentType => "text/csv";
    }
}
