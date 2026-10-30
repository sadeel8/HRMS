namespace HRMS.DTO.Employees
{
    public class SaveEmployeeDto
    {
        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public long? PositionId { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Salary { get; set; }
        public long? DepartmentId { get; set; }
        public long? ManagerId { get; set; }

    }
}
