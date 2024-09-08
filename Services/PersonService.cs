using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class PersonService : IPersonService
    {
        public readonly ApplicationDbContext _context;
        public PersonService( ApplicationDbContext context) { 
            _context = context;
           
        }
        public async Task<Person?> GetPersonById(string personId)
        {
            return await _context.Users.FirstAsync(u => u.Id == personId);
        }
        public async Task<List<Person>> GetPeople()
        {
            return await _context.Users.ToListAsync();
      
        }
    }
}
