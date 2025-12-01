using AutoMapper;
using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Elements.ApplicationModel.Queries;
using NOCO.Threvw.Tenants.Domain;
using NOCO.Threvw.Tenants.Service.Http.Features.Tenants.Models;
using NOCO.Threvw.Tenants.Service.Http.TBD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Service.Http.Features.Tenants {
    [ApiController]
    [Route("api/tenant")]
    public class TenantController : ControllerBase {
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantController> _logger;
        private readonly IMapper _mapper;

        public TenantController(ITenantService tenantService, ILogger<TenantController> logger, IMapper mapper) {
            _tenantService = tenantService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> FindTenants([FromBody] FindTenantsRequest request) {
                        
            var query = _mapper.Map<FindTenantsQuery>(request);

            var result = await _tenantService.GetTenants(query);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<IEnumerable<Models.Tenant>>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpGet("{id}")]        
        public async Task<IActionResult> GetTenantById([FromRoute]FindTenantByIdRequest request) {

            var query = _mapper.Map<FindTenantByIdQuery>(request);

            var result = await _tenantService.GetTenantById(query);
            
            if (result.IsSuccess()) {
                var retVal = _mapper.Map<Models.Tenant>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] Models.CreateTenantRequest model) {
            _logger.LogInformation("Creating tenant: {Tenant}", model);

            var entity = _mapper.Map<Domain.CreateTenantCommand>(model);
            var result = await _tenantService.CreateTenant(entity);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<Models.Tenant>(result.Result);
                return CreatedAtAction(nameof(GetTenantById), new { id = retVal.Id }, retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] Models.UpdateTenantRequest model) {
            
            var command = _mapper.Map<Domain.UpdateTenantCommand>(model);
            
            command.Id = id;

            var result = await _tenantService.UpdateTenant(command);
            
            if (result.IsSuccess()) {
                var retVal = _mapper.Map<Models.Tenant>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant([FromQuery] DeleteTenantRequest request) {

            var command = _mapper.Map<Domain.DeleteTenantCommand>(request);
            
            var result = await _tenantService.DeleteTenant(command);

            if (result.IsSuccess()) {                
                return NoContent();
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

            if( result.ValidationResult != null) {
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
