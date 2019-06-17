using System;

namespace SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse
{
    public class FrameworksView
    {
        public DateTime CreatedDate { get; set; }
        public Framework[] Frameworks { get; set; }
    }
}