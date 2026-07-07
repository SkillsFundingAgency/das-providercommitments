using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class GetApprenticeshipsFiltersResponse
{
    public IEnumerable<string> EmployerNames { get; set; }
    public IEnumerable<string> ProviderNames { get; set; }
    public IEnumerable<string> CourseNames { get; set; }
    public IEnumerable<string> Statuses { get; set; }
    public IEnumerable<DateTime> StartDates { get; set; }
    public IEnumerable<DateTime> EndDates { get; set; }
}