using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Models;

namespace MinimalAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _dB;
        public CouponRepository(AppDbContext dB)
        {
            _dB = dB;
        }

        public async Task CreateAsync(Coupon coupon)
        {
            _dB.Coupons.AddAsync(coupon);
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            return await _dB.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetAsync(int id)
        {
            return await _dB.Coupons.FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<Coupon> GetAsync(string couponName)
        {
            return await _dB.Coupons.FirstOrDefaultAsync(c => c.Name == couponName.ToLower());
        }

        public async Task RemoveAsync(Coupon coupon)
        {
            _dB.Coupons.Remove(coupon);
        }

        public async Task SaveAsync()
        {
            await _dB.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _dB.Coupons.Update(coupon);
        }
    }
}
