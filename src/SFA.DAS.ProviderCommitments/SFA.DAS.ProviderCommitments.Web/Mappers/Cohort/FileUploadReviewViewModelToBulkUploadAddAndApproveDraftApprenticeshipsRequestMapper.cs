﻿//using SFA.DAS.CommitmentsV2.Api.Types.Requests;
//using SFA.DAS.CommitmentsV2.Shared.Interfaces;
//using SFA.DAS.Encoding;
//using SFA.DAS.ProviderCommitments.Interfaces;
//using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
//{
//    public class FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper : FileUploadMapperBase, IMapper<FileUploadReviewViewModel, BulkUploadAddAndApproveDraftApprenticeshipsRequest>
//    {
//        private readonly ICacheService _cacheService;

//        public FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper(ICacheService cacheService, IEncodingService encodingService)
//            :base(encodingService)
//        {
//            _cacheService = cacheService;
//        }

//        public async Task<BulkUploadAddAndApproveDraftApprenticeshipsRequest> Map(FileUploadReviewViewModel source)
//        {
//            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
//            await _cacheService.ClearCache(source.CacheRequestId.ToString());
//            return new BulkUploadAddAndApproveDraftApprenticeshipsRequest
//            {
//                ProviderId = source.ProviderId,
//                BulkUploadAddAndApproveDraftApprenticeships = ConvertToBulkUploadApiRequest(csvRecords, source.ProviderId)
//            };
//        }
//    }
//}
