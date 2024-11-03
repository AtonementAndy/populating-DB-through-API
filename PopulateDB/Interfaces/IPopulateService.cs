using PopulateDB.Models;

namespace PopulateDB.Interfaces
{
    public interface IPopulateService
    {
        Task<User> Populate(User user);
    }
}
