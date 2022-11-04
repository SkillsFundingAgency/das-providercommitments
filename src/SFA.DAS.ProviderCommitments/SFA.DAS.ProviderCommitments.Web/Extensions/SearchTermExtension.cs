using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Html;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class FiltersMessageExtension
    {
        public static HtmlString GetFiltersUsedMessage(this ApprenticesFilterModel model)
        {
            var filters = BuildUsedFilterList(model);

            if (filters.Count == 0)
            {
                return HtmlString.Empty;
            }

            var message = new StringBuilder();

            message.Append($"matching <strong>{filters[0]}</strong>");

            for (var i = 1; i < filters.Count; i++)
            {
                message.Append(i == filters.Count - 1 ? " and " : ", ");

                message.Append($"<strong>{filters[i]}</strong>");
            }

            return new HtmlString(message.ToString());
        }

        private static IList<string> BuildUsedFilterList(ApprenticesFilterModel model)
        {
            var filters = new List<string>();
            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                filters.Add($"‘{WebUtility.HtmlEncode(model.SearchTerm)}’");
            }

            if (!string.IsNullOrWhiteSpace(model.SelectedEmployer))
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedEmployer));
            }

            if (!string.IsNullOrWhiteSpace(model.SelectedCourse))
            { 
                filters.Add(WebUtility.HtmlEncode(model.SelectedCourse));
            }

            if (model.SelectedStatus.HasValue)
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedStatus.Value.GetDescription()));
            }

            if (model.SelectedStartDate.HasValue)
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedStartDate.Value.ToGdsFormatWithoutDay()));
            }

            if (model.SelectedEndDate.HasValue)
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedEndDate.Value.ToGdsFormatWithoutDay()));
            }

            if (model.SelectedAlert.HasValue)
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedAlert.Value.GetDescription()));
            }

            if (model.SelectedApprenticeConfirmation.HasValue)
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedApprenticeConfirmation.Value.GetDescription()));
            }

            if (model.SelectedDeliveryModel.HasValue)
            {
                filters.Add(WebUtility.HtmlEncode(model.SelectedDeliveryModel.Value.ToDescription()));
            }

            return filters;
        }
    }
}
