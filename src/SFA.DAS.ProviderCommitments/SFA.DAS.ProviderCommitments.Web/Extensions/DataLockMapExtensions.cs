using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DataLockMapExtensions
    {
        public static IEnumerable<CourseDataLockViewModel> MapCourseDataLock(this GetApprenticeshipResponse apprenticeship, IList<DataLockViewModel> dataLockWithCourseMismatch, IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            if (apprenticeship.HasHadDataLockSuccess)
            {
                return Array.Empty<CourseDataLockViewModel>();
            }
          
            return dataLockWithCourseMismatch
              .Select(dataLock =>
              {
                  var previousPriceEpisode = priceEpisodes
                      .OrderByDescending(m => m.FromDate)
                      .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

                  return new CourseDataLockViewModel
                  {
                      CurrentStartDate = previousPriceEpisode?.FromDate ?? DateTime.MinValue,
                      CurrentEndDate = previousPriceEpisode?.ToDate,
                      CurrentTrainingName = apprenticeship.CourseName,
                      IlrTrainingName = dataLock.IlrTrainingCourseName,
                      IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate ?? DateTime.MinValue,
                      IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate
                  };

              }).ToList();           
        }

        public static IEnumerable<PriceDataLockViewModel> MapPriceDataLock(this IReadOnlyCollection<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes, IEnumerable<DataLock> dataLockWithOnlyPriceMismatch)
        {
            var dataLocks = dataLockWithOnlyPriceMismatch
                .OrderBy(x => x.IlrEffectiveFromDate);

            return dataLocks
               .Select(dataLock =>
               {
                   var previousPriceEpisode = priceEpisodes
                       .OrderByDescending(m => m.FromDate)
                       .FirstOrDefault(m => m.FromDate <= dataLock.IlrEffectiveFromDate);

                   if (previousPriceEpisode == null)
                   {
                       previousPriceEpisode = priceEpisodes.MaxBy(m => m.FromDate);
                   }

                   return new PriceDataLockViewModel
                    {
                        ApprenticeshipId = previousPriceEpisode.ApprenticeshipId,
                        CurrentStartDate = previousPriceEpisode?.FromDate ?? DateTime.MinValue, 
                        CurrentEndDate = previousPriceEpisode?.ToDate,
                        CurrentCost = previousPriceEpisode?.Cost ?? default(decimal),
                        IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate ?? DateTime.MinValue,
                        IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
                        IlrTotalCost = dataLock.IlrTotalCost ?? default(decimal)
                    };
                  
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
                DataLockEventDatetime = dataLock.DataLockEventDatetime,
                PriceEpisodeIdentifier = dataLock.PriceEpisodeIdentifier,
                ApprenticeshipId = dataLock.ApprenticeshipId,
                IlrTrainingCourseCode = dataLock.IlrTrainingCourseCode,                
                IlrTrainingCourseName = training.Name,
                IlrActualStartDate = dataLock.IlrActualStartDate,
                IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
                IlrPriceEffectiveToDate = dataLock.IlrPriceEffectiveToDate,
                IlrTotalCost = dataLock.IlrTotalCost,                
                DataLockErrorCode = dataLock.ErrorCode
            };
        }
    }
}
