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

                return Ok(new
                {
                    car.Name,
                    car.TrackerSerialNumber,
                    car.UserId
                });
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }

        [HttpPost(nameof(AddCoordinate))]
        public async Task<ActionResult> AddCoordinate([FromBody] Coordinates coord)
        {
            try
            {
                await CarService.AddCoordinate(coord);

                return Ok(new
                {
                    coord.CarTrackerSerialNumber,
                    coord.Coords,
                    coord.Time
                });
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }

        [HttpGet]
        [Route("{tsn}")]
        public async Task<ActionResult> Get(string tsn)
        {
            try
            {
                var car = await CarService.GetCar(tsn);
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
        [Route("{tsn}")]
        public async Task<ActionResult> Delete(string tsn)
        {
            try
            {
                await CarService.DeleteCar(tsn);
                return Ok();
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex);
            }
        }
        //[HttpPatch]
        //[Route("{tsn}")]
        //public async Task<ActionResult> UpdateCar([FromBody] Car car, string tsn)
        //{
        //    try
        //    {
        //        await CarService.UpdateCar(car, tsn);
        //        return Ok(car);
        //    }
        //    catch (Exception ex)
        //    {
        //        return await GetErrorMessageResponse(ex);
        //    }
        //}
    }
}
