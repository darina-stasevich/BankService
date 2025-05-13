using BankService.Domain.Entities;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class UserRepository(ApplicationContext db) : IUserRepository
{
    public User? GetById(Guid id)
    {
        return db.Users.FirstOrDefault(u => u.Id == id);
    }

    public User? FindBySameData(User user)
    {
        var found =  db.Users.FirstOrDefault(u => u.FirstName == user.FirstName &&
                                            u.LastName == user.LastName &&
                                            u.IsResident == user.IsResident &&
                                            u.Email == user.Email &&
                                            u.PhoneNumber == user.PhoneNumber &&
                                            u.SecondName == user.SecondName &&
                                            (user.IsResident == true
            ? u.NationalPassportNumber == user.NationalPassportNumber && u.NationalPassportID
            == user.NationalPassportID
            : u.ForeignPassportNumber == user.ForeignPassportNumber && u.ForeignPassportID == user.ForeignPassportID));
        return found;
    }

    public User? FindByUniqueData(User user)
    {
        
        var found = db.Users.FirstOrDefault(u => u.Id != user.Id &&
                                            (u.Email == user.Email ||
                                             u.PhoneNumber == user.PhoneNumber ||
                                             (user.IsResident == true
                                                ? (u.NationalPassportID == user.NationalPassportID ||
                                                  u.NationalPassportNumber == user.NationalPassportNumber)
                                                : (u.ForeignPassportNumber == user.ForeignPassportNumber ||
                                                  u.ForeignPassportID == user.ForeignPassportID))));
        return found;
    }

    public User? FindByPassport(string data)
    {
        return db.Users.FirstOrDefault(u => u.NationalPassportNumber == data);
    }

    public void Add(User user)
    {
        db.Users.Add(user);
        db.SaveChanges();
    }

    public void Update(User user)
    {
        db.Entry(user).State = EntityState.Modified;
        db.SaveChanges();
    }
}