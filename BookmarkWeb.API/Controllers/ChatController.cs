using System.Net;
using BookmarkWeb.API.Models.Chats;
using BookmarkWeb.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkWeb.API.Controllers
{
    [Route("chats")]
    [Filters.SetAppState]
    public class ChatController: ControllerBase
    {
        private readonly IChatsModel _chatsModel;
        private readonly IChatService _chatService;

        public ChatController(
            IChatsModel chatsModel,
            IServiceProvider provider,
            IChatService chatService
        )
        {
            _chatsModel = chatsModel ?? throw new ArgumentNullException(nameof(chatsModel));
            _chatService = chatService;
        }

        [HttpGet]
        [Authorize]
        public async Task<string> Demo()
        {
            var result = await _chatService.SummaryFileAsync("abc", "abc");
            return result;
        }
    }
}