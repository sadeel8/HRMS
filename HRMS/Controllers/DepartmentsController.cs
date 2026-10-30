using System.Linq;
using HRMS.DbContexts;
using HRMS.DTO.Departments;
using HRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {

        private readonly HRMSContext _dbContext;
        public DepartmentsController(HRMSContext dbContext)
        {
            _dbContext = dbContext;
        }

       
        [HttpGet("search")]
        public IActionResult GetByCriteria(SearchDepartmentDto SearchDepartmentDto)
        {
            try
            {
                var data1 = from Dep in _dbContext.Departments
                            from Type in _dbContext.Lookups.Where(x=>x.Id==Dep.TypeId).DefaultIfEmpty()
                            where
                            (SearchDepartmentDto.Name == null || Dep.Name.ToUpper().Contains(SearchDepartmentDto.Name.ToUpper())) &&
                            (SearchDepartmentDto.FloorNumber == null || Dep.FloorNumber == SearchDepartmentDto.FloorNumber)
                            orderby Dep.Id descending
                            select new DepartmentsDto
                            {
                                Id = Dep.Id,
                                Name = Dep.Name,
                                Description = Dep.Description,
                                FloorNumber = Dep.FloorNumber,
                                TypeId = Dep.TypeId,
                                 TypeName=Type.Name
                            };
                return Ok(data1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
           
        }
        [HttpGet("{id}")]
        public IActionResult GetByld(long id)
        {
            try
            {
                var data1 = _dbContext.Departments.Select(x => new DepartmentsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    FloorNumber = x.FloorNumber,
                    TypeId = x.TypeId, 
                    TypeName=x.Type.Name


                }).FirstOrDefault(x => x.Id == id);
                if (data1 == null)
                {
                    return NotFound(new Exception("Employee not found"));
                }
                return Ok(data1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }
          
        }
        [HttpPost]
        public IActionResult Create([FromBody] SaveDepartmentDto departmentDto)
        {
            try
            {
                var department = new Department
                {
                    Id = 0,
                    Name = departmentDto.Name,
                    Description = departmentDto.Description,
                    FloorNumber = departmentDto.FloorNumber,
                    TypeId = departmentDto.TypeId
                };

                _dbContext.Departments.Add(department);
                _dbContext.SaveChanges();
                return Ok(department.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception(ex.Message));
            }

           
        }
        [HttpPut("{id:long}")]
        public IActionResult Update(long id, [FromBody] SaveDepartmentDto departmentDto)
        {
            try
            {
                if (id != departmentDto.Id)
                {
                    return BadRequest(new Exception("Id Mismatch"));
                }

                var department = _dbContext.Departments.FirstOrDefault(x => x.Id == departmentDto.Id);
                if (department == null)
                {
                    return NotFound(new Exception("Department Not Found"));
                }

                department.Name = departmentDto.Name;
                department.Description = departmentDto.Description;
                department.FloorNumber = departmentDto.FloorNumber;
                department.TypeId = departmentDto.TypeId;

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
                var department = _dbContext.Departments.FirstOrDefault(x => x.Id == id);
                if (department == null)
                {
                    return NotFound(new Exception("Department Not Found"));
                }

                var isEmployee = _dbContext.Employees.Any(x => x.DepartmentId == id);
                if (isEmployee)
                {
                    return BadRequest("Department with assigned employees can not be deleted");
                }


                _dbContext.Departments.Remove(department);
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
