using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.ProviderCommitments.Extensions;

public static class LearningTypeExtensions
{
    public static string ToDisplayString(this LearningType value) => value switch
    {
        LearningType.Apprenticeship => "Apprenticeship",
        LearningType.FoundationApprenticeship => "Foundation Apprenticeship",
        LearningType.ApprenticeshipUnit => "Apprenticeship Unit",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value,
           $"Unsupported {nameof(LearningType)}: {value}")
    };
}
