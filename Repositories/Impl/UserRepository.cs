using Chat.Data;
using Chat.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Repositories.Impl;

public class UserRepository(ApplicationDbContext _context) : IUserRepository
{
    public async Task<User?> FindByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    public async Task<User?> FindByIdAsync(int id)
        => await _context.Users.FindAsync(id);
    public async Task<List<User>> FindAllAsync()
        => await _context.Users.ToListAsync();
    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
