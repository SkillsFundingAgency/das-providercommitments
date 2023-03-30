using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Interfaces;
using static SFA.DAS.ProviderCommitments.Web.Services.RplOpenAiService;

namespace SFA.DAS.ProviderCommitments.Web.Services
{
    public interface IRplOpenAiService
    {
        Task<CourseResponse> GetRplCourseForApprenticeship(string courseName);
    }

    public class RplOpenAiService : IRplOpenAiService
    {
        private readonly IOpenAiHttpClient _client;
        private readonly OpenAiConfiguration _configuration;

        public RplOpenAiService(IOpenAiHttpClient client, OpenAiConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<CourseResponse> GetRplCourseForApprenticeship(string courseName)
        {
            var list = await GetListOfPreviousCourses(courseName);
            return await ConvertToListOfCourses(list);
        }

        public async Task<string> GetListOfPreviousCourses(string description)
        {
            var prompt =
                $"As an expert on the following apprenticeship '{description}' list the top 10 Recognized Prior Learning (RPL) qualifications, which would reduce the time the apprentice needs to qualify for this apprenticeship. Simply return this as a list and add no additional comments";

            var request = BuildPrompt(prompt);

            var response = await _client.PostAsJson(OpenAiEndpoint, request, CancellationToken.None);
            var openAiReponse = JsonConvert.DeserializeObject<OpenAiResponse>(response);

            return openAiReponse?.Choices?.FirstOrDefault()?.Text;
        }

        public async Task<CourseResponse> ConvertToListOfCourses(string list)
        {
            var prompt = "Convert the following to a JSON list of courses in the format { \"courses\": [\"Course A\", \"Course B\",...] } \r\n \"\" \r\n " + list + " \r\n \"\" \r\n";

            var request = BuildPrompt(prompt);

            var response = await _client.PostAsJson(OpenAiEndpoint, request, CancellationToken.None);
            var openAiReponse = JsonConvert.DeserializeObject<OpenAiResponse>(response);
            var json = openAiReponse?.Choices?.FirstOrDefault()?.Text;
            return JsonConvert.DeserializeObject<CourseResponse>(json);
        }




        /*
                public Task<AddDraftApprenticeshipResponse> AddDraftApprenticeship(
                    long cohortId,
                    AddDraftApprenticeshipRequest request,
                    CancellationToken cancellationToken = default(CancellationToken))
                {
                    return this._client.PostAsJson<AddDraftApprenticeshipRequest, AddDraftApprenticeshipResponse>(
                        string.Format("api/cohorts/{0}/draft-apprenticeships", (object)cohortId), request, cancellationToken);
                }
        */

        private string OpenAiEndpoint => $"completions?api-version={_configuration.ApiVersion}";

        private OpenAiRequest BuildPrompt(string prompt)
        {
            return new OpenAiRequest
            {
                prompt = prompt,
                max_tokens = 1000,
                temperature = (decimal) 0.5,
                frequency_penalty = 0,
                presence_penalty = 0,
                //best_of = 1,
                top_p = (decimal) 0.5,
                stop = null
            };
        }


        public class OpenAiRequest
        {
            public string prompt { get; set; }
            public int max_tokens { get; set; }
            public decimal temperature { get; set; }
            public int frequency_penalty { get; set; }
            public int presence_penalty { get; set; }
            //public int best_of { get; set; }
            public decimal top_p { get; set; }
            public string stop { get; set; }

        }

        public class OpenAiResponse
        {
            public OpenAiChoice[] Choices;

        }

        public class OpenAiChoice
        {
            public string Text { get; set; }
        }

        public class CourseResponse
        {
            public string[] Courses { get; set; }
            public bool HasCourses => Courses != null && Courses.Length > 0;
        }


        /*
        {
      "prompt": "How many hours do I have to work in UK at most 2020?\n\nYou can work up to 20 hours per week. Working more than 20 hours per week is a breach of your student visa conditions, and could result in your visa being cancelled.\n\nCan you work full time on a Tier 4 visa?\n\nIf you are a Tier 4 student visa holder, you are allowed to work part-time for up to 20 hours per week during term-time and full-time during vacations. However, you must not engage in business, self-employment,",
      "max_tokens": 100,
      "temperature": 0.5,
      "frequency_penalty": 0,
      "presence_penalty": 0,
      "top_p": 0.5,
      "stop": null
    }
    
        */
    }
}
