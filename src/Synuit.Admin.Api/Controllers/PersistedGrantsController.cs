using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synuit.Admin.Api.Configuration.Constants;
using Synuit.Admin.Api.Dtos.PersistedGrants;
using Synuit.Admin.Api.ExceptionHandling;
using Synuit.Admin.Api.Helpers;
using Synuit.Admin.Api.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;

namespace Synuit.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class PersistedGrantsController : ControllerBase
    {
        private readonly IPersistedGrantAspNetIdentityService _persistedGrantsService;

        public PersistedGrantsController(IPersistedGrantAspNetIdentityService persistedGrantsService)
        {
            _persistedGrantsService = persistedGrantsService;
        }

        [HttpGet("Subjects")]
        public async Task<ActionResult<PersistedGrantSubjectsApiDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var persistedGrantsDto = await _persistedGrantsService.GetPersistedGrantsByUsersAsync(searchText, page, pageSize);
            var persistedGrantSubjectsApiDto = persistedGrantsDto.ToPersistedGrantApiModel<PersistedGrantSubjectsApiDto>();

            return Ok(persistedGrantSubjectsApiDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersistedGrantApiDto>> Get(string id)
        {
            var persistedGrantDto = await _persistedGrantsService.GetPersistedGrantAsync(UrlHelpers.QueryStringUnSafeHash(id));
            var persistedGrantApiDto = persistedGrantDto.ToPersistedGrantApiModel<PersistedGrantApiDto>();

            ParsePersistedGrantKey(persistedGrantApiDto);

            return Ok(persistedGrantApiDto);
        }

        [HttpGet("Subjects/{subjectId}")]
        public async Task<ActionResult<PersistedGrantsApiDto>> GetBySubject(string subjectId, int page = 1, int pageSize = 10)
        {
            var persistedGrantDto = await _persistedGrantsService.GetPersistedGrantsByUserAsync(subjectId, page, pageSize);
            var persistedGrantApiDto = persistedGrantDto.ToPersistedGrantApiModel<PersistedGrantsApiDto>();

            ParsePersistedGrantKeys(persistedGrantApiDto);

            return Ok(persistedGrantApiDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _persistedGrantsService.DeletePersistedGrantAsync(UrlHelpers.QueryStringUnSafeHash(id));

            return Ok();
        }

        [HttpDelete("Subjects/{subjectId}")]
        public async Task<IActionResult> DeleteBySubject(string subjectId)
        {
            await _persistedGrantsService.DeletePersistedGrantsAsync(subjectId);

            return Ok();
        }

        private void ParsePersistedGrantKey(PersistedGrantApiDto persistedGrantApiDto)
        {
            if (!string.IsNullOrEmpty(persistedGrantApiDto.Key))
            {
                persistedGrantApiDto.Key = UrlHelpers.QueryStringSafeHash(persistedGrantApiDto.Key);
            }
        }

        private void ParsePersistedGrantKeys(PersistedGrantsApiDto persistedGrantApiDto)
        {
            persistedGrantApiDto.PersistedGrants.ForEach(ParsePersistedGrantKey);
        }
    }
}





