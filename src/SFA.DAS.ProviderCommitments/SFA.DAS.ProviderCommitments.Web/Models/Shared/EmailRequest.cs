using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared
{
    public class EmailRequest: DraftApprenticeshipRequest
    {
      
        public string Email { get; set; }

        public string Name { get; set; }
    }

    public class EmailViewModel:DraftApprenticeshipRequest
    {
       
        public string Email { get; set; }

        public string Name {  get; set; }
    }

}
