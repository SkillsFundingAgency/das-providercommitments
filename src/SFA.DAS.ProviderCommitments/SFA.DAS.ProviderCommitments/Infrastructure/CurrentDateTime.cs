using System;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        private readonly DateTime? _time;

        public DateTime Now => _time ?? DateTime.UtcNow;

        public CurrentDateTime()
        {
        }

        public CurrentDateTime(DateTime? time)
        {
            _time = time;
        }
    }
}
