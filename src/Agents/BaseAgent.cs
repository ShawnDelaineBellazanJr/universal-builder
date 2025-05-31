using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace UniversalAutonomousBuilder.Agents
{
    /// <summary>
    /// Base class for all Universal Autonomous Builder agents
    /// </summary>
    public abstract class BaseAgent
    {
        protected readonly Kernel _kernel;
        protected readonly IChatCompletionService _chatService;
        protected readonly string _model;
        protected readonly bool _debugMode;
        
        public BaseAgent(Kernel kernel, string model = "gpt-4", bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _model = model;
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Execute a prompt with the given system and user messages
        /// </summary>
        protected async Task<string> ExecutePromptAsync(string systemMessage, string userMessage)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing prompt: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock response in debug mode
                    return $"DEBUG MODE: Response to prompt: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...";
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Execute a prompt with a chat history and chain of thought
        /// </summary>
        protected async Task<string> ExecuteChainOfThoughtAsync(ChatHistory chatHistory, string userMessage)
        {
            chatHistory.AddUserMessage($"Let's think step-by-step about this: {userMessage}");
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing chain of thought: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock response in debug mode
                    return $"DEBUG MODE: Chain of thought response about: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...";
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Execute a prompt using a Prompty template file
        /// </summary>
        protected async Task<string> ExecutePromptyTemplateAsync(string templatePath, Dictionary<string, string> variables)
        {
            try
            {
                // In a real implementation, we would load and parse the Prompty template
                // For now, we'll use a simple approach
                string templateContent = await File.ReadAllTextAsync(templatePath);
                
                // Replace variables in the template
                foreach (var kvp in variables)
                {
                    templateContent = templateContent.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
                }
                
                // Parse the template to extract system and user messages
                var chatHistory = new ChatHistory();
                
                // Simple parser for demonstration purposes
                string[] sections = templateContent.Split("<message role=\"", StringSplitOptions.RemoveEmptyEntries);
                foreach (var section in sections)
                {
                    if (section.StartsWith("system\">"))
                    {
                        string content = section.Substring("system\">".Length);
                        int endIndex = content.IndexOf("</message>");
                        if (endIndex >= 0)
                        {
                            chatHistory.AddSystemMessage(content.Substring(0, endIndex).Trim());
                        }
                    }
                    else if (section.StartsWith("user\">"))
                    {
                        string content = section.Substring("user\">".Length);
                        int endIndex = content.IndexOf("</message>");
                        if (endIndex >= 0)
                        {
                            chatHistory.AddUserMessage(content.Substring(0, endIndex).Trim());
                        }
                    }
                }
                
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing Prompty template: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock response in debug mode
                    return $"DEBUG MODE: Response from template {templatePath} with variables: {string.Join(", ", variables.Keys)}";
                }
                
                throw;
            }
        }
    }
} 