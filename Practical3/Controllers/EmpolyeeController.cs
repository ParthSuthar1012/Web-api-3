using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using practical1.Models;
using practical1.Models.Models;
using Repository.Repository.IRepository;

namespace Practical3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpolyeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmpolyeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize]
        public IActionResult GetAllEmployee()
        {
            var employee = _unitOfWork.empolyeeRepository.GetAll();

            return Ok(employee);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult AddEmpolyee([FromBody] Empolyee empolyee)
        {
            _unitOfWork.empolyeeRepository.Add(empolyee);
            _unitOfWork.Save();
            return StatusCode(StatusCodes.Status201Created,
                           new Responce { Status = "Success", Message = "Empolyee Added Successfully" });
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _unitOfWork.empolyeeRepository.GetFirstOrDefault(a => a.Id == id);

            if(employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }
    }
}
