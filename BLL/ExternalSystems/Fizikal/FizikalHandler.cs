using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ExternalSystems.Fizikal
{
    public class FizikalHandler : IFizikalHandler
    {
        private readonly HttpClient _httpClient;
        const string _url = "https://fizikalcoredev.incloud.co.il/external/v1/classes";

        public FizikalHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetClassScheduleAsync()
        {
            string requestUrl = $"{_url}/schedule/guestView?OrganizationId=1&CompanyId=2&BranchId=7&FromDate=2025-02-02&ToDate=2025-02-07&&AppTypeId=0";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "frumi2023!");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            var res  = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching schedule: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetClassCategories()
        {
            string requestUrl = $"{_url}/groups?OrganizationId=1&CompanyId=2&BranchId=7";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "frumi2023!");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            var res = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching schedule: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }

}
