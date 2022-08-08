using HospitalService.MessageConsumer.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalService.MessageConsumer.Data
{
    public class PayloadMessageContext : DbContext
    {
        private string _connectionString;
        public DbSet<Message> Messages { get; set; }

        public PayloadMessageContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
