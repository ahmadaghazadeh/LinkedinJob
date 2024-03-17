using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace LinkedinScraping.Controllers
{
	public class Service: IHostedService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		private string path = "https://api.scrapingdog.com/linkedinjobs/?api_key=65f69b4159b3923d7ddeded1";
		private string jobPath = "https://api.scrapingdog.com/linkedinjobs?api_key=65f69b4159b3923d7ddeded1";
		List<string> Fields = new List<string>();
		List<string> geoids = new List<string>();
		int pageFetch = 2;

		private readonly IConfiguration configuration;

		private readonly IServiceProvider _serviceProvider;
		public Service(
			IConfiguration configuration,
			IHttpClientFactory httpClientFactory,
			IServiceProvider serviceProvider)
		{
			 
			_serviceProvider = serviceProvider;
			Fields = configuration.GetSection("Fileds").Get<List<string>>();
			geoids = configuration.GetSection("geoids").Get<List<string>>();
			pageFetch = configuration.GetSection("pageFetch").Get<int>();
			_httpClientFactory = httpClientFactory;
		}
		public Task StartAsync(CancellationToken cancellationToken)
		{

			DoWorkAsync(cancellationToken).ConfigureAwait(false);
			return Task.CompletedTask;
		}

		private async Task DoWorkAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await PublishEvent();
				await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
			}

		}

		private async Task PublishEvent()
		{
			try
			{
				using (var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<JobContext>())
				{
					foreach (var geoid in geoids)
					{
						foreach (var field in Fields)
						{
							for (int page = 1; page < pageFetch; page++)
							{
								try
								{
									var tempJobs = await TempJobs(field, geoid, page);
									if (tempJobs.Any())
									{
										dbContext.Jobs.AddOrUpdateRange(tempJobs);
										await dbContext.SaveChangesAsync();
									}
								}
								catch (Exception e)
								{
									Console.WriteLine(e);
									throw;
								}
							}
						}
					}

				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
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
					try
					{
						var jobNew = await getJobDescriptionTask(job);
						jobs.Add(jobNew);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}

				}
			}
			return jobs;
		}

		private async Task<Job> getJobDescriptionTask(Job job)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync($"{jobPath}&job_id={job.JobId}");
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStreamAsync();
				var jobInfo = await JsonSerializer.DeserializeAsync<List<JobInfo>>(content);
				if (jobInfo[0].JobDescription.Contains("relocat"))
				{
					job.IsRelocation = true;
				}
				if (jobInfo[0].JobDescription.Contains("remote"))
				{
					job.IsRemote = true;
				}

				job.JobDescriptiob = jobInfo[0].JobDescription;
			}

			return job;
		}
	}
}
