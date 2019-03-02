using System.Threading.Tasks;
using  DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string passowrd);
         Task<User> Login(string username, string passowrd);
         Task<bool> UserExist(string username);
    }
}