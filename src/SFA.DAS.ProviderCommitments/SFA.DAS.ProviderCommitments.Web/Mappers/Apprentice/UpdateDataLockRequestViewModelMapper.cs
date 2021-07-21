using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Extensions;


namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class UpdateDataLockRequestViewModelMapper : IMapper<UpdateDateLockRequest, UpdateDateLockViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public UpdateDataLockRequestViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<UpdateDateLockViewModel> Map(UpdateDateLockRequest source)
        {
            var dataLockSummariesTask = _commitmentsApiClient.GetApprenticeshipDatalockSummariesStatus(source.ApprenticeshipId);

            var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);

            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            var trainingProgrammesTask = _commitmentsApiClient.GetAllTrainingProgrammes();

            await Task.WhenAll(dataLockSummariesTask, priceEpisodesTask, apprenticeshipTask, trainingProgrammesTask);

            var dataLockSummaries = dataLockSummariesTask.Result;
            var priceEpisodes = priceEpisodesTask.Result;
            var apprenticeship = apprenticeshipTask.Result;
            var trainingProgrammes = trainingProgrammesTask.Result;            

            //var dataLockSummary = MapDataLockSummary(dataLockSummaries, trainingProgrammes);
            var dataLockSummary = dataLockSummaries.MapDataLockSummary(trainingProgrammes);

            var dataLocksPrice =  dataLockSummaries
                                  .DataLocksWithCourseMismatch
                                  .Concat(dataLockSummaries.DataLocksWithOnlyPriceMismatch)
                                  .Where(m => m.ErrorCode.HasFlag(DataLockErrorCode.Dlock07));
            
            return new UpdateDateLockViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,               
                ApprenticeshipId = source.ApprenticeshipId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN = apprenticeship.Uln,
                DateOfBirth = apprenticeship.DateOfBirth,
                CourseName = apprenticeship.CourseName,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,              
                CourseDataLocks = apprenticeship.MapCourseDataLock(dataLockSummary.DataLockWithCourseMismatch, priceEpisodes.PriceEpisodes),  //MapCourseDataLock(apprenticeship, dataLockSummary.DataLockWithCourseMismatch, priceEpisodes.PriceEpisodes),
                PriceDataLocks = priceEpisodes.PriceEpisodes.MapPriceDataLock(dataLocksPrice)    //MapPriceDataLock(priceEpisodes.PriceEpisodes, dataLocksPrice)
            };
        }


        /* MOVED TO EXTENSION
       private IEnumerable<CourseDataLockViewModel> MapCourseDataLock(GetApprenticeshipResponse apprenticeship, IList<DataLockViewModel> dataLockWithCourseMismatch, IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
       {
           if (apprenticeship.HasHadDataLockSuccess) return new CourseDataLockViewModel[0];

           var result = new List<CourseDataLockViewModel>();

           return dataLockWithCourseMismatch
             .Select(dataLock =>
             {
                 var previousPriceEpisode = priceEpisodes
                     .OrderByDescending(m => m.FromDate)
                     .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

                 return new CourseDataLockViewModel
                 {
                     FromDate = previousPriceEpisode?.FromDate ?? DateTime.MinValue, //CurrentStartDate
                     ToDate = previousPriceEpisode?.ToDate, //CurrentEndDate
                     TrainingName = apprenticeship.CourseName,
                     ApprenticeshipStartDate = apprenticeship.StartDate,
                     IlrTrainingName = dataLock.IlrTrainingCourseName,
                     IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate ?? DateTime.MinValue,
                     IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate
                 };
             }).ToList();


           //foreach (var datalock in dataLockWithCourseMismatch)
           //{
           //    var priceHistory = priceEpisodes
           //        .OrderByDescending(x => x.FromDate)
           //        .First(x => x.FromDate <= datalock.IlrEffectiveFromDate.Value);

           //    result.Add(new CourseDataLockViewModel
           //    {   
           //        FromDate = priceHistory.FromDate,
           //        ToDate = priceHistory.ToDate,
           //        TrainingName = apprenticeship.CourseName,
           //        ApprenticeshipStartDate = apprenticeship.StartDate,
           //        IlrTrainingName = datalock.IlrTrainingCourseName,
           //        IlrEffectiveFromDate = datalock.IlrEffectiveFromDate,
           //        IlrEffectiveToDate = datalock.IlrEffectiveToDate
           //    });
           //}

           //return result;
       }



      private IEnumerable<PriceHistoryViewModel> MapPriceDataLock(IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes, IEnumerable<DataLock> dataLockWithOnlyPriceMismatch)
      {
          var dataLocks = dataLockWithOnlyPriceMismatch
              .OrderBy(x => x.IlrEffectiveFromDate);

          return dataLocks
             .Select(dataLock =>
             {
                 var previousPriceEpisode = priceEpisodes
                     .OrderByDescending(m => m.FromDate)
                     .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

                 return new PriceHistoryViewModel
                 {
                     ApprenticeshipId = previousPriceEpisode.ApprenticeshipId,
                     FromDate = previousPriceEpisode?.FromDate ?? DateTime.MinValue, //CurrentStartDate
                     ToDate = previousPriceEpisode?.ToDate, //CurrentEndDate
                     Cost = previousPriceEpisode?.Cost ?? default(decimal), //CurrentCost
                     IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate ?? DateTime.MinValue,
                     IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
                     IlrTotalCost = dataLock.IlrTotalCost ?? default(decimal)
                 };
             }).ToList();
      }




      private UpdateDateLockSummaryViewModel MapDataLockSummary(GetDataLockSummariesResponse dataLockSummaries, GetAllTrainingProgrammesResponse trainingProgrammes)
      {
          var result = new UpdateDateLockSummaryViewModel
          {
              DataLockWithCourseMismatch = new List<DataLockViewModel>(),               
          };

          foreach (var dataLock in dataLockSummaries.DataLocksWithCourseMismatch)
          {
              var training = trainingProgrammes.TrainingProgrammes.SingleOrDefault(x => x.CourseCode == dataLock.IlrTrainingCourseCode);
              if (training == null)
              {
                  throw new InvalidOperationException(
                      $"Datalock {dataLock.Id} IlrTrainingCourseCode {dataLock.IlrTrainingCourseCode} not found; possible expiry");
              }
              result.DataLockWithCourseMismatch.Add(MapDataLockStatus(dataLock, training));
          }

          return result;
      }

      private DataLockViewModel MapDataLockStatus(DataLock dataLock, TrainingProgramme training)
      {
          return new DataLockViewModel
          {
              //DataLockEventId = dataLock.DataLockEventId,
              DataLockEventDatetime = dataLock.DataLockEventDatetime,
              PriceEpisodeIdentifier = dataLock.PriceEpisodeIdentifier,
              ApprenticeshipId = dataLock.ApprenticeshipId,
              IlrTrainingCourseCode = dataLock.IlrTrainingCourseCode,
              //IlrTrainingType = (TrainingType)dataLock.IlrTrainingType,
              IlrTrainingCourseName = training.Name,
              IlrActualStartDate = dataLock.IlrActualStartDate,
              IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
              IlrPriceEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
              IlrTotalCost = dataLock.IlrTotalCost,
              TriageStatusViewModel = (TriageStatusViewModel)dataLock.TriageStatus,
              DataLockErrorCode = dataLock.ErrorCode
          };
      }       */
    }
}
