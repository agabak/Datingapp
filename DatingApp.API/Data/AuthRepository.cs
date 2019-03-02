using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{  
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string passowrd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
             if (user == null) return null;
             
             if(!VerifyPasswordHash(passowrd, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        private bool VerifyPasswordHash(string passowrd, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
              var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passowrd));
                for(int i =0 ; i< computedHash.Length; i++)
                {
                     if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string passowrd)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(passowrd, out passwordHash, out passwordSalt);
            user.PasswordHash =  passwordHash;
            user.PasswordSalt = passwordSalt;

            // Save into the database
            await  _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string passowrd, out byte[] passwordHash, out byte[] passwordSalt)
        {   // if you have out parament you must change the values 
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
              passwordSalt = hmac.Key;
              passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passowrd));
            }
        }

        public async Task<bool> UserExist(string username)
        {
            if( await _context.Users.AnyAsync(x => x.Username == username)) return true;
            return false;
        }
    }
}