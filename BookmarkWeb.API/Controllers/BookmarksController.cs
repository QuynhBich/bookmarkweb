using System.Net;
using BookmarkWeb.API.Database.Entities;
using BookmarkWeb.API.Models.Bookmarks;
using BookmarkWeb.API.Models.Bookmarks.Schemas;
using BookmarkWeb.API.Models.Common.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkWeb.API.Controllers
{
    [Route("bookmarks")]
    [Filters.SetAppState]
    public class BookmarksController: ControllerBase
    {
        private readonly IBookmarkModel _bookmarkModel;

        public BookmarksController(IBookmarkModel bookmarkModel)
        {
            _bookmarkModel = bookmarkModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BookmarkDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetListBookmark()
        {
            try
            {
                return Ok(await _bookmarkModel.GetListBookmark());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost("create-bookmark")]
        [Authorize]
        public async Task<ActionResult<BookmarkDto>>  CreateNewBookmark(BookmarkCreateModel bookmark)
        {
            try
            {
                var response = await _bookmarkModel.CreateNewBookmark(bookmark);
                return response;
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
    }
}