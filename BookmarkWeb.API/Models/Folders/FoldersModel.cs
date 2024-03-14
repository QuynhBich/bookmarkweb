using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BookmarkWeb.API.Database.Entities;
using BookmarkWeb.API.Models.Common.Schemas;
using BookmarkWeb.API.Models.Folders.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookmarkWeb.API.Models.Folders
{
    public interface IFolderModel
    {
        Task<ResponseInfo> CreateNewFolder(FolderCreateModel data);
        Task<List<FolderDto>> GetListFolder();
    }
    public class FoldersModel : BaseModel, IFolderModel
    {
        private readonly ILogger<FoldersModel> _logger;
        private string _className = "";
        public FoldersModel(ILogger<FoldersModel> logger, IServiceProvider provider) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }
        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
        public async Task<ResponseInfo> CreateNewFolder(FolderCreateModel data)
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
                var folder = new Folder()
                {
                    Id = newId,
                    Name = data.Name,
                    Description = data.Description,
                    UserId = userId,
                };

                _context.Folders.Add(folder);
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

        public async Task<List<FolderDto>> GetListFolder()
        {
            string method = GetActualAsyncMethodName();
            try{
                _logger.LogInformation($"[{_className}][{method}] Start");
                List<FolderDto> result = new();
                var user = AppState.Instance.CurrentUser.Id;
                result = await _context.Folders.Where(x => x.UserId == user).Select(x => new FolderDto(){
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name
                }).ToListAsync();
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