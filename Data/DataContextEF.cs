using DotnetAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

        public virtual DbSet<UserSalary> UserSalary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if the connection string is yet to set up
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                //tell entityframework to connect to sqlserver for the given connection string
                    .UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    //retry if connection to db fail
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            //Initialize entity
            modelBuilder.Entity<User>()
                //Set the table we map the entity to
                .ToTable("Users", "TutorialAppSchema")
                //by providing the userid
                .HasKey(u => u.UserId);

            //Initialize entity
            modelBuilder.Entity<UserJobInfo>()
                //provide the userid
                .HasKey(u => u.UserId);

            //Initialize entity
            modelBuilder.Entity<UserSalary>()
                //provide the userid
                .HasKey(u => u.UserId);
        }



    }
}