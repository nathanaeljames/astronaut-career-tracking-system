using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

/*
    NEW ENDPOINT: PUT /Person/{name}
    Why a new endpoint vs 'upsert' pattern with existing CreatePerson command?
    1. Follows RESTful API design principles where POST is for resource creation and PUT is for updates
    This makes the API predictable and follows HTTP standards that developers expect
    2. PUT is idempotent - calling it multiple times with the same data produces the same result
    POST is not idempotent - each call creates a new resource. This distinction is important for retry logic and error recovery
    3. Separate endpoints make the developer's intention clear
    When you call POST, you explicitly want to CREATE
    When you call PUT, you explicitly want to UPDATE
    This prevents accidental overwrites or unexpected behavior
*/

namespace StargateAPI.Business.Commands
{
    // Request - PUT /Person/{currentName} with newName in body
    public class UpdatePerson : IRequest<UpdatePersonResult>
    {
        public required string CurrentName { get; set; } = string.Empty;
        public required string NewName { get; set; } = string.Empty;
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;

        public UpdatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }

        // Preprocessor - validation BEFORE processing
        public Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            // CHECK 1: Current person must exist
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.CurrentName);

            if (person is null)
            {
                throw new BadHttpRequestException($"Person with name '{request.CurrentName}' not found", StatusCodes.Status404NotFound);
            }

            // CHECK 2: New name must not already exist (unless it's the same name)
            if (request.CurrentName != request.NewName)
            {
                var existingPerson = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.NewName);

                if (existingPerson is not null)
                {
                    throw new BadHttpRequestException($"Person with name '{request.NewName}' already exists", StatusCodes.Status409Conflict);
                }
            }

            return Task.CompletedTask;
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
    {
        private readonly StargateContext _context;

        public UpdatePersonHandler(StargateContext context)
        {
            _context = context;
        }

        // Handler - the actual update code
        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            // Find the person by current name (with tracking, so EF knows to update)
            var person = await _context.People.FirstOrDefaultAsync(z => z.Name == request.CurrentName, cancellationToken);

            if (person == null)
            {
                // This shouldn't happen because the preprocessor already checks, but defensive coding
                return new UpdatePersonResult
                {
                    Success = false,
                    Message = $"Person with name '{request.CurrentName}' not found",
                    ResponseCode = (int)HttpStatusCode.NotFound
                };
            }

            // Update the name
            person.Name = request.NewName;

            // Save changes to the database
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatePersonResult
            {
                Id = person.Id,
                Success = true,
                Message = $"Person updated from '{request.CurrentName}' to '{request.NewName}'",
                ResponseCode = (int)HttpStatusCode.OK
            };
        }
    }

    // Result - returns person's id (which doesn't change) and success/message from BaseResponse.
    public class UpdatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}