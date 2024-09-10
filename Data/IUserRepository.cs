using DotnetAPI.Model;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();

        public void AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<User> GetUsers();

        public User GetSingleUserFromDatabase(int userID);

    }
}