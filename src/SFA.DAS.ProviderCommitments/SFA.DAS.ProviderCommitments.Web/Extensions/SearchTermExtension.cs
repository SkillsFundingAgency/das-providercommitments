using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class FiltersMessageExtension
    {
        public static HtmlString GetFiltersUsedMessage(this ManageApprenticesFilterModel model)
        {
            var filters = new List<string>();
            if (!string.IsNullOrWhiteSpace(model.SearchTerm)) filters.Add($"‘{model.SearchTerm}’");
            if (!string.IsNullOrWhiteSpace(model.SelectedEmployer)) filters.Add(model.SelectedEmployer);
            if (!string.IsNullOrWhiteSpace(model.SelectedCourse)) filters.Add(model.SelectedCourse);
            if (model.SelectedStatus.HasValue) filters.Add(model.SelectedStatus.Value.GetDescription());
            if (model.SelectedStartDate.HasValue) filters.Add(model.SelectedStartDate.Value.ToGdsFormatWithoutDay());
            if (model.SelectedEndDate.HasValue) filters.Add(model.SelectedEndDate.Value.ToGdsFormatWithoutDay());

            if (filters.Count == 0) return HtmlString.Empty;

            var message = new StringBuilder();

            message.Append($"matching <strong>{filters[0]}</strong>");

            for (var i = 1; i < filters.Count; i++)
            {
                if (i == filters.Count - 1)
                {
                    message.Append(" and ");
                }
                else
                {
                    message.Append(", ");
                }

                message.Append($"<strong>{filters[i]}</strong>");
            }

            return new HtmlString(message.ToString());
        }
    }
}
