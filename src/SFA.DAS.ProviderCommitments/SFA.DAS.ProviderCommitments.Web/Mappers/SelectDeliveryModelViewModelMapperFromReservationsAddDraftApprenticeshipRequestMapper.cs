﻿using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, SelectDeliveryModelViewModel>
    {
        private readonly ISelectDeliveryModelMapperHelper _helper;

        public SelectDeliveryModelViewModelMapperFromReservationsAddDraftApprenticeshipRequestMapper(ISelectDeliveryModelMapperHelper helper)
        {
            _helper = helper;
        }

        public Task<SelectDeliveryModelViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            return _helper.Map(source.ProviderId, source.CourseCode, source.DeliveryModel);
        }
    }
}