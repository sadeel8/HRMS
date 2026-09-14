using System.Globalization;
using HRMS.DTO.Employees;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    //data annotation
    [Route("api/[controller]")] //appi/Employees
    [ApiController]
    public class EmployeesController : ControllerBase
    {     //Mockup Date
        public static List<Employee> employees = new List<Employee>()
        {


                new Employee(){ Id = 1, FirstName = "Ahmad", LastName = "Nasser", Email = "Ahmad@123.com", Position = "Developer", BirthDate = new DateTime(1995,1,25), PhoneNumber = "+9627516848", IsActive = true, StartDate = new DateTime(), Salary = 1000},
                new Employee(){ Id = 2, FirstName = "Layla", LastName = "Kareem", Email = "Layla@123.com", Position = "HR", BirthDate = new DateTime(2000,1,25), PhoneNumber = "+9625588625", IsActive = true, StartDate = new DateTime(2026, 1, 1), Salary = 1000},
                new Employee(){ Id = 3, FirstName = "Yousef", LastName = "Faris", Email = "Yousef@123.com", Position = "Manager", BirthDate = new DateTime(1996,1,25), PhoneNumber = "+9625588625", IsActive = true, StartDate = new DateTime(2026, 1, 1), Salary = 1200},
                new Employee(){ Id = 4, FirstName = "Nadia", LastName = "Zaid", Email = "Nadia@123.com", Position = "Developer", BirthDate = new DateTime(1999,1,25), PhoneNumber = "+9625588625", IsActive = true, StartDate = new DateTime(2026, 1, 1), Salary = 800}

        };





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
            

            var data = from emp in employees
                       where
                       (SearchEmployeeDto.Position == null || emp.Position.ToUpper().Contains(SearchEmployeeDto.Position.ToUpper()))&&
                       (SearchEmployeeDto.Name==null|| emp.FirstName.ToUpper().Contains(SearchEmployeeDto.Name.ToUpper()))&&
                       (SearchEmployeeDto.IsActive==null||emp.IsActive== SearchEmployeeDto.IsActive )
                       orderby emp.Id descending
                       select new EmployeeDto
                       {
                           Id = emp.Id,
                           FullName = emp.FirstName + " " + emp.LastName,
                           Position = emp.Position,
                           BirthDate = emp.BirthDate,
                           StartDate = emp.StartDate,
                           EndDate = emp.EndDate,
                           Salary = emp.Salary,
                       };

            return Ok(data);


        }

        [HttpGet("{id:long}")]//Rout Parameter
        public IActionResult GetById(long id)//ترجع موظف واحد فقط
        {
            // var data = employees.Where(x => x.Id == id)
            var data = employees.Select(x => new EmployeeDto
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                Position = x.Position,
                BirthDate = x.BirthDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Salary = x.Salary,
            }).FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return NotFound(new Exception("Employee not found"));
            }
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Create([FromBody] SaveEmployeeDto employeeDto)// انشاء موظف جديد
        {
            var employee = new Employee()

            {
                Id = (employees.LastOrDefault()?.Id ?? 0) + 1,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Position = employeeDto.Position,
                BirthDate = employeeDto.BirthDate,
                StartDate = employeeDto.StartDate,
                EndDate = employeeDto.EndDate,
                Email = employeeDto.Email,
                IsActive = employeeDto.IsActive,
                PhoneNumber = employeeDto.PhoneNumber,
                Salary = employeeDto.Salary,
            };
            employees.Add(employee);
            return Ok(employee.Id);
        }

        [HttpPut]
        public IActionResult Update(long id,[FromBody] SaveEmployeeDto employeeDto)
        {
            var employee = employees.FirstOrDefault(x => x.Id == employeeDto.Id);
            if (employee == null)
            {
                return NotFound(new Exception("Employee not found"));
            }
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Position = employeeDto.Position;
            employee.StartDate = employeeDto.StartDate;
            employee.EndDate = employeeDto.EndDate;
            employee.Email = employeeDto.Email;
            employee.IsActive = employeeDto.IsActive;
            employee.PhoneNumber = employeeDto.PhoneNumber;
            employee.Salary = employeeDto.Salary;
            return Ok();

        }
        [HttpDelete]
        public IActionResult Delete(long id)

        {
            var employee = employees.FirstOrDefault(x => x.Id == id);
            if (employee ==null)
            {
                return NotFound(new Exception("employee not found"));

            }
            employees.Remove(employee);
            return Ok();
        }


    }
}
   
   


