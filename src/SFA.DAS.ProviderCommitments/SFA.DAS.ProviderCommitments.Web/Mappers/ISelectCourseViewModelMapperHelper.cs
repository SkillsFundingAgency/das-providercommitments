using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public interface ISelectCourseViewModelMapperHelper
    {
        Task<SelectCourseViewModel> Map(string courseCode, long accountLegalEntityId);
    }
}