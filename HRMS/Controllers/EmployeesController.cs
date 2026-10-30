using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using HRMS.DbContexts;
using HRMS.DTO.Employees;
using HRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Controllers
{

    //data annotation
    [Authorize]
    [Route("api/[controller]")] //appi/Employees
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //Dependcey Injuction

        private readonly HRMSContext _dbContext; 


        public EmployeesController(HRMSContext dbContext)
        {
            _dbContext = dbContext;
        }


        //Endpoints
        //    [HttpPost("GetEmployee")]
        //    public IActionResult Get()
        //    {
        //  .3
        //  return Ok(new {Name="Emp",Salary=1000}); //status:200 Ok
        //        //return BadRequest("data not loaded"); //status:400 Bad Request
        //        //return NotFound("Employee Not Found"); //status:404 Not found
        //        //status :500 intrenal error
        //       // return StatusCode(500, "intrenal error");

        //    }
        //    [HttpGet("GetEmployee")]
        //    public IActionResult GetEmployee()
        //    {
        //        return Ok();
        //    }
        //}


        [HttpGet]
        public IActionResult GetByCriteria([FromQuery]SearchEmployeeDto SearchEmployeeDto)
        {
            try
            {
                //Extracted From Token
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                var data = from emp in _dbContext.Employees
                           from dep in _dbContext.Departments.Where(x => x.Id == emp.DepartmentId).DefaultIfEmpty()
                           from manager in _dbContext.Employees.Where(x => x.Id == emp.ManagerId).DefaultIfEmpty()
                           from lookup in _dbContext.Lookups.Where(x => x.Id == emp.PositionId).DefaultIfEmpty()
                           where
                           (SearchEmployeeDto.PositionId == null || emp.PositionId == SearchEmployeeDto.PositionId) &&
                           (SearchEmployeeDto.Name == null || emp.FirstName.ToUpper().Contains(SearchEmployeeDto.Name.ToUpper())) &&
                           (SearchEmployeeDto.IsActive == null || emp.IsActive == SearchEmployeeDto.IsActive)
                           orderby emp.Id descending
                           select new EmployeeDto
                           {
                               Id = emp.Id,
                               FullName = emp.FirstName + " " + emp.LastName,
                               PositionId = emp.PositionId,
                               PositionName = lookup.Name,
                               BirthDate = emp.BirthDate,
                               StartDate = emp.StartDate,
                               EndDate = emp.EndDate,
                               Salary = emp.Salary,
                               DepartmentId = emp.DepartmentId,//dep.Id ,
                               DepartmentName = dep.Name,
                               ManagerId = emp.ManagerId, //manager.Id
                               ManagerName = manager.FirstName + "  " + manager.LastName,
                               UserId =emp .UserId 
                           };

                if(role?.ToUpper()!="Admin"&& role?.ToUpper() != "HR")
                {
                    data = data.Where(x=>x.UserId==long .Parse(userId));
                }
                return Ok(data);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
           
        }

        [HttpGet("{id:long}")]//Rout Parameter
        public IActionResult GetById(long id)//ترجع موظف واحد فقط
        {
            try
            {
                var data = _dbContext.Employees.Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    PositionId = x.PositionId,
                    PositionName = x.Lookup.Name,
                    BirthDate = x.BirthDate,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Salary = x.Salary,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Name,
                    ManagerId = x.ManagerId,
                    ManagerName = x.FirstName + "  " + x.LastName

                }).FirstOrDefault(x => x.Id == id);

                //var data = _dbContext.Employees.Include(x => x.Department).FirstOrDefault(x => x.Id == id).Department.Name;
                if (data == null)
                {
                    return NotFound(new Exception("Employee not found"));
                }
                return Ok(data);
            }   
            catch(Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));

            }
           
        }

        [Authorize(Roles = "HR,Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] SaveEmployeeDto employeeDto)// انشاء موظف جديد
        {
            try
            {

                var user = new User()
                {
                    Id = 0,
                    Username= $"{employeeDto.FirstName}_{employeeDto .LastName}_HRMS",
                    HashedPassword=BCrypt.Net.BCrypt.HashPassword($"{employeeDto.FirstName}@123"),
                    IsAdmin =false
                };
                var isDuplicated = _dbContext.Users.Any(x => x.Username.ToUpper() == user.Username.ToUpper());
                if(isDuplicated)
                {
                    return BadRequest(new Exception("Cannot Add Employee:Already Exist"));
                }
                _dbContext.Users.Add(user);


                var employee = new Employee()

                {
                    Id = 0,//(employees.LastOrDefault()?.Id ?? 0) + 1,
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    PositionId = employeeDto.PositionId,
                    BirthDate = employeeDto.BirthDate,
                    StartDate = employeeDto.StartDate,
                    EndDate = employeeDto.EndDate,
                    Email = employeeDto.Email,
                    IsActive = employeeDto.IsActive,
                    PhoneNumber = employeeDto.PhoneNumber,
                    Salary = employeeDto.Salary,
                    DepartmentId = employeeDto.DepartmentId,
                    ManagerId = employeeDto.ManagerId,
                    //UserId=user .Id
                    User=user
                };
                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges(); //Go's to database
                return Ok(employee.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));

            }

           
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpPut]
        public IActionResult Update(long id,[FromBody] SaveEmployeeDto employeeDto)
        {
            try
            {
                if (id != employeeDto.Id)
                {
                    return BadRequest(new Exception("Id Mismatch"));
                }
                var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == employeeDto.Id);
                if (employee == null)
                {
                    return NotFound(new Exception("Employee not found"));
                }
                employee.FirstName = employeeDto.FirstName;
                employee.LastName = employeeDto.LastName;
                employee.PositionId = employeeDto.PositionId;
                employee.StartDate = employeeDto.StartDate;
                employee.EndDate = employeeDto.EndDate;
                employee.Email = employeeDto.Email;
                employee.IsActive = employeeDto.IsActive;
                employee.PhoneNumber = employeeDto.PhoneNumber;
                employee.Salary = employeeDto.Salary;
                employee.DepartmentId = employeeDto.DepartmentId;
                employee.ManagerId = employeeDto.ManagerId;

                _dbContext.SaveChanges();
                return Ok();

            }

            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
           

        }
        [Authorize(Roles = "HR,Admin")]
        [HttpDelete]
        public IActionResult Delete(long id)

        {
            try
            {
                var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    return NotFound(new Exception("Employee not found"));

                }
                _dbContext.Employees.Remove(employee);
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
   
   


