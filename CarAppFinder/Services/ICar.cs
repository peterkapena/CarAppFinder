using CarAppFinder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarAppFinder.Services
{
    public interface ICarService
    {
        public Task<Car> AddCar(Car car);
        public Task<Coordinates> AddCoordinate(Coordinates coordinates);
        public Task<Car> GetCar(string TrackerSerialNumber);
        public Task<IEnumerable<Car>> GetCars(string userId);
        public Task DeleteCar(string TrackerSerialNumber);
        public Task UpdateCar(Car car, string tsn);
        Task<Coordinates> GetRecentCoord(string tsn);
    }

    public class CarService : ICarService
    {
        public DatabaseContext Context { get; set; }

        public CarService(DatabaseContext context)
        {
            Context = context;
        }

        public async Task<Car> AddCar(Car car)
        {
            var existingCar = await Context.Cars.Where(c => c.TrackerSerialNumber == car.TrackerSerialNumber).FirstOrDefaultAsync();

            if (existingCar is not null)
                throw new InvalidDataException("This tracker has been already used.");

            car = (await Context.Cars.AddAsync(car)).Entity;
            await Context.SaveChangesAsync();

            return car;
        }

        public Task<Car> GetCar(string TrackerSerialNumber)
        {
            var car = Context.Cars.Where(x => x.TrackerSerialNumber == TrackerSerialNumber).FirstOrDefault();
            return Task.FromResult(car);
        }

        public async Task DeleteCar(string TrackerSerialNumber)
        {
            var car = Context.Cars.Where(x => x.TrackerSerialNumber == TrackerSerialNumber).FirstOrDefault();
            if (car is null) throw new InvalidDataException("car not found");

            Context.Cars.Remove(car);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateCar(Car car, string tsn)
        {
            if (car.TrackerSerialNumber == tsn) return;

            if (await Context.Cars.AnyAsync(c => c.TrackerSerialNumber == tsn)) throw new InvalidDataException("this tracker has already been used.");

            var carToUpdate = await Context.Cars.Where(x => x.TrackerSerialNumber == car.TrackerSerialNumber).FirstOrDefaultAsync();
            if (carToUpdate is null) throw new InvalidDataException("car not found");

            //update coordinates
            //var friends = Context.Coordinates.Where(coord => coord.CarTrackerSerialNumber == car.TrackerSerialNumber).ToList();
            //friends.ForEach(a => a.CarTrackerSerialNumber = tsn);
            //await Context.SaveChangesAsync();

            //update car CarTrackerSerialNumber
            carToUpdate.Coordinates.ForEach(a => a.CarTrackerSerialNumber = tsn);
            carToUpdate.TrackerSerialNumber = tsn;

            Context.Cars.Update(carToUpdate);

            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Car>> GetCars(string userId)
        {
            return await Context.Cars.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Coordinates> AddCoordinate(Coordinates x)
        {
            if (!await Context.Cars.AnyAsync(car => car.TrackerSerialNumber == x.CarTrackerSerialNumber))
                throw new InvalidDataException("no car found for this coord");

            await Context.Coordinates.AddAsync(x);
            await Context.SaveChangesAsync();
            return x;
        }

        public async Task<Coordinates> GetRecentCoord(string tsn)
        {
            if (tsn.IsNullOrEmpty()) throw new InvalidDataException("invalid serial number");

            var coord = await Context.Coordinates.Where(crd => crd.CarTrackerSerialNumber == tsn)
                .OrderByDescending(crd => crd.Time)
                .FirstOrDefaultAsync();
            if (coord is null) throw new InvalidDataException("no data received for this car yet");
            return coord;
        }
    }
}
