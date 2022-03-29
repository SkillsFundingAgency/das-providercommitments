﻿using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectCourseViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectCourseViewModel>
    {
        private readonly ISelectCourseViewModelMapperHelper _selectCourseViewModelHelper;

        public SelectCourseViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper(ISelectCourseViewModelMapperHelper selectCourseViewModelHelper)
        {
            _selectCourseViewModelHelper = selectCourseViewModelHelper;
        }

        public Task<SelectCourseViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            return _selectCourseViewModelHelper.Map(source.CourseCode, source.AccountLegalEntityId) ;
        }
    }
}