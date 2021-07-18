using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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

            var dataLockSummary = MapDataLockSummary(dataLockSummaries, trainingProgrammes);

            var dataLocksPrice =  dataLockSummaries
                                  .DataLocksWithCourseMismatch
                                  .Concat(dataLockSummaries.DataLocksWithOnlyPriceMismatch)
                                  .Where(m => m.ErrorCode.HasFlag(DataLockErrorCode.Dlock07));          


            //var dataLock = dataLockSummaries
            //    .DataLocksWithCourseMismatch
            //    .Where(x => x.TriageStatus == TriageStatus.Unknown)
            //    .OrderBy(x => x.IlrEffectiveFromDate)
            //    .FirstOrDefault();

            return new UpdateDateLockViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,               
                ApprenticeshipId = source.ApprenticeshipId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN = apprenticeship.Uln,
                CourseName = apprenticeship.CourseName,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,              
                //IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
                //IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
                CourseDataLocks = MapCourseDataLock(apprenticeship, dataLockSummary.DataLockWithCourseMismatch, priceEpisodes.PriceEpisodes),
                PriceDataLocks = MapPriceDataLock(priceEpisodes.PriceEpisodes, dataLocksPrice)
            };
        }

        private IEnumerable<CourseDataLockViewModel> MapCourseDataLock(GetApprenticeshipResponse apprenticeship, IList<DataLockViewModel> dataLockWithCourseMismatch, IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            if (apprenticeship.HasHadDataLockSuccess)
                return new CourseDataLockViewModel[0];

            var priceHistorViewModels = priceEpisodes
               .Select(history => new PriceHistoryViewModel
               {
                   ApprenticeshipId = history.ApprenticeshipId,
                   Cost = history.Cost,
                   FromDate = history.FromDate,
                   ToDate = history.ToDate
               });

            var result = new List<CourseDataLockViewModel>();

            foreach (var datalock in dataLockWithCourseMismatch)
            {
                var s = priceHistorViewModels
                    .OrderByDescending(x => x.FromDate)
                    .First(x => x.FromDate <= datalock.IlrEffectiveFromDate.Value);

                result.Add(new CourseDataLockViewModel
                {
                    FromDate = s.FromDate,
                    ToDate = s.ToDate,
                    TrainingName = apprenticeship.CourseName,
                    ApprenticeshipStartDate = apprenticeship.StartDate,
                    IlrTrainingName = datalock.IlrTrainingCourseName,
                    IlrEffectiveFromDate = datalock.IlrEffectiveFromDate,
                    IlrEffectiveToDate = datalock.IlrEffectiveToDate
                });
            }

            return result;
        }

        private IEnumerable<PriceHistoryViewModel> MapPriceDataLock(IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes, IEnumerable<DataLock> dataLockWithOnlyPriceMismatch)
        {
            var priceHistoryViewModels = priceEpisodes
              .Select(history => new PriceHistoryViewModel
              {
                  ApprenticeshipId = history.ApprenticeshipId,
                  Cost = history.Cost,
                  FromDate = history.FromDate,
                  ToDate = history.ToDate
              });

            var dataLocks = dataLockWithOnlyPriceMismatch
                .OrderBy(x => x.IlrEffectiveFromDate);

            //return datalocks.Select(
            //    datalock =>
            //    {
            //        var s = priceHistoryViewModels
            //            .OrderByDescending(x => x.FromDate)
            //            .First(x => x.FromDate <= datalock.IlrEffectiveFromDate.Value);
            //        s.IlrEffectiveFromDate = datalock.IlrEffectiveFromDate;
            //        s.IlrEffectiveToDate = datalock.IlrPriceEffectiveToDate;
            //        s.IlrTotalCost = datalock.IlrTotalCost;
            //        return s;
            //    }
            //);


            return dataLocks
               .Select(dataLock =>
               {
                   var previousPriceEpisode = priceEpisodes
                       .OrderByDescending(m => m.FromDate)
                       .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

                   return new PriceHistoryViewModel
                   {
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
                DataLockWithOnlyPriceMismatch = new List<DataLockViewModel>()
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


            foreach (var dataLock in dataLockSummaries.DataLocksWithOnlyPriceMismatch)
            {
                var training = trainingProgrammes.TrainingProgrammes.SingleOrDefault(x => x.CourseCode == dataLock.IlrTrainingCourseCode);
                if (training == null)
                {
                    throw new InvalidOperationException(
                        $"Datalock {dataLock.Id} IlrTrainingCourseCode {dataLock.IlrTrainingCourseCode} not found; possible expiry");
                }
                result.DataLockWithOnlyPriceMismatch.Add(MapDataLockStatus(dataLock, training));
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
                IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
                IlrTotalCost = dataLock.IlrTotalCost,
                TriageStatusViewModel = (TriageStatusViewModel)dataLock.TriageStatus,
                DataLockErrorCode = dataLock.ErrorCode
            };
        }

        //private DataLockViewModel MapCourseDataLockStatus(DataLock dataLock, TrainingProgramme training)
        //{
        //    return new CourseDataLockViewModel
        //    {
        //       IlrTrainingName = dataLock.IlrTrainingCourseCode,
        //        IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
        //       IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
        //       FromDate = (DateTime)training.EffectiveFrom,
        //       ToDate = training.EffectiveTo,
        //       TrainingName = training.Name               
        //    };
        //}

        //private DataLockViewModel MapPriceDataLockStatus(DataLock dataLock, TrainingProgramme training)
        //{
        //    return new PriceHistoryViewModel
        //    {
        //        IlrTotalCost = dataLock.IlrTotalCost,
        //        IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
        //        IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
        //        FromDate = (DateTime)training.EffectiveFrom,
        //        ToDate = training.EffectiveTo                
        //    };
        //}



    }
}
