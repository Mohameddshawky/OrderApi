using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
{
    public CouponRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Code == code);
    }
}
