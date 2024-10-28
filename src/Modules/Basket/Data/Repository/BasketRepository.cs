using Basket.Basket.Exceptions;
using Basket.Basket.Models;
using Microsoft.EntityFrameworkCore;

namespace Basket.Data.Repository;

public class BasketRepository : IBasketRepository
{
    private readonly BasketDbContext _context;

    public BasketRepository(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = _context.ShoppingCarts
            .Include(c => c.Items)
            .Where(x => x.UserName == userName);

        if (asNoTracking)
        {
            query.AsNoTracking();
        }
        
        var basket = await query.FirstOrDefaultAsync(cancellationToken);

        return basket ?? throw new BasketNotFoundException(userName);
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        _context.ShoppingCarts.Add(basket);
        await _context.SaveChangesAsync(cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await GetBasket(userName,false,cancellationToken);
        
        _context.ShoppingCarts.Remove(basket);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}