using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Users.Managers;

public class AddUserManager : ISaver<User>
{
    private readonly IDataHandler<User> _dataHandler;

    public AddUserManager(IDataHandler<User> dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public User Save(User data)
    {
        return _dataHandler.Save(data);
    }
}
