using CarRentalAPI.Entities;
using CarRentalAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using CarRentalAPI.DTO;
using CarRentalAPI.Helpers;
using Microsoft.AspNetCore.Authorization;


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
                                                Role_ID = u.Role_ID
                                            }).ToListAsync();

            return users;
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult SecretData()
        {
            return Ok("Bu bilgi sadece token sahiplerine özel 😎");
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
                                               Role_ID = u.Role_ID,
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
        public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return BadRequest("Password is required!");
            }

            // Şifreyi hashleyerek kaydet
            PasswordHelper.CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber,
                Role_ID = userDTO.Role_ID,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createdUserDTO = new UserDTO
            {
                User_ID = user.User_ID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role_ID = user.Role_ID
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.User_ID }, createdUserDTO);
        }



        //4. Put: api/Users/5 ==> Mevcut kullanıcıyı güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.User_ID)
            {
                return BadRequest("Güncellenmek istenen kullanıcı ID'si eşleşmiyor!");
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            // Kullanıcı bilgilerini güncelle
            existingUser.FirstName = userDTO.FirstName;
            existingUser.LastName = userDTO.LastName;
            existingUser.Email = userDTO.Email;
            existingUser.PhoneNumber = userDTO.PhoneNumber;
            existingUser.Role_ID = userDTO.Role_ID;

            // Şifre güncelleme kontrolü
            if (!string.IsNullOrEmpty(userDTO.Password))
            {
                PasswordHelper.CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
                existingUser.PasswordHash = passwordHash;
                existingUser.PasswordSalt = passwordSalt;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
