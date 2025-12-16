using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Services;
using System.Net;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggingService _loggingService; // ADDED logging service
        public PersonController(IMediator mediator, ILoggingService loggingService)
        {
            _mediator = mediator;
            _loggingService = loggingService; //ADDED logging service
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {

                });

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // ADDED log exception first
                await _loggingService.LogException(
                    action: "GetPeople",
                    ex: ex
                );

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // ADDED log exception first
                await _loggingService.LogException(
                    action: "GetPersonByName",
                    ex: ex,
                    personName: name
                );

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });

                return this.GetResponse(result);
            }
            // ADDED 400 catch for expected errors
            catch (BadHttpRequestException ex)
            {
                // This is a validation error (expected) - use LogError, not LogException
                await _loggingService.LogError(
                    action: "CreatePerson",
                    message: ex.Message,
                    personName: name
                );

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = ex.StatusCode  // Use the exception's status code (400)
                });
            }
            catch (Exception ex)
            {
                // ADDED log exception first
                await _loggingService.LogException(
                    action: "CreatePerson",
                    ex: ex,
                    personName: name
                );

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }

        }

        // ADDED new PUT endpoint for UPDATE operations
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdatePerson(string name, [FromBody] string newName)
        {
            try
            {
                var result = await _mediator.Send(new UpdatePerson()
                {
                    CurrentName = name,      // From URL parameter
                    NewName = newName        // From request body
                });

                return this.GetResponse(result);
            }
            // ADDED 400 catch for expected errors
            catch (BadHttpRequestException ex)
            {
                await _loggingService.LogError(
                    action: "UpdatePerson",
                    message: ex.Message,
                    personName: name
                );

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = ex.StatusCode
                });
            }
            catch (Exception ex)
            {
                // ADDED log exception first
                await _loggingService.LogException(
                    action: "UpdatePerson",
                    ex: ex,
                    personName: name
                );

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}