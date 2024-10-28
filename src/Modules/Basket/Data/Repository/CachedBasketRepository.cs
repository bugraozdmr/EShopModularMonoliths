using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Basket.Models;
using Basket.Data.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repository;

public class CachedBasketRepository : IBasketRepository
{
    private readonly IBasketRepository _basketRepository;
    private readonly IDistributedCache _distributedCache;
    
    // proxy olarak verdi
    public CachedBasketRepository(IBasketRepository basketRepository
        , IDistributedCache distributedCache)
    {
        _basketRepository = basketRepository;
        _distributedCache = distributedCache;
    }
    
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new ShoppingCartConverter(), new ShoppingCartItemConverter() }
    };
    
    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        if (!asNoTracking)
        {
            // tracklenen islevler varsa yani sepete ekleme cikartma silme vs buraya dusup keyi yeniden cekiyor
            return await _basketRepository.GetBasket(userName, false, cancellationToken);
        } 
        
        var cachedBasket = await _distributedCache.GetStringAsync(userName, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedBasket))
        {
            // private set oldugu icin gelen deger yansitilamiyor ondan options tanimlandi bunlar gelcek diye yoksa taniyamiyordu
            // ONEMLI
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket,_options)!;
        }
        
        var basket = await _basketRepository.GetBasket(userName, asNoTracking, cancellationToken);
        await _distributedCache.SetStringAsync(userName
            ,JsonSerializer.Serialize(basket,_options)
            ,cancellationToken);
        return basket;
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await _basketRepository.CreateBasket(basket, cancellationToken);
        
        await _distributedCache.SetStringAsync(basket.UserName
            , JsonSerializer.Serialize(basket,_options)
            , cancellationToken);

        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        await _basketRepository.DeleteBasket(userName, cancellationToken);
        
        await _distributedCache.RemoveAsync(userName, cancellationToken);
        
        return true;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        var result = await _basketRepository.SaveChangesAsync(cancellationToken:cancellationToken);
        
        // TODO: Clear cache
        if (userName is not null)
        {
            await _distributedCache.RemoveAsync(userName, cancellationToken);
        }

        return result;
    }
}

/*
* Mantık belli redis ile anahtarları tutuyorsun
* ve eğer redisli olanlar seslenmemişse karışmıyorsun
* normal netodu sesliyorsun
*/