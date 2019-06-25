using System;

namespace SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse
{
    public class StandardsView
    {
        public DateTime CreationDate { get; set; }
        public Standard[] Standards { get; set; }
    }
}