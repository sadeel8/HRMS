using HRMS.DbContexts;
using HRMS.DTO.Vacations;
using HRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VacationsController : ControllerBase
    {
        private readonly HRMSContext _dbContext;

        public VacationsController(HRMSContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetByCriteria([FromQuery] VacationsFilterDto filter)
        {
            try
            {
                var data = from vac in _dbContext.Vacations
                           from emp in _dbContext.Employees.Where(x => x.Id == vac.EmployeeId).DefaultIfEmpty()
                           from type in _dbContext.Lookups.Where(x => x.Id == vac.TypeId).DefaultIfEmpty()
                           where
                           (filter.VacationTypeId == null || vac.TypeId == filter.VacationTypeId) &&
                           (filter.EmployeeId == null || vac.EmployeeId == filter.EmployeeId)
                           orderby vac.Id descending
                           select new VacationDto
                           {
                               Id = vac.Id,
                               EmployeeId = vac.EmployeeId,
                               EmployeeName = emp.FirstName + " " + emp.LastName,
                               CreationDate = vac.CreationDate,
                               StartDate = vac.StartDate,
                               EndDate = vac.EndDate,
                               TypeId = vac.TypeId,
                               TypeName = type.Name,
                               Notes = vac.Notes
                           };

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public IActionResult GetById(long id)
        {
            try
            {
                var data = (from vac in _dbContext.Vacations
                            from emp in _dbContext.Employees.Where(x => x.Id == vac.EmployeeId).DefaultIfEmpty()
                            from type in _dbContext.Lookups.Where(x => x.Id == vac.TypeId).DefaultIfEmpty()
                            where vac.Id == id
                            select new VacationDto
                            {
                                Id = vac.Id,
                                EmployeeId = vac.EmployeeId,
                                EmployeeName = emp.FirstName + " " + emp.LastName,
                                CreationDate = vac.CreationDate,
                                StartDate = vac.StartDate,
                                EndDate = vac.EndDate,
                                TypeId = vac.TypeId,
                                TypeName = type.Name,
                                Notes = vac.Notes
                            }).FirstOrDefault();

                if (data == null)
                {
                    return NotFound(new Exception("Vacation not found"));
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] SaveVacationDto vacationDto)
        {
            try
            {
                var vacation = new Vacation
                {
                    Id = 0,
                    StartDate = vacationDto.StartDate,
                    EndDate = vacationDto.EndDate,
                    Notes = vacationDto.Notes,
                    EmployeeId = vacationDto.EmployeeId,
                    TypeId = vacationDto.TypeId
                };

                _dbContext.Vacations.Add(vacation);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public IActionResult Update(long id, [FromBody] SaveVacationDto vacationDto)
        {
            try
            {
                if (id != vacationDto.Id)
                {
                    return BadRequest(new Exception("Id Mismatch"));
                }

                var vacation = _dbContext.Vacations.FirstOrDefault(x => x.Id == vacationDto.Id);
                if (vacation == null)
                {
                    return NotFound(new Exception("Vacation not found"));
                }

                vacation.StartDate = vacationDto.StartDate;
                vacation.EndDate = vacationDto.EndDate;
                vacation.Notes = vacationDto.Notes;
                vacation.EmployeeId = vacationDto.EmployeeId;
                vacation.TypeId = vacationDto.TypeId;

                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var vacation = _dbContext.Vacations.FirstOrDefault(x => x.Id == id);
                if (vacation == null)
                {
                    return NotFound(new Exception("Vacation not found"));
                }

                _dbContext.Vacations.Remove(vacation);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
        }
    }
}
