using Core.Entities;

namespace Application.Interfaces;

public interface ICouponRepository : IGenericRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code);
}
