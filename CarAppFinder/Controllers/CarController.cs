using CarAppFinder.Models;
using CarAppFinder.Services;
using CarAppFinder.Services.Bug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAppFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : BaseAPI
    {
        #region 'Construcors'
        public CarController(UserManager<User> userMgr,
                              IErrorLogService errorLogService,
                              ICarService carService
                             )
         : base(userMgr: userMgr, errorLogService: errorLogService)
        {
            CarService = carService;
        }

        public ICarService CarService { get; }

        //[AllowAnonymous]
        #endregion.

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Car car)
        {
            try
            {
                await CarService.AddCar(car);
                SetReturnValue("car", new
                {
                    car.Id,
                    car.Name,
                    car.TrackerId,
                    car.UserId
                });
                return Ok(ReturnValue);
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }

        [HttpPost(nameof(AddCoordinate))]
        public async Task<ActionResult> AddCoordinate([FromBody] Coordinates coordinates)
        {
            try
            {
                await CarService.AddCoordinate(coordinates);

                return Ok(new
                {
                    coordinates.CarId,
                    coordinates.Coords
                });
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult> Get(long Id)
        {
            try
            {
                var car = await CarService.GetCar(Id);
                return Ok(car);
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }

        [HttpGet]
        [Route("[action]/{userId}")]
        public async Task<ActionResult> GetCars(string userId)
        {
            try
            {
                var cars = await CarService.GetCars(userId);
                return Ok(cars);
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<ActionResult> Delete(long Id)
        {
            try
            {
                await CarService.DeleteCar(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }
        [HttpPatch]
        public async Task<ActionResult> UpdateCar([FromBody] Car car)
        {
            try
            {
                await CarService.UpdateCar(car);
                return Ok(car);
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }
    }
}
