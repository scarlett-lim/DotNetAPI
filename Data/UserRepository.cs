using DotnetAPI.Model;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;

        // Contructor name must follow file name
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);

        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
                _entityFramework.Add(entityToAdd);
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
                _entityFramework.Remove(entityToRemove);
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }

        public User GetSingleUserFromDatabase(int userID)
        {
            //query Users from db
            User? user = _entityFramework.Users
                // filter by PK UserId, u is the variable name, represent every _entityFramework.Users in the Users db
                .Where(u => u.UserId == userID)
                // This method returns the first user that matches the condition or null if no match is found.
                .FirstOrDefault<User>();

            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to Get User");
        }


    }
}