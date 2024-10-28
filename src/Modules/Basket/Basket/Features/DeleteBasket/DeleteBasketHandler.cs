using Basket.Data.Repository;
using Shared.Contracts.CQRS;

namespace Basket.Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName)
    : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketHandler
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    private readonly IBasketRepository _basketRepository;
    public DeleteBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        // tracking default true -- burda lazım yoksa takip edemez
        await _basketRepository.DeleteBasket(command.UserName,cancellationToken);
        
        return new DeleteBasketResult(true);
    }
}