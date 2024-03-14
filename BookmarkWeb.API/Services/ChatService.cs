using System.Text;
using BookmarkWeb.API.Hubs;
using BookmarkWeb.API.Models.Common.Schemas;
using Microsoft.AspNetCore.SignalR;

namespace BookmarkWeb.API.Services
{
    public interface IChatService
    {
        public Task<string> SummaryFileAsync(string conversationId, string replyId);
        public Task<string> SendChatAsync(string conversationId, string question, string replyId, bool isExpanded = false);
    }
    public class ChatService : IChatService
    {
        private readonly IApiService _apiService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public ChatService(
            IApiService apiService,
            IConfiguration configuration,
            IHubContext<NotificationHub> notificationHub)
        {
            var host = configuration.GetValue<string>("ChatService:Host");
            _apiService = apiService;
            _apiService.Setting(host);
            _notificationHub = notificationHub;
        }
        public async Task<string> SendChatAsync(string conversationId, string question, string replyId, bool expanded = false)
        {
            return await SendMessageAsync(conversationId, new { question, expanded }, replyId);
        }

        public async Task<string> SummaryFileAsync(string conversationId, string replyId)
        {
            return await SendMessageAsync(conversationId, null, replyId);
        }

        private async Task<string> SendMessageAsync(string conversationId, object body, string replyId)
        {
            var response = await _apiService.PostAsync($"/chat/{conversationId}", body, isStreaming: true);
            response.EnsureSuccessStatusCode();

            var username = AppState.Instance.CurrentUser.Username;
            string responseMessage = "";
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                // Process the content stream as needed
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    responseMessage += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    await _notificationHub.Clients
                        .User(username)
                        .SendAsync(NotificationHub.GPT_WRITE_REPLY, new
                        {
                            id = replyId,
                            content = responseMessage
                        });
                }
            }
            return responseMessage;
        }
    }
}