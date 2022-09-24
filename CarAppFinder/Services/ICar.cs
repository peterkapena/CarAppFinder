using CarAppFinder.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;

namespace CarAppFinder.Services
{
    public interface ICarService
    {
        public Task<Car> AddCar(Car car);
        //public Task<Tracker> AddTracker(Tracker car);
        public Task<Coordinates> AddCoordinate(Coordinates coordinates);
        public Task<Car> GetCar(long Id);
        public Task<IEnumerable<Car>> GetCars(string userId);
        public Task DeleteCar(long Id);
        public Task UpdateCar(Car car);
    }

    public class CarService : ICarService
    {
        public DatabaseContext Context { get; set; }

        public CarService(DatabaseContext context)
        {
            Context = context;
        }
        public Task<List<Car>> Cars => Context.Cars.Where(x => !x.Archived).ToListAsync();
        public Task<List<Coordinates>> Coordinates => Context.Coordinates.Where(x => !x.Archived).ToListAsync();

        public async Task<Car> AddCar(Car c)
        {
            var car = new Car
            {
                Name = c.Name,
                TrackerId = c.TrackerId,
                UserId = c.UserId
            };

            //if (!(await Coordinates).Any(t => t.Id == c.TrackerId))
            //{
            //    await Context.Trackers.AddAsync(new Tracker { Id = c.TrackerId });
            //}

            car = (await Context.Cars.AddAsync(car)).Entity;
            await Context.SaveChangesAsync();

            return car;
        }

        public async Task<Car> GetCar(long Id)
        {
            var car = (await Cars).Where(x => x.Id == Id).FirstOrDefault();
            return car;
        }

        public async Task DeleteCar(long Id)
        {
            var car = (await Cars).Where(x => x.Id == Id).FirstOrDefault();
            if (car is null) throw new Exception("car not found");

            car.Archived = true;

            Context.Cars.Update(car);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateCar(Car car)
        {
            var carToUpdate = await Context.Cars.Where(x => x.Id == car.Id).FirstOrDefaultAsync(); //(await Cars).Where(x => x.Id == car.Id).FirstOrDefault();
            if (carToUpdate is null) throw new Exception("car not found");

            carToUpdate.Name = car.Name;

            Context.Cars.Update(carToUpdate);

            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Car>> GetCars(string userId)
        {
            return (await Cars).Where(x => x.UserId == userId);
        }

        public async Task<Coordinates> AddCoordinate(Coordinates coordinates)
        {
            //var tracker = (await Coordinates).Where(x => x.Id == t.Id).FirstOrDefault();

            //if (tracker is null)
            //{
            //    tracker = new Tracker { Id = t.Id, };
            //    //await Context.Trackers.AddAsync(tracker);
            //    await Context.SaveChangesAsync();
            //}


            return coordinates;
        }
    }
}
