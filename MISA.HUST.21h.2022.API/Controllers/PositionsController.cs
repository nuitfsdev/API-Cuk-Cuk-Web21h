using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21h._2022.API.Entities;
using MySqlConnector;

namespace MISA.HUST._21h._2022.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllPositions()
        {
            try
            {
                string valuesConnection = "Server=localhost;Port=3306;Database=uit.21h.2022.nnnam;Uid=root;Pwd=21082002;";
                var mySQLConnection = new MySqlConnection(valuesConnection);
                string getAllPositionsQuery = "SELECT * FROM positions;";
                var positions = mySQLConnection.Query<Position>(getAllPositionsQuery);
                if (positions != null)
                {
                    return StatusCode(StatusCodes.Status200OK, positions);
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
