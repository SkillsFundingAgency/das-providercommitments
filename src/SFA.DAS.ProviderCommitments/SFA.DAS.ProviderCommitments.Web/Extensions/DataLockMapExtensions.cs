using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DataLockMapExtensions
    {
        public static IEnumerable<CourseDataLockViewModel> MapCourseDataLock(this GetApprenticeshipResponse apprenticeship, IList<DataLockViewModel> dataLockWithCourseMismatch, IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            if (apprenticeship.HasHadDataLockSuccess) return new CourseDataLockViewModel[0];

            var result = new List<CourseDataLockViewModel>();

            //return dataLockWithCourseMismatch
            //  .Select(dataLock =>
            //  {
            //      var previousPriceEpisode = priceEpisodes
            //          .OrderByDescending(m => m.FromDate)
            //          .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

            //      return new CourseDataLockViewModel
            //      {
            //          FromDate = previousPriceEpisode?.FromDate ?? DateTime.MinValue, //CurrentStartDate
            //          ToDate = previousPriceEpisode?.ToDate, //CurrentEndDate
            //          TrainingName = apprenticeship.CourseName,
            //          ApprenticeshipStartDate = apprenticeship.StartDate,
            //          IlrTrainingName = dataLock.IlrTrainingCourseName,
            //          IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate ?? DateTime.MinValue,
            //          IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate
            //      };
            //  }).ToList();


            foreach (var datalock in dataLockWithCourseMismatch)
            {
                var priceHistory = priceEpisodes
                    .OrderByDescending(x => x.FromDate)
                    .FirstOrDefault(x => x.FromDate <= datalock.IlrEffectiveFromDate.Value);

                result.Add(new CourseDataLockViewModel
                {
                    FromDate = priceHistory?.FromDate ?? DateTime.MinValue,
                    ToDate = priceHistory?.ToDate,
                    TrainingName = apprenticeship.CourseName,
                    ApprenticeshipStartDate = apprenticeship.StartDate,
                    IlrTrainingName = datalock.IlrTrainingCourseName,
                    IlrEffectiveFromDate = datalock.IlrEffectiveFromDate,
                    IlrEffectiveToDate = datalock.IlrPriceEffectiveToDate
                });
            }

            return result;
        }

        public static IEnumerable<PriceHistoryViewModel> MapPriceDataLock(this IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes, IEnumerable<DataLock> dataLockWithOnlyPriceMismatch)
        {
            var dataLocks = dataLockWithOnlyPriceMismatch
                .OrderBy(x => x.IlrEffectiveFromDate);

            return dataLocks
               .Select(dataLock =>
               {
                   var previousPriceEpisode = priceEpisodes
                       .OrderByDescending(m => m.FromDate)
                       .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

                   //if (previousPriceEpisode != null)
                   //{
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
                   //}
                   //else
                   //{
                   //    return new PriceHistoryViewModel
                   //    {
                   //        IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate ?? DateTime.MinValue,
                   //        IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
                   //        IlrTotalCost = dataLock.IlrTotalCost ?? default(decimal)
                   //    };
                   //}
               }).ToList();
        }


        public static UpdateDateLockSummaryViewModel MapDataLockSummary(this GetDataLockSummariesResponse dataLockSummaries, GetAllTrainingProgrammesResponse trainingProgrammes)
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

        private static DataLockViewModel MapDataLockStatus(DataLock dataLock, TrainingProgramme training)
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
        }
    }
}
