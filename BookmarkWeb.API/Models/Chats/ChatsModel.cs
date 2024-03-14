using System.Runtime.CompilerServices;
using BookmarkWeb.API.Commons;
using BookmarkWeb.API.Database.Entities;
using BookmarkWeb.API.Hubs;
using BookmarkWeb.API.Models.Chats.Schemas;
using BookmarkWeb.API.Models.Common.Schemas;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookmarkWeb.API.Models.Chats
{
    public interface IChatsModel
    {
        Task<ResponseInfo> SaveMessage(Schemas.Conversation data);
        Task<ResponseInfo> CreateNewConversation(Schemas.Conversation data);
    }
    public class ChatsModel : BaseModel,IChatsModel
    {
        private string _className = "";
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly ILogger<ChatsModel> _logger;

        public ChatsModel(
            IHubContext<NotificationHub> notificationHub,
            ILogger<ChatsModel> logger,
            IServiceProvider provider
        ) : base(provider)
        {
            _notificationHub = notificationHub ?? throw new ArgumentNullException(nameof(notificationHub));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }
        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
        public async Task<ResponseInfo> CreateNewConversation(Schemas.Conversation data)
        {
            ResponseInfo response = new ResponseInfo();
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                var userId = AppState.Instance.CurrentUser.Id;
                _ = Guid.TryParse(data.ConversationId, out Guid conversationId);
                _ = Guid.TryParse(data.Bookmark.BookmarkId, out Guid bookmarkId);
                transaction = await _context.Database.BeginTransactionAsync();
                var conversation = new Database.Entities.Conversation()
                {
                    Id = conversationId,
                    Name = data.Name,
                    Description = "",
                    BookmarkId = bookmarkId,
                };
                if (data.Name.Length > 50)
                {
                    conversation.Name = data.Name.Substring(0, 50);
                }

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"[{_className}][{method}] End");
            }
            catch (Exception e)
            {
                await _context.RollbackAsync(transaction);
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");
                response.Exception(e);

                throw;
            }

            return response;
        }

        public async Task<ResponseInfo> SaveMessage(Schemas.Conversation data)
        {
            ResponseInfo response = new ResponseInfo();
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                var userId = AppState.Instance.CurrentUser.Id;
                
                transaction = await _context.Database.BeginTransactionAsync();
                _ = Guid.TryParse(data.ConversationId, out Guid conversationId);
                var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.Id.ToString() == data.ConversationId);
                if (conversation != null)
                {
                    conversation.Description = data.Reply.Content.Length > 100 ? $"{data.Reply.Content.Substring(0, 97)}..." : data.Reply.Content;
                }
                if (data.Message is not null)
                {
                    _ = Guid.TryParse(data.Message?.Id, out Guid messageId);
                    _context.Messages.Add(new Message()
                    {
                        Id = messageId,
                        ConversationId = conversationId,
                        Content = data.Message.Content,
                        UserId = userId,
                    });
                    await _context.SaveChangesAsync();
                }
                _ = Guid.TryParse(data.Reply.Id, out Guid replyId);
                _context.Messages.Add(new Message()
                {
                    Id = replyId,
                    ConversationId = conversationId,
                    Content = data.Reply.Content,
                    UserId = null,
                });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation($"[{_className}][{method}] End");
            }
            catch (Exception e)
            {
                await _context.RollbackAsync(transaction);
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");
                response.Exception(e);
            }
            return response;
        }
    }
}