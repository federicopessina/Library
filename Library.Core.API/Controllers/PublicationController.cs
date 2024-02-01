using Library.Core.Exceptions;
using Library.Core.Exceptions.PublicationStore;
using Library.Core.Exceptions.Results;
using Library.Entities;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers
{
    /// <summary>
    /// PublicationController.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationController : ControllerBase, IPublicationController
    {
        private const string PublicationTag = "Publication";
        private readonly IPublicationStore _publicationStore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publicationStore"></param>
        public PublicationController(IPublicationStore publicationStore)
        {
            this._publicationStore = publicationStore;
        }
        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        [Tags(PublicationTag)]
        [HttpDelete($"{nameof(Delete)}/{{isbn}}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string isbn)
        {
            try
            {
                await _publicationStore.DeleteAsync(isbn);
                return base.Ok(isbn);
            }
            catch (StoreIsEmptyException)
            {

                return base.NoContent();
            }
            catch (IsbnNotFoundException)
            {

                return base.NotFound();
            }
        }
        /// <summary>
        /// Get.
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        [Tags(PublicationTag)]
        [HttpGet($"{nameof(Get)}/{{isbn}}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string isbn)
        {
            try
            {
                var result = await _publicationStore.GetAsync(isbn);
                return base.Ok(result);
            }
            catch (StoreIsEmptyException)
            {

                return base.NoContent();
            }
            catch (IsbnNotFoundException)
            {

                return base.NotFound();
            }
        }
        /// <summary>
        /// GetAll.
        /// </summary>
        /// <returns></returns>
        [Tags(PublicationTag)]
        [HttpGet($"{nameof(GetAll)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _publicationStore.GetAllAsync();
                return Ok(result);
            }
            catch (StoreIsEmptyException)
            {

                return NoContent();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }
        /// <summary>
        /// GetByAuthor.
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [Tags(PublicationTag)]
        [HttpGet($"{nameof(GetByAuthor)}/{{author}}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAuthor(string author)
        {
            try
            {
                var result = await _publicationStore.GetByAuthorAsync(author);
                return Ok(result);
            }
            catch (StoreIsEmptyException)
            {

                return NoContent();
            }
            catch (EmptyResultException)
            {

                return NotFound();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="publication"></param>
        /// <returns></returns>
        [Tags(PublicationTag)]
        [HttpPut(nameof(Insert))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Insert([FromBody] Publication publication)
        {
            try
            {
                await _publicationStore.InsertAsync(publication);
                return Ok(publication);
            }
            catch (DuplicatedIsbnException)
            {

                return Conflict(publication);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }
}
