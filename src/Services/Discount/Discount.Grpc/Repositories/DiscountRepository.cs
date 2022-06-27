using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration= configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var result = await connection.ExecuteAsync("insert into coupon(ProductName, Description, Amount) values (@ProductName, @Description, @Amount)", new { ProductName = coupon.ProductName, Description=coupon.Description, Amount=coupon.Amount});
            if (result ==0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var result = await connection.ExecuteAsync("delete from coupon where ProductName=@ProductName", new { ProductName = productName});
            if (result == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("select * from coupon where ProductName=@ProductName", new { ProductName = productName });
            if (coupon == null)
            {
                return new Coupon { ProductName="No Discount", Amount=0, Description="No Discount Desc"};
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var result = await connection.ExecuteAsync("update coupon set productName=@ProductName, description=@Description, amount=@Amount where id=@Id", new { ProductName = coupon.ProductName, Description=coupon.Description, Amount=coupon.Amount , Id=coupon.Id});
            if (result == 0)
            {
                return false;
            }
            return true;
        }
    }
}
