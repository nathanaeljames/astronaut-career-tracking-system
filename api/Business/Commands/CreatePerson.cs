using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        public CreatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            // VALIDATION 1: check if name is null/empty/whitespace
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BadHttpRequestException("Name cannot be null or empty");
            }
            // VALIDATION 2: check if person already exists (duplicate)
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            //if (person is not null) throw new BadHttpRequestException("Bad Request");
            if (person is not null) throw new BadHttpRequestException($"Person with name '{request.Name}' already exists");

            return Task.CompletedTask;
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService; //ADDED logging service

        public CreatePersonHandler(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService; // ADDED logging service
        }
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {

                var newPerson = new Person()
                {
                   Name = request.Name
                };

                await _context.People.AddAsync(newPerson);

                await _context.SaveChangesAsync();

                // ADDED log success
                await _loggingService.LogSuccess(
                    action: "CreatePerson",
                    message: $"Person '{request.Name}' created successfully with ID {newPerson.Id}",
                    personName: request.Name
                );

                return new CreatePersonResult()
                {
                    Id = newPerson.Id
                };
          
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
