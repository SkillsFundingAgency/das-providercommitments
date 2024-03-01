using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AttachApimUserInfoToSaveRequests<TFrom, TTo> : IMapper<TFrom, TTo> 
        where TFrom : class
        where TTo : class
    {
        private readonly IMapper<TFrom, TTo> _innerMapper;
        private readonly IAuthenticationService _authenticationService;

        public AttachApimUserInfoToSaveRequests(IMapper<TFrom, TTo> innerMapper, IAuthenticationService authenticationService)
        {
            _innerMapper = innerMapper;
            _authenticationService = authenticationService;
        }

        public async Task<TTo> Map(TFrom source)
        {
            var to = await _innerMapper.Map(source);

            if (to is ApimSaveDataRequest saveDataRequest)
            {
                saveDataRequest.UserInfo = GetUserInfo();
            }
            return to;
        }

        protected ApimUserInfo GetUserInfo()
        {
            if (_authenticationService.IsUserAuthenticated())
            {
                return new ApimUserInfo
                {
                    UserId = _authenticationService.UserId,
                    UserDisplayName = _authenticationService.UserName,
                    UserEmail = _authenticationService.UserEmail
                };
            }

            return null;
        }
    }
}
