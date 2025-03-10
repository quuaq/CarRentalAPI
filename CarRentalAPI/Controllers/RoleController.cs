using CarRentalAPI.DataAccess;
using CarRentalAPI.DTO; // DTO klasörünü kullanıyoruz
using CarRentalAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly Context _context;

        // Constructor ile veritabanı context enjekte ediyoruz
        public RoleController(Context context)
        {
            _context = context;
        }

        // 1. Get: api/Role ==> Rolleri listeler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            // Role verilerini ve ilişkili User verilerini DTO'ya dönüştürüyoruz
            var roles = await _context.Roles.Include(r => r.Users)
                                .Select(r => new RoleDTO
                                {
                                    Role_ID = r.Role_ID,
                                    RoleName = r.RoleName,
                                    Description = r.Description,
                                    Users = r.Users.Select(u => new UserDTO
                                    {
                                        User_ID = u.User_ID,
                                        FirstName = u.FirstName,
                                        LastName = u.LastName,
                                        Email = u.Email,
                                        PhoneNumber = u.PhoneNumber,
                                        // RoleController'daki RoleName alanını UserDTO'ya atama
                                        RoleName = r.RoleName
                                    }).ToList()
                                }).ToListAsync();

            return roles;
        }

        // 2. Get: api/Role/5 ==> Belirli bir rolü ID'ye göre getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRole(int id)
        {
            var role = await _context.Roles.Include(r => r.Users)
                                           .Select(r => new RoleDTO
                                           {
                                               Role_ID = r.Role_ID,
                                               RoleName = r.RoleName,
                                               Description = r.Description,
                                               Users = r.Users.Select(u => new UserDTO
                                               {
                                                   User_ID = u.User_ID,
                                                   FirstName = u.FirstName,
                                                   LastName = u.LastName,
                                                   Email = u.Email,
                                                   PhoneNumber = u.PhoneNumber
                                               }).ToList()
                                           })
                                           .FirstOrDefaultAsync(r => r.Role_ID == id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        // 3. Post: api/Role ==> Yeni bir rol eklemek için
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> CreateRole(RoleDTO roleDto)
        {
            // RoleDTO'dan Role Entity'sine dönüştürme işlemi
            var role = new Role
            {
                RoleName = roleDto.RoleName,
                Description = roleDto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // DTO'ya dönüştürme
            var createdRole = new RoleDTO
            {
                Role_ID = role.Role_ID,
                RoleName = role.RoleName,
                Description = role.Description
            };

            return CreatedAtAction(nameof(GetRole), new { id = createdRole.Role_ID }, createdRole);
        }

        // 4. Put: api/Role/5 ==> Mevcut rolü günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDTO roleDto)
        {
            if (id != roleDto.Role_ID)
            {
                return BadRequest();
            }

            // Role Entity'sine DTO'dan veriyi dönüştürüyoruz
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.RoleName = roleDto.RoleName;
            role.Description = roleDto.Description;

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // 5. Delete: api/Role/5 ==> Rolü silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Role'ün olup olmadığını kontrol eden metot
        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Role_ID == id);
        }
    }
}
