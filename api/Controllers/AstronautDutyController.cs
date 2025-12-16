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
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggingService _loggingService; // ADDED logging service
        public AstronautDutyController(IMediator mediator, ILoggingService loggingService)
        {
            _mediator = mediator;
            _loggingService = loggingService; // ADDED logging service
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                //var result = await _mediator.Send(new GetPersonByName()
                //FIXED: Wrong handler we want to return person info AND duties list 
                var result = await _mediator.Send(new GetAstronautDutiesByName()
                {
                    Name = name
                });

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // ADDED log exception first
                await _loggingService.LogException(
                    action: "GetAstronautDutiesByName",
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

        /*[HttpPost("")]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
        {
                var result = await _mediator.Send(request);
                return this.GetResponse(result);           
        }*/

        // MODIFIED original method to include proper try/catch exception handling and logging
        [HttpPost("")]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return this.GetResponse(result);
            } // INCLUDED 400 error catch for expected errors
            catch (BadHttpRequestException ex)
            {
                await _loggingService.LogError(
                    action: "CreateAstronautDuty",
                    message: ex.Message,
                    personName: request.Name
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
                // ADDED log exceptions first
                await _loggingService.LogException(
                    action: "CreateAstronautDuty",
                    ex: ex,
                    personName: request.Name
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