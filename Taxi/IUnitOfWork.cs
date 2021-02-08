using TaxiDriverServiceWPF.DataTypes;
using TaxiDriverServiceWPF.Repository;

namespace TaxiDriverServiceWPF.UnitOfWorkNS
{
    public interface IUnitOfWork
    {
        GenericRepository<TaxiClient> Clients { get; }
        GenericRepository<TaxiDriver> Drivers { get; }
        GenericRepository<TaxiOrder> Orders { get; }
    }
}
