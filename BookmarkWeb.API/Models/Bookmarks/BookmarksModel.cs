using System.Runtime.CompilerServices;
using BookmarkWeb.API.Database.Entities;
using BookmarkWeb.API.Models.Bookmarks.Schemas;
using BookmarkWeb.API.Models.Common.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookmarkWeb.API.Models.Bookmarks
{
    public interface IBookmarkModel
    {
        Task<ResponseInfo> CreateNewBookmark(BookmarkCreateModel data);
        Task<List<BookmarkDto>> GetListBookmark();
    }
    public class BookmarksModel : BaseModel, IBookmarkModel
    {
        private readonly ILogger<BookmarksModel> _logger;
        private string _className = "";
        public BookmarksModel(ILogger<BookmarksModel> logger, IServiceProvider provider) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }
        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
        public async Task<ResponseInfo> CreateNewBookmark(BookmarkCreateModel data)
        {
            ResponseInfo response = new();
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                var userId = AppState.Instance.CurrentUser.Id;
                var newId = Guid.NewGuid();
                transaction = await _context.Database.BeginTransactionAsync();
                var bookmark = new Bookmark()
                {
                    Id = newId,
                    UserId = userId,
                    Domain = data.Domain,
                    Url = data.Url,
                    Image = data.Image,
                    Note = data.Note,
                    FolderId = data.FolderId
                };

                _context.Bookmarks.Add(bookmark);
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

        public async Task<List<BookmarkDto>> GetListBookmark()
        {
            string method = GetActualAsyncMethodName();
            try{
                _logger.LogInformation($"[{_className}][{method}] Start");
                List<BookmarkDto> result = new();
                var user = AppState.Instance.CurrentUser.Id;
                result = await _context.Bookmarks.Where(x => x.UserId == user).Select(x => new BookmarkDto(){
                    Id = x.Id,
                    Url = x.Url,
                    Description = x.Description,
                    Domain = x.Domain,
                    Image = x.Image,
                    Note = x.Note,
                    CreatedAt = x.CreatedAt,
                }).OrderByDescending(x => x.CreatedAt).ToListAsync();
                _logger.LogInformation($"[{_className}][{method}] End");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");
                throw;
            }
        }
    }
}