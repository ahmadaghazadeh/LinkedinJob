using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LinkedinScraping.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class JobController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		private string path = "https://api.scrapingdog.com/linkedinjobs/?api_key=65f5604effbbac3174b4d31b";
		private JobContext _dbContext;
		List<string> Fields=new List<string>();
		List<string> geoids = new List<string>();
		int pageFetch =2;

		public JobController(IConfiguration configuration, JobContext dbContext, IHttpClientFactory httpClientFactory)
		{
			_dbContext = dbContext;

			Fields = configuration.GetSection("Fileds").Get<List<string>>();
			geoids = configuration.GetSection("geoids").Get<List<string>>();
			pageFetch = configuration.GetSection("pageFetch").Get<int>();
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public async Task<List<Job>> GetAllContacts()
		{
			

			var jobs=new List<Job>();

			Parallel.ForEach(geoids, geoid =>
			{
				Parallel.ForEach(Fields, field =>
				{
					Parallel.For(1, pageFetch, async page =>
					{
						var tempJobs = await TempJobs(field, geoid, page);
						jobs.AddRange(tempJobs);
					});
				});

			});
 
			return jobs;
		}

		private async Task<List<Job>?> TempJobs(string field, string geoid, int page)
		{
			var jobs = new List<Job>();
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync($"{path}&field={field}&geoid={geoid}&page={page}");
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStreamAsync();
				var tempJobs = await JsonSerializer.DeserializeAsync<List<Job>>(content);
				foreach (var job in tempJobs)
				{
					var jobNew =await getJobDescriptionTask(job);
					jobs.Add(jobNew);
				}
			}
			return jobs;
		}

		private async Task<Job> getJobDescriptionTask(Job job)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var request = new HttpRequestMessage(HttpMethod.Get, job.JobLink);
			request.Headers.Add("Cookie", "bcookie=\"v=2&5189c6ca-992c-4ec6-8693-eaa083a369b3\"; lang=v=2&lang=en-us; li_gc=MTswOzE3MTA1Nzk2MzI7MjswMjEIt+qNlqOr1l4nDib3XM5DNgMWyhInR8l5mk4KWqwEdQ==; lidc=\"b=VGST07:s=V:r=V:a=V:p=V:g=2888:u=1:x=1:i=1710594731:t=1710681131:v=2:sig=AQH83d0dCgjmKiCzDYjNS-eMDXv6Z1NF\"; JSESSIONID=ajax:4908604551682048916; bscookie=\"v=1&20240316131210de789e08-5611-4431-8623-63f2416c8255AQH9C284r0ekK0Ndc0GM2r5HkKy1w3LE\"");
			var response = await httpClient.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStreamAsync();
				var jobDesc = await JsonSerializer.DeserializeAsync<string>(content);
				if (jobDesc.Contains("relocat"))
				{
					job.IsRelocation = true;
				}
				if (jobDesc.Contains("remote"))
				{
					job.IsRemote = true;
				}
			}
			
			return job;
		}
	}
}
