using System.Text.Json.Serialization;

public class JobInfo
{
	[JsonPropertyName("job_position")]
	public string JobPosition { get; set; }

	[JsonPropertyName("job_location")]
	public string JobLocation { get; set; }

	[JsonPropertyName("company_name")]
	public string CompanyName { get; set; }

	[JsonPropertyName("company_linkedin_id")]
	public Uri CompanyLinkedinId { get; set; }

	[JsonPropertyName("job_posting_time")]
	public string JobPostingTime { get; set; }

	[JsonPropertyName("base_pay")]
	public string BasePay { get; set; }

	[JsonPropertyName("job_description")]
	public string JobDescription { get; set; }

	[JsonPropertyName("Seniority_level")]
	public string SeniorityLevel { get; set; }

	[JsonPropertyName("Employment_type")]
	public string EmploymentType { get; set; }

	[JsonPropertyName("Job_function")]
	public string JobFunction { get; set; }

	[JsonPropertyName("Industries")]
	public string Industries { get; set; }

}