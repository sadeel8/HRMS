using System.Linq;
using HRMS.DTO.Departments;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        public static List<DepartmentClass> Departments = new List<DepartmentClass>()
        {
            new DepartmentClass (){Id=101,Name="HR Department",Description="Employee affairs and recruitment.",FloorNumber=1},
             new DepartmentClass (){Id=102,Name="Payroll",Description="Salaries and compensation.",FloorNumber=1},
             new DepartmentClass (){Id=103,Name="IT Support",Description="Technical and system support.",FloorNumber=2},
              new DepartmentClass (){Id=104,Name="Meeting Room",Description="Interviews and meetings.",FloorNumber=2}

        };

        [HttpGet]
        public IActionResult GetByCriteria(SearchDepartmentDto SearchDepartmentDto)
        {
            var data1 = from Dep in Departments
                        where 
                        (SearchDepartmentDto.Name==null||Dep.Name.ToUpper().Contains(SearchDepartmentDto.Name.ToUpper()))&&
                        (SearchDepartmentDto.FloorNumber==null||Dep.FloorNumber==SearchDepartmentDto.FloorNumber)
                        orderby Dep.Id descending
                        select new DepartmentsDto
                        {
                            Name = Dep.Name,
                            FloorNumber = Dep.FloorNumber
                        };
            return Ok(data1);
        }
        [HttpGet]
        public IActionResult GetByld(long id)
        {
            var data1 = Departments.Select(x => new DepartmentsDto
            {
                Id = x.Id,
                Name = x.Name,
                FloorNumber = x.FloorNumber

            }).FirstOrDefault(x => x.Id == id);
            if(data1 == null)
            {
                return NotFound(new Exception("Employee not found"));
            }
            return Ok(data1);
        }
       


    }

  
}
