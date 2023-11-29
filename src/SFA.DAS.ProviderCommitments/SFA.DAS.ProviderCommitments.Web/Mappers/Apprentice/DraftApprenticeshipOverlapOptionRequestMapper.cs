using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DraftApprenticeshipOverlapOptionRequestMapper : IMapper<ChangeOfEmployerOverlapAlertViewModel, DraftApprenticeshipOverlapOptionRequest>
    {

        public DraftApprenticeshipOverlapOptionRequestMapper()
        {
        }

        public async Task<DraftApprenticeshipOverlapOptionRequest> Map(ChangeOfEmployerOverlapAlertViewModel source)
        {
            return new DraftApprenticeshipOverlapOptionRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId
            };

        }
    }
}