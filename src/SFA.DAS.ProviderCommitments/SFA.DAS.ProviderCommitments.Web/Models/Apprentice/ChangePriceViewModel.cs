using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class ChangePriceViewModel : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string ApprenticeshipHashedId { get; set; }
    public string ApprenticeName { get; set; }
    public string Employer { get; set; }
    public string Reference { get; set; }
    public int? FundingBandCap { get; set; }

    [Range(1, 100000, ErrorMessage = "The training price must be a whole number between 1 - 100,000")]
    public int? TrainingPrice { get; set; }
    [Range(1, 100000, ErrorMessage = "The end-point assessment price must be a whole number between 1 - 100,000")]
    public int? EndpointAssessmentPrice { get; set; }

    public int TotalPrice => TrainingPrice.GetValueOrDefault() + EndpointAssessmentPrice.GetValueOrDefault();

    public int OriginalTrainingPrice { get; set; }
    public int OriginalEndpointAssessmentPrice { get; set; }
    public bool PricesChanged => TrainingPrice != OriginalTrainingPrice || EndpointAssessmentPrice != OriginalEndpointAssessmentPrice;
}