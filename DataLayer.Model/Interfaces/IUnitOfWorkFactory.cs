

namespace DataAccessLayer.Model.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
