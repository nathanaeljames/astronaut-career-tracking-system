using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            // VALIDATION 1: Check required fields
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BadHttpRequestException("Person name cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.Rank))
            {
                throw new BadHttpRequestException("Rank cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.DutyTitle))
            {
                throw new BadHttpRequestException("Duty title cannot be null or empty");
            }

            // VALIDATION 2: Check if person exists
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);
            //if (person is null) throw new BadHttpRequestException("Bad Request");
            if (person is null) throw new BadHttpRequestException($"Person with name '{request.Name}' not found");

            // VALIDATION 3: Check for duplicate duty
            var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);
            //if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException("Bad Request");
            if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException($"Duty '{request.DutyTitle}' with start date {request.DutyStartDate:yyyy-MM-dd} already exists");

            return Task.CompletedTask;
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService; //ADDED logging service

        public CreateAstronautDutyHandler(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService; //ADDED logging service
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {

            //var query = $"SELECT * FROM [Person] WHERE \'{request.Name}\' = Name";
            //var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);
            // FIXED: String interpolation opens us up to SQL injection attacks. Use parameterized queries with anonymous objects instead
            // Dapper automatically sanitizes and escapes parameters
            var query = "SELECT * FROM [Person] WHERE Name = @Name";
            var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query, new { Name = request.Name });

            //query = $"SELECT * FROM [AstronautDetail] WHERE {person.Id} = PersonId";
            //var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query);
            // FIXED: Use parameterized query
            query = "SELECT * FROM [AstronautDetail] WHERE PersonId = @PersonId";
            var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query, new { PersonId = person.Id });

            if (astronautDetail == null)
            {
                astronautDetail = new AstronautDetail();
                astronautDetail.PersonId = person.Id;
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;
                astronautDetail.CareerStartDate = request.DutyStartDate.Date;
                if (request.DutyTitle == "RETIRED")
                {
                    //astronautDetail.CareerEndDate = request.DutyStartDate.Date;
                    // FIXED: Career end date should be day BEFORE retirement (Rule 7) - already correct in else clause
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                await _context.AstronautDetails.AddAsync(astronautDetail);

            }
            else
            {
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;
                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }
                _context.AstronautDetails.Update(astronautDetail);
            }

            //query = $"SELECT * FROM [AstronautDuty] WHERE {person.Id} = PersonId Order By DutyStartDate Desc";
            //var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query);
            // FIXED: Use parameterized query
            query = "SELECT * FROM [AstronautDuty] WHERE PersonId = @PersonId ORDER BY DutyStartDate DESC";
            var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query, new { PersonId = person.Id });

            if (astronautDuty != null)
            {
                astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(astronautDuty);
            }

            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty);

            await _context.SaveChangesAsync();

            // ADDED log success
            await _loggingService.LogSuccess(
                action: "CreateAstronautDuty",
                message: $"Astronaut duty '{request.DutyTitle}' created for '{request.Name}' starting {request.DutyStartDate:yyyy-MM-dd}",
                personName: request.Name
            );

            return new CreateAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
