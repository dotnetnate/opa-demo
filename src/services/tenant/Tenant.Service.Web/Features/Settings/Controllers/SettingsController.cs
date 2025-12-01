using AutoMapper;
using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Elements.ApplicationModel.Queries;
using NOCO.Threvw.Tenants.Domain;
using NOCO.Threvw.Tenants.Service.Http.Features.Settings.Models;
using NOCO.Threvw.Tenants.Service.Http.Features.Tenants;
using NOCO.Threvw.Tenants.Service.Http.Features.Tenants.Models;
using NOCO.Threvw.Tenants.Service.Http.TBD;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace NOCO.Threvw.Tenants.Service.Http.Features.Settings.Controllers {

    [ApiController]
    [Route("api/tenant/{tenantId}/settings")]
    [EnableCors("AllowAll")]
    public class SettingsController : Controller {
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantController> _logger;
        private readonly IMapper _mapper;

        public SettingsController(ITenantService tenantService, ILogger<TenantController> logger, IMapper mapper) {
            _tenantService = tenantService;
            _logger = logger;
            _mapper = mapper;
        }

        [Route("{settingName}")]        
        [HttpGet]
        public async Task<IActionResult> GetSettingsGroup([FromRoute] GetTenantSettingsRequest request) {

            var query = _mapper.Map<FindTenantSettingsQuery>(request);

            var result = await _tenantService.GetSettingByKey(query);

            if (result.IsSuccess()) {
                var retVal = result.Result; // just return the raw settings for now, this really should just be a document //_mapper.Map<IEnumerable<Models.Tenant>>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [Route("{settingName}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSettingsGroup( [FromRoute] Guid tenantId, [FromRoute] string settingName,   [FromBody] UpdateOptionsGroupRequest request) {

            request.TenantId = tenantId;
            request.OptionsGroupName = settingName;

            var entity = _mapper.Map<Domain.UpdateOptionsGroupCommand>(request);
            var result = await _tenantService.UpdateSetting(entity);

            if (result.IsSuccess()) {
                var retVal = result.Result; // same thing //_mapper.Map<Models.Tenant>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }


        private IActionResult BuildErrorResponse<T>(QueryResult<T> result) {

            Shared.Models.InternalValidationResult? validationResult = null;

            if (result.ValidationResult != null) {
                validationResult = _mapper.Map<Shared.Models.InternalValidationResult>(result.ValidationResult);
            }


            if (result.ValidationResult?.IsValid() == false) {
                return BadRequest(new ErrorResult { ValidationResult = validationResult, Errors = result.ErrorMessages });
            }

            if (result.FailureCategory == QueryFailureCategory.ResourceNotFound) {
                return NotFound();
            }
            return Problem("An unknown error occurred");
        }
        private IActionResult BuildErrorResponse<T>(CommandResult<T> result) {

            Shared.Models.InternalValidationResult? validationResult = null;

            if (result.ValidationResult != null) {
                validationResult = _mapper.Map<Shared.Models.InternalValidationResult>(result.ValidationResult);
            }


            if (result.ValidationResult?.IsValid() == false) {
                return BadRequest(new ErrorResult { ValidationResult = validationResult, Errors = result.ErrorMessages });
            }

            if (result.FailureCategory == CommandFailureCategory.VersionConlfict) {
                return Conflict();
            }

            if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                return BadRequest(new ErrorResult { ValidationResult = validationResult, Errors = result.ErrorMessages });
            }

            if (result.FailureCategory == CommandFailureCategory.InsufficientPermissions) {
                return NotFound();
            }

            if (result.FailureCategory == CommandFailureCategory.GeneralRuntimeError) {
                return Problem("An unknown error occurred");
            }

            return Problem("An unknown error occurred");
        }
        private IActionResult BuildErrorResponse(CommandResult result) {

            Shared.Models.InternalValidationResult? validationResult = null;

            if (result.ValidationResult != null) {
                validationResult = _mapper.Map<Shared.Models.InternalValidationResult>(result.ValidationResult);
            }


            if (result.ValidationResult?.IsValid() == false) {
                return BadRequest(new ErrorResult { ValidationResult = validationResult, Errors = result.ErrorMessages });
            }

            if (result.FailureCategory == CommandFailureCategory.VersionConlfict) {
                return Conflict();
            }

            if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                return BadRequest(new ErrorResult { ValidationResult = validationResult, Errors = result.ErrorMessages });
            }

            if (result.FailureCategory == CommandFailureCategory.InsufficientPermissions) {
                return NotFound();
            }

            if (result.FailureCategory == CommandFailureCategory.GeneralRuntimeError) {
                return Problem("An unknown error occurred");
            }

            return Problem("An unknown error occurred");
        }
    }
}
