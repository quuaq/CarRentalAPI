using CarRentalAPI.Entities;
using CarRentalAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using CarRentalAPI.DTO;


namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Context _context;

        public UsersController(Context context)
        {
            _context = context;
        }

        ////1. Get: api/Users ==> Tüm kullanıcıları listeler
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        ////2. Get: api/Users/5 ==> Belirli bir kullanıcıyı ID'ye göre getirir
        //[HttpGet("{id}")]
        //public async Task<ActionResult<User>> GetUser(int id)
        //{
        //    var user = await _context.Users.FindAsync(id);

        //    if(user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.Include(u => u.Role)
                                            .Select(u => new UserDTO
                                            {
                                                User_ID = u.User_ID,
                                                FirstName = u.FirstName,
                                                LastName = u.LastName,
                                                Email = u.Email,
                                                PhoneNumber = u.PhoneNumber,
                                                RoleName = u.Role != null ? u.Role.RoleName : "No Role Assigned"
                                            }).ToListAsync();

            return users;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .Select(u => new UserDTO
                                           {
                                               User_ID = u.User_ID,
                                               FirstName = u.FirstName,
                                               LastName = u.LastName,
                                               Email = u.Email,
                                               PhoneNumber = u.PhoneNumber,
                                               RoleName = u.Role.RoleName
                                           })
                                           .FirstOrDefaultAsync(u => u.User_ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        //3. Post: api/Users ==> Yeni kullanıcı eklemek için
        [HttpPost]
        public async  Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.User_ID }, user);
        }

        //4. Put: api/Users/5 ==> Mevcut kullanıcıyı güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if(id!=user.User_ID)
            {
                return BadRequest("Güncellenmek istenen kullanıcı ID'sine ulaşılamıyor!");
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        //5. Delete: api/Users/5 ==> İstenilen bir kullanıcıyı silme işlemi
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user==null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //UserExist metodu kullanıcının var olup olmadığını kontrol eder
        private bool UserExists(int id)
        {
            return _context.Users.Any(e=>e.User_ID==id);
        }

    }                                       
}
