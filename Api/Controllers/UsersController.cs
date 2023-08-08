using Domain.Entities;
using Domain.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Application.Users.Dtos;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISaveable<User> _userSaver;
        private readonly IQuerableFilterable<User, UserFilter> _userQuerable;
        private readonly IFileProcessor _fileProcessor;

        public UsersController(
            ISaveable<User> userSaver,
            IQuerableFilterable<User, UserFilter> userQuerable,
            IFileProcessor fileProcessor)
        {
            _userSaver = userSaver;
            _userQuerable = userQuerable;
            _fileProcessor = fileProcessor;
        }

        [HttpGet(Name = nameof(Get))]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppResponse<Pagination<User>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(AppResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public ActionResult Get([FromQuery] UserFilter userFilter)
        {
            var resumen = _userQuerable.Get(userFilter) as Pagination<User>;
            return Ok(AppResponse<Pagination<User>>.Success(resumen));
        }

        [HttpPost(Name = nameof(Post))]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(AppResponse<User>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(AppResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public ActionResult Post([FromBody] CreateUser createUserDto)
        {
            var user = _userSaver.Save(createUserDto.ToUser());
            return Created("Created", AppResponse<User>.Success(user));
        }

        [HttpPost("bulk-data")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(AppResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> BatchUserInfoFromfile(IFormFile file)
        {
            if (file.ContentType != "text/csv") {
                throw new AppException("Invalid file type");
            }
            if (file.Length == 0)
            {
                throw new AppException("The files is empty");
            }

            using var fileStream = file.OpenReadStream();
            await _fileProcessor.Process(fileStream);

            return Accepted();
        }
    }
}
