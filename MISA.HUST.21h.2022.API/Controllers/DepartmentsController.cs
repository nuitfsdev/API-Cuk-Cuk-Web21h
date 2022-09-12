using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21h._2022.API.Entities;
using MySqlConnector;

namespace MISA.HUST._21h._2022.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllDepartments()
        {

            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string getAllDepartmentsQuery = "SELECT * FROM department;";
                var departments = mySQLConnection.Query<Department>(getAllDepartmentsQuery);
                if (departments != null)
                {
                    return StatusCode(StatusCodes.Status200OK, departments);
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
