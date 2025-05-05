using CarRentalAPI.DTO;
using CarRentalAPI.Entities;

namespace CarRentalAPI.DataAccess
{
    public interface IAuthRepository
    {
        Task<User> Register(UserForRegisterDTO userForRegisterDto);
        Task<User?> Login(UserForLoginDTO userForLoginDto);
        Task<bool> UserExists(string email);
        
    }
}

