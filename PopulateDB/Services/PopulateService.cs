using Dapper;
using PopulateDB.Interfaces;
using PopulateDB.Models;

namespace PopulateDB.Services
{
    public class PopulateService : IPopulateService
    {
        private readonly IDbConnectionFactory _context;

        public PopulateService(IDbConnectionFactory context)
        {
            _context = context;
        }
        public async Task<User> Populate(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            const string sqlUser = @"INSERT INTO Users (Name, Username, Phone, Website, Email, AddressId)
                                     VALUES (@Name, @Username, @Phone, @Website, @Email, @AddressId)
                                     SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlAddress = @"INSERT INTO Addresses (Street, Suite, City, ZipCode, GeoId)
                                         VALUES (@Street, @Suite, @City, @ZipCode, @GeoId);
                                         SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlGeo = @"INSERT INTO Geo (Latitude, Longitude) 
                                    VALUES (@Lat, @Lng);
                                    SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();

            try
            {
                var geoId = await connection.QueryFirstAsync<int>(sqlGeo, new
                {
                    Lat = user.Address.Geo.Lat,
                    Lng = user.Address.Geo.Lng
                });

                var addressId = await connection.QuerySingleAsync<int>(sqlAddress, new
                {
                    Street = user.Address.Street,
                    Suite = user.Address.Suite,
                    City = user.Address.City,
                    ZipCode = user.Address.ZipCode,
                    GeoId = geoId
                });

                user.Id = await connection.QuerySingleAsync<int>(sqlUser, new
                {
                    user.Name,
                    user.Username,
                    user.Phone,
                    user.Website,
                    user.Email,
                    AddressId = addressId
                });

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
