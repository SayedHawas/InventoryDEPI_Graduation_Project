
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.Entities.InventoryTransaction;

public class TransactionPayment : Entity
{
    public int TransactionId { get; private set; }
    public decimal PayedAmount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string PaymentMethod { get; private set; } = null!;

    private TransactionPayment(decimal payedAmount, string paymentMethod)
    {
        PayedAmount = payedAmount;
        PaymentMethod = paymentMethod;
    }

    internal static IResult<TransactionPayment> Create(decimal payedAmount, string paymentMethod)
    {
        if (payedAmount < 0)
            return new Result<TransactionPayment>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.PayedAmount.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrEmpty(paymentMethod))
            return new Result<TransactionPayment>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.PaymentMethod.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        return new Result<TransactionPayment>(new TransactionPayment(payedAmount, paymentMethod));
    }

    internal IResult<TransactionPayment> Update(decimal payedAmount, string paymentMethod)
    {
        if (payedAmount < 0)
            return new Result<TransactionPayment>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.PayedAmount.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrEmpty(paymentMethod))
            return new Result<TransactionPayment>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.PaymentMethod.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        PayedAmount = payedAmount;
        PaymentMethod = paymentMethod;

        return new Result<TransactionPayment>(this);
    }
}