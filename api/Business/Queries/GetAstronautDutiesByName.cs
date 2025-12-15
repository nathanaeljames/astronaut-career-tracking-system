using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {

            var result = new GetAstronautDutiesByNameResult();

            //var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE \'{request.Name}\' = a.Name";
            //var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query);
            // FIXED: Use parameterized query instead of string interpolation
            var query = @"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate 
                          FROM [Person] a 
                          LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id 
                          WHERE a.Name = @Name";
            var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query, new { Name = request.Name });

            // FIXED: make sure the person exists first!
            if (person == null)
            {
                return new GetAstronautDutiesByNameResult
                {
                    Success = false,
                    Message = $"Person with name '{request.Name}' not found",
                    ResponseCode = 404
                };
            }

            result.Person = person;

            //query = $"SELECT * FROM [AstronautDuty] WHERE {person.PersonId} = PersonId Order By DutyStartDate Desc";
            //var duties = await _context.Connection.QueryAsync<AstronautDuty>(query);
            // FIXED: Use parameterized query
            query = "SELECT * FROM [AstronautDuty] WHERE PersonId = @PersonId ORDER BY DutyStartDate DESC";
            var duties = await _context.Connection.QueryAsync<AstronautDuty>(query, new { PersonId = person.PersonId });

            result.AstronautDuties = duties.ToList();

            return result;

        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
