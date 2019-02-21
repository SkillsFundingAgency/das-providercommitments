using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse
{
    public class FrameworksView
    {
        public DateTime CreatedDate { get; set; }
        public Framework[] Frameworks { get; set; }
    }
}