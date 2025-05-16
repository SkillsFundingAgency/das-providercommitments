using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;

public class GetLearnerSelectedRequest(long providerId, long learnerId) : IGetApiRequest
{
    public string GetUrl => $"providers/{providerId}/unapproved/add/learners/select/{learnerId}";
}

public class GetLearnerSelectedResponse
{
    public long Uln { get; set; }
    public long Ukprn { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime Dob { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int EpaoPrice { get; set; }
    public int TrainingPrice { get; set; }
    public int StandardCode { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
}
