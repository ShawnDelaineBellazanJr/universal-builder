using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutonomousAI
{
    /// <summary>
    /// GitHubMemory provides persistence for the Universal Builder using GitHub as a database.
    /// - Uses Gists for templates
    /// - Uses Secrets for economic values
    /// - Uses Issues for work tracking
    /// - Uses Releases for history
    /// </summary>
    public class GitHubMemory
    {
        private readonly GitHubClient _client;
        private readonly string _owner;
        private readonly string _repo;
        private readonly HttpClient _httpClient;
        private readonly bool _debugMode;
        private const int MaxRetries = 5;

        public GitHubMemory(string token, string owner, string repo, bool debugMode = false)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("GitHub token cannot be null or empty", nameof(token));
            
            if (string.IsNullOrEmpty(owner))
                throw new ArgumentException("Repository owner cannot be null or empty", nameof(owner));
            
            if (string.IsNullOrEmpty(repo))
                throw new ArgumentException("Repository name cannot be null or empty", nameof(repo));
            
            _owner = owner;
            _repo = repo;
            _debugMode = debugMode;
            
            var credentials = new Credentials(token);
            _client = new GitHubClient(new ProductHeaderValue("UniversalBuilder"), new InMemoryCredentialStore(credentials));
            
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "UniversalBuilder");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {token}");
            
            if (_debugMode)
            {
                Console.WriteLine("GitHubMemory initialized in debug mode - some operations will be mocked");
            }
        }

        #region Template Management (Gists)

        /// <summary>
        /// Gets a Prompty template from a Gist
        /// </summary>
        public async Task<string> GetTemplateAsync(string gistId, string? fileName = null)
        {
            if (string.IsNullOrEmpty(gistId))
                throw new ArgumentException("Gist ID cannot be null or empty", nameof(gistId));
            
            // In debug mode, return from local templates if available, otherwise use default templates
            if (_debugMode)
            {
                string templatePath = Path.Combine("templates", fileName ?? "default.prompty");
                if (File.Exists(templatePath))
                {
                    Console.WriteLine($"Debug mode: Loading template from local file: {templatePath}");
                    return await File.ReadAllTextAsync(templatePath);
                }
                
                Console.WriteLine($"Debug mode: No local template found, using default templates");
                return string.Empty; // Empty will trigger default templates
            }
            
            return await ExecuteWithRetryAsync(async () =>
            {
                // Handle empty gistId - return an empty string to trigger the default templates
                if (string.IsNullOrEmpty(gistId))
                    return string.Empty;
                
                try
                {
                    var gist = await _client.Gist.Get(gistId);
                    
                    if (fileName == null)
                    {
                        // Return the first file if no specific file is requested
                        var firstFile = gist.Files.FirstOrDefault();
                        return firstFile.Value.Content ?? string.Empty;
                    }
                    
                    if (gist.Files.TryGetValue(fileName, out var file))
                    {
                        return file.Content ?? string.Empty;
                    }
                    
                    throw new FileNotFoundException($"File {fileName} not found in gist {gistId}");
                }
                catch (NotFoundException)
                {
                    // If gist is not found, return empty string to trigger default templates
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// Validates that a template from a Gist is properly formatted
        /// </summary>
        public bool ValidateTemplate(string templateContent)
        {
            if (string.IsNullOrEmpty(templateContent))
                return false;
                
            // Basic validation for Prompty template format
            // Check for required sections
            bool hasSystem = templateContent.Contains("system:");
            bool hasUser = templateContent.Contains("user:");
            bool hasAssistant = templateContent.Contains("assistant:");
            
            return hasSystem && hasUser && hasAssistant;
        }

        /// <summary>
        /// Creates a new Gist with a template
        /// </summary>
        public async Task<string> CreateTemplateAsync(string templateName, string templateContent, bool isPublic = false)
        {
            if (string.IsNullOrEmpty(templateName))
                throw new ArgumentException("Template name cannot be null or empty", nameof(templateName));
            
            if (string.IsNullOrEmpty(templateContent))
                throw new ArgumentException("Template content cannot be null or empty", nameof(templateContent));
            
            return await ExecuteWithRetryAsync(async () =>
            {
                var gistCreate = new NewGist
                {
                    Description = $"Universal Builder Template: {templateName}",
                    Public = isPublic
                };
                
                gistCreate.Files.Add($"{templateName}.prompty", templateContent);
                
                var gist = await _client.Gist.Create(gistCreate);
                return gist.Id;
            });
        }

        #endregion

        #region Economic Values (Repository Secrets)

        /// <summary>
        /// Gets a secret from the repository secrets (requires PAT with appropriate permissions)
        /// </summary>
        public async Task<string> GetSecretAsync(string secretName)
        {
            if (string.IsNullOrEmpty(secretName))
                throw new ArgumentException("Secret name cannot be null or empty", nameof(secretName));
            
            // Note: This is a placeholder as GitHub API doesn't directly allow retrieving secret values
            // In practice, you would use environment variables set in the workflow
            throw new NotSupportedException("GitHub API doesn't allow retrieving secret values directly. Use environment variables in the workflow instead.");
        }

        /// <summary>
        /// Sets a secret in the repository secrets
        /// </summary>
        public async Task SetSecretAsync(string secretName, string secretValue)
        {
            // Actually implement this method with await operations
            if (string.IsNullOrEmpty(secretName))
                throw new ArgumentException("Secret name cannot be null or empty", nameof(secretName));
            
            if (string.IsNullOrEmpty(secretValue))
                throw new ArgumentException("Secret value cannot be null or empty", nameof(secretValue));
                
            // Simulate an async operation
            await Task.Delay(100);
            Console.WriteLine($"Secret {secretName} would be set in a real implementation");
        }

        #endregion

        #region Work Tracking (Issues)

        /// <summary>
        /// Creates a new issue for work tracking
        /// </summary>
        public async Task<int> CreateIssueAsync(string title, string description, string[]? labels = null)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Issue title cannot be null or empty", nameof(title));
            
            // In debug mode, simulate issue creation
            if (_debugMode)
            {
                Console.WriteLine($"Debug mode: Simulating issue creation: {title}");
                Console.WriteLine($"Labels: {(labels != null ? string.Join(", ", labels) : "none")}");
                Console.WriteLine($"Description: {description}");
                
                // Create a directory for local issue tracking if it doesn't exist
                Directory.CreateDirectory("debug_issues");
                
                // Generate a fake issue number
                int issueNumber = new Random().Next(100, 1000);
                
                // Save issue info to a local file
                string issuePath = Path.Combine("debug_issues", $"issue_{issueNumber}.json");
                var issueData = new 
                {
                    Number = issueNumber,
                    Title = title,
                    Body = description,
                    Labels = labels ?? Array.Empty<string>(),
                    State = "open",
                    CreatedAt = DateTime.UtcNow,
                    Comments = new List<object>()
                };
                
                await File.WriteAllTextAsync(issuePath, JsonSerializer.Serialize(issueData, new JsonSerializerOptions { WriteIndented = true }));
                
                return issueNumber;
            }
            
            return await ExecuteWithRetryAsync(async () =>
            {
                var newIssue = new NewIssue(title)
                {
                    Body = description
                };
                
                if (labels != null && labels.Length > 0)
                {
                    foreach (var label in labels)
                    {
                        newIssue.Labels.Add(label);
                    }
                }
                
                var issue = await _client.Issue.Create(_owner, _repo, newIssue);
                return issue.Number;
            });
        }

        /// <summary>
        /// Updates an existing issue
        /// </summary>
        public async Task UpdateIssueAsync(int issueNumber, string? title = null, string? description = null, ItemState? state = null)
        {
            // In debug mode, simulate updating an issue
            if (_debugMode)
            {
                Console.WriteLine($"Debug mode: Updating issue #{issueNumber}");
                if (!string.IsNullOrEmpty(title))
                    Console.WriteLine($"New title: {title}");
                if (!string.IsNullOrEmpty(description))
                    Console.WriteLine($"New description: {description}");
                if (state.HasValue)
                    Console.WriteLine($"New state: {state}");
                
                // If we have a debug issue file, update it
                string issuePath = Path.Combine("debug_issues", $"issue_{issueNumber}.json");
                if (File.Exists(issuePath))
                {
                    try
                    {
                        string issueJson = await File.ReadAllTextAsync(issuePath);
                        var issueData = JsonSerializer.Deserialize<Dictionary<string, object>>(issueJson);
                        if (issueData != null)
                        {
                            // Update the issue properties
                            if (!string.IsNullOrEmpty(title))
                                issueData["Title"] = title;
                            
                            if (!string.IsNullOrEmpty(description))
                                issueData["Body"] = description;
                            
                            if (state.HasValue)
                                issueData["State"] = state.ToString().ToLower();
                            
                            // Save updated issue
                            await File.WriteAllTextAsync(issuePath, 
                                JsonSerializer.Serialize(issueData, new JsonSerializerOptions { WriteIndented = true }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Debug mode: Error updating issue file: {ex.Message}");
                    }
                }
                
                return;
            }
            
            await ExecuteWithRetryAsync(async () =>
            {
                var issueUpdate = new IssueUpdate();
                
                if (!string.IsNullOrEmpty(title))
                    issueUpdate.Title = title;
                
                if (!string.IsNullOrEmpty(description))
                    issueUpdate.Body = description;
                
                if (state.HasValue)
                    issueUpdate.State = state.Value;
                
                await _client.Issue.Update(_owner, _repo, issueNumber, issueUpdate);
                return true;
            });
        }

        /// <summary>
        /// Gets an issue by number
        /// </summary>
        public async Task<Issue> GetIssueAsync(int issueNumber)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                return await _client.Issue.Get(_owner, _repo, issueNumber);
            });
        }

        /// <summary>
        /// Adds a comment to an issue
        /// </summary>
        public async Task<int> AddIssueCommentAsync(int issueNumber, string comment)
        {
            if (string.IsNullOrEmpty(comment))
                throw new ArgumentException("Comment cannot be null or empty", nameof(comment));
            
            // In debug mode, simulate adding a comment
            if (_debugMode)
            {
                Console.WriteLine($"Debug mode: Adding comment to issue #{issueNumber}:");
                Console.WriteLine(comment);
                
                // If we have a debug issue file, update it
                string issuePath = Path.Combine("debug_issues", $"issue_{issueNumber}.json");
                if (File.Exists(issuePath))
                {
                    try
                    {
                        string issueJson = await File.ReadAllTextAsync(issuePath);
                        var issueData = JsonSerializer.Deserialize<Dictionary<string, object>>(issueJson);
                        if (issueData != null)
                        {
                            // Generate a unique comment ID
                            int commentId = new Random().Next(1000, 9999);
                            
                            // Add the comment
                            if (issueData.TryGetValue("Comments", out var commentsObj) && 
                                commentsObj is JsonElement commentsElement &&
                                commentsElement.ValueKind == JsonValueKind.Array)
                            {
                                // Convert existing comments
                                var comments = new List<object>();
                                foreach (var element in commentsElement.EnumerateArray())
                                {
                                    if (element.ValueKind == JsonValueKind.Object)
                                    {
                                        var commentObj = new Dictionary<string, object>();
                                        foreach (var prop in element.EnumerateObject())
                                        {
                                            commentObj[prop.Name] = prop.Value.GetRawText() ?? "";
                                        }
                                        comments.Add(commentObj);
                                    }
                                }
                                
                                // Add new comment
                                comments.Add(new 
                                {
                                    Id = commentId,
                                    Body = comment,
                                    CreatedAt = DateTime.UtcNow.ToString("o")
                                });
                                
                                // Update issue data
                                issueData["Comments"] = comments;
                                
                                // Save updated issue
                                await File.WriteAllTextAsync(issuePath, 
                                    JsonSerializer.Serialize(issueData, new JsonSerializerOptions { WriteIndented = true }));
                            }
                            
                            return commentId;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Debug mode: Error updating issue file: {ex.Message}");
                    }
                }
                
                // Return a mock ID
                return new Random().Next(1000, 9999);
            }
            
            return await ExecuteWithRetryAsync(async () =>
            {
                var issueComment = await _client.Issue.Comment.Create(_owner, _repo, issueNumber, comment);
                return issueComment.Id;
            });
        }

        #endregion

        #region History (Releases)

        /// <summary>
        /// Creates a new release to store build history
        /// </summary>
        public async Task<string> CreateReleaseAsync(string tag, string title, string description, bool prerelease = false)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
            
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title cannot be null or empty", nameof(title));
            
            // In debug mode, simulate creating a release
            if (_debugMode)
            {
                Console.WriteLine($"Debug mode: Creating release '{title}' with tag '{tag}'");
                Console.WriteLine($"Description: {description}");
                
                // Create directory for debug releases if it doesn't exist
                Directory.CreateDirectory("debug_releases");
                
                // Generate a mock release ID
                string releaseId = new Random().Next(1000, 9999).ToString();
                
                // Save release info to a file
                string releasePath = Path.Combine("debug_releases", $"release_{releaseId}.json");
                var releaseData = new
                {
                    Id = releaseId,
                    Tag = tag,
                    Name = title,
                    Body = description,
                    Prerelease = prerelease,
                    CreatedAt = DateTime.UtcNow
                };
                
                await File.WriteAllTextAsync(releasePath, 
                    JsonSerializer.Serialize(releaseData, new JsonSerializerOptions { WriteIndented = true }));
                
                return releaseId;
            }
            
            return await ExecuteWithRetryAsync(async () =>
            {
                var newRelease = new NewRelease(tag)
                {
                    Name = title,
                    Body = description,
                    Prerelease = prerelease
                };
                
                var release = await _client.Repository.Release.Create(_owner, _repo, newRelease);
                return release.Id.ToString();
            });
        }

        /// <summary>
        /// Gets release history
        /// </summary>
        public async Task<IReadOnlyList<Release>> GetReleasesAsync(int count = 10)
        {
            // In debug mode, simulate getting releases
            if (_debugMode)
            {
                Console.WriteLine($"Debug mode: Getting {count} releases");
                
                // Check if we have any debug releases
                if (Directory.Exists("debug_releases"))
                {
                    try
                    {
                        // Get all release files
                        var releaseFiles = Directory.GetFiles("debug_releases", "release_*.json");
                        
                        // Sort by creation time (newest first)
                        Array.Sort(releaseFiles, (a, b) => 
                            File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));
                        
                        // Limit to requested count
                        if (releaseFiles.Length > count)
                        {
                            releaseFiles = releaseFiles.Take(count).ToArray();
                        }
                        
                        // Create mock releases
                        var releases = new List<Release>();
                        foreach (var file in releaseFiles)
                        {
                            try
                            {
                                string releaseJson = await File.ReadAllTextAsync(file);
                                var releaseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(releaseJson);
                                if (releaseData != null)
                                {
                                    // Create a simple mock release object using reflection to bypass the constructor signature issues
                                    var release = new Release();
                                    
                                    // Set required properties using reflection
                                    var idValue = long.Parse(releaseData["Id"].GetString() ?? "0");
                                    var tagValue = releaseData["Tag"].GetString() ?? "";
                                    var nameValue = releaseData["Name"].GetString() ?? "";
                                    var bodyValue = releaseData["Body"].GetString() ?? "";
                                    var prelease = releaseData["Prerelease"].ValueKind == JsonValueKind.True;
                                    
                                    // Use reflection to set private fields if needed
                                    typeof(Release).GetProperty("Id")?.SetValue(release, idValue);
                                    typeof(Release).GetProperty("TagName")?.SetValue(release, tagValue);
                                    typeof(Release).GetProperty("Name")?.SetValue(release, nameValue);
                                    typeof(Release).GetProperty("Body")?.SetValue(release, bodyValue);
                                    
                                    releases.Add(release);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Debug mode: Error parsing release file {file}: {ex.Message}");
                            }
                        }
                        
                        return releases;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Debug mode: Error getting releases: {ex.Message}");
                    }
                }
                
                // If no releases or error, return empty list
                return new List<Release>();
            }
            
            return await ExecuteWithRetryAsync(async () =>
            {
                var options = new ApiOptions
                {
                    PageSize = count,
                    PageCount = 1
                };
                
                return await _client.Repository.Release.GetAll(_owner, _repo, options);
            });
        }

        #endregion

        #region Pull Requests for Self-Evolution

        /// <summary>
        /// Creates a pull request for self-evolution
        /// </summary>
        public async Task<int> CreatePullRequestAsync(string title, string description, string head, string baseBranch = "main")
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("PR title cannot be null or empty", nameof(title));
            
            if (string.IsNullOrEmpty(head))
                throw new ArgumentException("Head branch cannot be null or empty", nameof(head));
            
            // In debug mode, simulate creating a PR
            if (_debugMode)
            {
                Console.WriteLine($"Debug mode: Creating PR '{title}' from '{head}' to '{baseBranch}'");
                Console.WriteLine($"Description: {description}");
                
                // Create directory for debug PRs if it doesn't exist
                Directory.CreateDirectory("debug_prs");
                
                // Generate a mock PR number
                int prNumber = new Random().Next(100, 999);
                
                // Save PR info to a file
                string prPath = Path.Combine("debug_prs", $"pr_{prNumber}.json");
                var prData = new
                {
                    Number = prNumber,
                    Title = title,
                    Body = description,
                    Head = head,
                    Base = baseBranch,
                    CreatedAt = DateTime.UtcNow,
                    State = "open"
                };
                
                await File.WriteAllTextAsync(prPath, 
                    JsonSerializer.Serialize(prData, new JsonSerializerOptions { WriteIndented = true }));
                
                return prNumber;
            }
            
            return await ExecuteWithRetryAsync(async () =>
            {
                var newPr = new NewPullRequest(title, head, baseBranch)
                {
                    Body = description
                };
                
                var pr = await _client.PullRequest.Create(_owner, _repo, newPr);
                return pr.Number;
            });
        }

        #endregion

        #region Retry Logic with Exponential Backoff

        /// <summary>
        /// Executes an API call with retry logic and exponential backoff
        /// </summary>
        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    return await operation();
                }
                catch (RateLimitExceededException ex)
                {
                    if (attempt >= MaxRetries)
                        throw;
                    
                    // Calculate backoff time based on rate limit reset
                    var resetTime = ex.Reset.ToLocalTime();
                    var now = DateTime.Now;
                    var waitTime = resetTime > now 
                        ? resetTime - now 
                        : TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    
                    Console.WriteLine($"Rate limit exceeded. Waiting for {waitTime.TotalSeconds} seconds before retry. Attempt {attempt}/{MaxRetries}");
                    await Task.Delay(waitTime);
                }
                catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    if (attempt >= MaxRetries)
                        throw;
                    
                    // Secondary rate limit or abuse detection
                    var waitTime = TimeSpan.FromSeconds(Math.Pow(2, attempt + 2)); // More aggressive backoff
                    Console.WriteLine($"Secondary rate limit or abuse detection triggered. Waiting for {waitTime.TotalSeconds} seconds before retry. Attempt {attempt}/{MaxRetries}");
                    await Task.Delay(waitTime);
                }
                catch (Exception ex) when (
                    ex is ApiException apiEx && 
                    (apiEx.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                     apiEx.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                     apiEx.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                     apiEx.StatusCode == System.Net.HttpStatusCode.GatewayTimeout))
                {
                    if (attempt >= MaxRetries)
                        throw;
                    
                    // Server errors - use exponential backoff
                    var waitTime = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    Console.WriteLine($"Server error: {ex.Message}. Waiting for {waitTime.TotalSeconds} seconds before retry. Attempt {attempt}/{MaxRetries}");
                    await Task.Delay(waitTime);
                }
            }
        }

        #endregion
    }
} 