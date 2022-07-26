﻿using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ReservationsAddDraftApprenticeshipRequest : BaseReservationsAddDraftApprenticeshipRequest, IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public long? CohortId { get; set; }
    }

    public class BaseReservationsAddDraftApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }

        public BaseReservationsAddDraftApprenticeshipRequest CloneBaseValues() => (BaseReservationsAddDraftApprenticeshipRequest) MemberwiseClone();
    }
}