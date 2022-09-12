using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21h._2022.API.Entities;
using MySqlConnector;
using System;


namespace MISA.HUST._21h._2022.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// API lấy tất cả nhân viên
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllEmployees()
        {

            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string getAllEmployeesQuery = "SELECT * FROM employee;";
                var employees = mySQLConnection.Query(getAllEmployeesQuery);
                if(employees != null)
                {
                    return StatusCode(StatusCodes.Status200OK, employees);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }

        /// <summary>
        /// API lấy một nhân viên theo ID
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{employeeID}")]
        public IActionResult GetEmplyeeByID([FromRoute] Guid employeeID)
        {
            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string getEmployeeByIDQuery = "SELECT * FROM employee WHERE EmployeeID=@EmployeeID";

                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);

                var employees = mySQLConnection.Query(getEmployeeByIDQuery,parameters);
                if (employees != null)
                {
                    return StatusCode(StatusCodes.Status200OK, employees);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }

        /// <summary>
        /// API lọc nhân viên
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="positionID"></param>
        /// <param name="departmentID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("filters")]
        public IActionResult FilterEmployees(
         [FromQuery] string? keyword,
         [FromQuery] Guid? positionID,
         [FromQuery] Guid? departmentID,
         [FromQuery] int pageSize=10,
         [FromQuery] int pageNumber=1
        )
        {
            try {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);

                var parameters= new DynamicParameters();
                parameters.Add("@v_Offset",(pageNumber-1)*pageSize);
                parameters.Add("@v_Limit", pageSize);
                parameters.Add("@v_Sort", "ModifiedDate DESC");
                
                var orString=new List<string>();
                var andString= new List<string>();
                string whereString = "";
                if (keyword != null)
                {
                    orString.Add($"EmployeeCode LIKE '%{keyword}%'");
                    orString.Add($"EmployeeName LIKE '%{keyword}%'");
                    orString.Add($"PhoneNumber LIKE '%{keyword}%'");


                }
                if(orString.Count > 0)
                {
                    whereString = $"({string.Join(" OR ", orString)})";
                }
                if(positionID != null)
                {
                    andString.Add($"PositionID LIKE '%{positionID}%'");
                }
                if(departmentID != null)
                {
                    andString.Add($"DepartmentID LIKE '%{departmentID}%'");
                }
                if(andString.Count > 0)
                {
                    whereString += $"({string.Join(" AND ", andString)})";
                }
                parameters.Add("@v_Where",whereString);
                var result = mySQLConnection.QueryMultiple("Proc_employee_GetPaging", parameters, commandType: System.Data.CommandType.StoredProcedure);
                if(result != null)
                {
                    var employees = result.Read<Employee>().ToList();
                    var totalCount = result.Read<long>().Single();
                    return StatusCode(StatusCodes.Status200OK, new PagingData()
                    {
                        Data = employees,
                        TotalCount = employees.Count,
                    }

                   );
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }

        /// <summary>
        /// API thêm 1 nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertEmployee([FromBody] Employee employee)
        {
            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string insertEmployeeQuery = "INSERT INTO employee (EmployeeID, EmployeeCode, EmployeeName, DateOfBirth, Gender, IdentityNumber, IdentityDate, IdentityPlace, Email, PhoneNumber, PositionID, PositionName, DepartmentID, DepartmentName, PersonalTaxCode, Salary, JoinDate, WorkStatus, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)" +
                    " VALUES (@EmployeeID, @EmployeeCode, @EmployeeName, @DateOfBirth, @Gender, @IdentityNumber, @IdentityDate, @IdentityPlace, @Email, @PhoneNumber, @PositionID, @PositionName, @DepartmentID, @DepartmentName, @PersonalTaxCode, @Salary, @JoinDate, @WorkStatus, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy)";

                var employeeID = Guid.NewGuid();
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityDate", employee.IdentityDate);
                parameters.Add("@IdentityPlace", employee.IdentityPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PositionID", employee.PositionID);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@PersonalTaxCode", employee.PersonalTaxCode);
                parameters.Add("@Salary", employee.Salary);
                parameters.Add("@JoinDate", employee.JoinDate);
                parameters.Add("@WorkStatus", employee.WorkStatus);
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                int numberRowAffected = mySQLConnection.Execute(insertEmployeeQuery, parameters);

                if(numberRowAffected > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }

        /// <summary>
        /// API cập nhật nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{employeeID}")]
        public IActionResult UpdateEmployee([FromBody] Employee employee, [FromRoute] Guid employeeID)
        {
            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string updateEmployeeIDQuery = "UPDATE employee " +
                    " SET EmployeeCode=@EmployeeCode, EmployeeName=@EmployeeName, DateOfBirth=@DateOfBirth, Gender=@Gender, IdentityNumber=@IdentityNumber, IdentityDate=@IdentityDate, IdentityPlace=@IdentityPlace, Email=@Email, PhoneNumber=@PhoneNumber, PositionID=@PositionID, PositionName=@PositionName, DepartmentID=@DepartmentID, DepartmentName=@DepartmentName, PersonalTaxCode=@PersonalTaxCode, Salary=@Salary, JoinDate=@JoinDate, WorkStatus=@WorkStatus, CreatedDate=@CreatedDate, CreatedBy=@CreatedBy, ModifiedDate=@ModifiedDate, ModifiedBy=@ModifiedBy " +
                    " WHERE EmployeeID=@EmployeeID;";
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityDate", employee.IdentityDate);
                parameters.Add("@IdentityPlace", employee.IdentityPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PositionID", employee.PositionID);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@PersonalTaxCode", employee.PersonalTaxCode);
                parameters.Add("@Salary", employee.Salary);
                parameters.Add("@JoinDate", employee.JoinDate);
                parameters.Add("@WorkStatus", employee.WorkStatus);
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                int numberRowAffected = mySQLConnection.Execute(updateEmployeeIDQuery, parameters);

                if (numberRowAffected > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }

        /// <summary>
        /// API xóa nhân viên theo ID
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{employeeID}")]
        public IActionResult DeleteEmployee([FromRoute] Guid employeeID)
        {

            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string deleteEmployeeByIDQuery = "DELETE FROM employee WHERE EmployeeID=@EmployeeID";
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                
                int numberRowAffected = mySQLConnection.Execute(deleteEmployeeByIDQuery, parameters);

                if (numberRowAffected > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }

        /// <summary>
        /// API tạo mã code nhân viên mới
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("newEmployeeCode")]
        public IActionResult GetNewEmployeeCode()
        {
            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string getLastEmployeeCode= "SELECT MAX(DISTINCT (EmployeeCode)) FROM employee;";
                var lastEmployeeCode = mySQLConnection.Query<string>(getLastEmployeeCode).ToList();
                string lastEmployeeCodetmp = "NV10000";
                if (lastEmployeeCode != null)
                {
                    lastEmployeeCode.ForEach(code => lastEmployeeCodetmp = code);
                    lastEmployeeCodetmp = lastEmployeeCodetmp.Substring(2);
                    int numberCode = int.Parse(lastEmployeeCodetmp) + 1;
                    lastEmployeeCodetmp = "NV" + numberCode.ToString();
                    return StatusCode(StatusCodes.Status200OK, lastEmployeeCodetmp);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "ERROR01");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "ERROR02");
            }
        }
    }
}
