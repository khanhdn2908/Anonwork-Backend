//using Anonwork.Application.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Anonwork.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]

//    public class UsersController : ControllerBase
//    {
//        private readonly IUserService _userService;

//        public UsersController(IUserService userService)
//        {
//            _userService = userService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var users = await _userService.GetAllAsync();
//            return Ok(users);
//        }
//    }
//}
