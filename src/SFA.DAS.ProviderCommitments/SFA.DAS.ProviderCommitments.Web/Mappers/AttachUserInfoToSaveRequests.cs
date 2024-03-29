﻿using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AttachUserInfoToSaveRequests<TFrom, TTo> : IMapper<TFrom, TTo> 
        where TFrom : class
        where TTo : class
    {
        private readonly IMapper<TFrom, TTo> _innerMapper;
        private readonly IAuthenticationService _authenticationService;

        public AttachUserInfoToSaveRequests(IMapper<TFrom, TTo> innerMapper, IAuthenticationService authenticationService)
        {
            _innerMapper = innerMapper;
            _authenticationService = authenticationService;
        }

        public async Task<TTo> Map(TFrom source)
        {
            var to = await _innerMapper.Map(source);

            if (to is SaveDataRequest saveDataRequest)
            {
                saveDataRequest.UserInfo = GetUserInfo();
            }
            return to;
        }

        protected UserInfo GetUserInfo()
        {
            if (_authenticationService.IsUserAuthenticated())
            {
                return new UserInfo
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
