using Dapper;
using PopulateDB.Interfaces;
using PopulateDB.Models;

namespace PopulateDB.Services
{
    public class PopulateService(IDbConnectionFactory context) : IPopulateService
    {
        public async Task<User> Populate(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user), "Os dados não podem ser nulos.");

            const string sqlUser = @"INSERT INTO Users (Name, Username, Phone, Website, Email, AddressId)
                                     VALUES (@Name, @Username, @Phone, @Website, @Email, @AddressId)
                                     SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlAddress = @"INSERT INTO Addresses (Street, Suite, City, ZipCode, GeoId)
                                         VALUES (@Street, @Suite, @City, @ZipCode, @GeoId);
                                         SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlGeo = @"INSERT INTO Geo (Latitude, Longitude) 
                                    VALUES (@Lat, @Lng);
                                    SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = context.CreateConnection();

            try
            {
                var geoId = await connection.QueryFirstAsync<int>(sqlGeo, new
                {
                    user.Address.Geo.Lat,
                    user.Address.Geo.Lng
                });

                var addressId = await connection.QuerySingleAsync<int>(sqlAddress, new
                {
                    user.Address.Street,
                    user.Address.Suite,
                    user.Address.City,
                    user.Address.ZipCode,
                    geoId
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
