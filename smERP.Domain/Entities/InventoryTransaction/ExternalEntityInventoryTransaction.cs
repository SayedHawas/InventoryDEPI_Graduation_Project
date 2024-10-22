
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Collections.Generic;
using System.Net;

namespace smERP.Domain.Entities.InventoryTransaction;

public abstract class ExternalEntityInventoryTransaction : InventoryTransaction
{
    public decimal AmountLeftToPay { get; private set; }
    public ICollection<TransactionPayment> Payments { get; private set; } = null!;

    protected ExternalEntityInventoryTransaction(int storageLocationId, DateTime transactionDate, ICollection<TransactionPayment>? payments, ICollection<InventoryTransactionItem> items) : base(storageLocationId, transactionDate, items)
    {
        Payments = payments ?? new List<TransactionPayment>();
        RecalculateAmountLeftToPay();
    }

    protected ExternalEntityInventoryTransaction() { }

    protected static IResult<(List<TransactionPayment>, List<InventoryTransactionItem>)> Create(List<(decimal PayedAmount, string PaymentMethod)>? payments,List<(int ProductInstanceId, int Quantity, decimal UnitPrice, bool IsTracked, List<string>? SerialNumbers)> transactionItems)
    {
        var baseDetailsCreateResult = CreateBaseDetails(transactionItems);
        if (baseDetailsCreateResult.IsFailed)
            return baseDetailsCreateResult.ChangeType((new List<TransactionPayment>(), new List<InventoryTransactionItem>()));

        var transactionPayment = new List<TransactionPayment>();

        if (payments != null && payments.Count > 0)
        {
            foreach (var (payedAmount, paymentMethod) in payments)
            {
                var transactionPaymentCreateResult = TransactionPayment.Create(payedAmount, paymentMethod);
                if (transactionPaymentCreateResult.IsFailed)
                    return transactionPaymentCreateResult.ChangeType((new List<TransactionPayment>(), new List<InventoryTransactionItem>()));

                transactionPayment.Add(transactionPaymentCreateResult.Value);
            }
        }

        return new Result<(List<TransactionPayment>, List<InventoryTransactionItem>)>((transactionPayment, baseDetailsCreateResult.Value));
    }

    public IResult<List<TransactionPayment>> AddTransactionPayments(List<(decimal PayedAmount, string PaymentMethod)> newPayments)
    {
        foreach (var (payedAmount, paymentMethod) in newPayments)
        {
            var newTransactionPayment = TransactionPayment.Create(payedAmount, paymentMethod);
            if (newTransactionPayment.IsFailed)
                return new Result<List<TransactionPayment>>()
                    .WithErrors(newTransactionPayment.Errors)
                    .WithStatusCode(HttpStatusCode.BadRequest);

            Payments.Add(newTransactionPayment.Value);
        }

        RecalculateAmountLeftToPay();

        return new Result<List<TransactionPayment>>(Payments.ToList());
    }

    public IResult<List<TransactionPayment>> UpdateTransactionPayments(List<(int TransactionPaymentId, decimal PayedAmount, string PaymentMethod)> payments)
    {
        foreach (var (transactionPaymentId, payedAmount, paymentMethod) in payments)
        {
            var existingTransactionPayment = Payments.FirstOrDefault(x => x.Id == transactionPaymentId);
            if (existingTransactionPayment != null)
            {
                var updateResult = existingTransactionPayment.Update(payedAmount, paymentMethod);
                if (updateResult.IsFailed)
                    return new Result<List<TransactionPayment>>()
                        .WithErrors(updateResult.Errors)
                        .WithStatusCode(HttpStatusCode.BadRequest);
            }
            else
            {
                var newTransactionPayment = TransactionPayment.Create(payedAmount, paymentMethod);
                if (newTransactionPayment.IsFailed)
                    return new Result<List<TransactionPayment>>()
                        .WithErrors(newTransactionPayment.Errors)
                        .WithStatusCode(HttpStatusCode.BadRequest);

                Payments.Add(newTransactionPayment.Value);
            }
        }

        RecalculateAmountLeftToPay();

        return new Result<List<TransactionPayment>>(Payments.ToList());
    }

    public IResult<List<TransactionPayment>> RemoveTransactionPayments(List<int> transactionPaymentIds)
    {
        if (transactionPaymentIds.Distinct().Count() != transactionPaymentIds.Count)
            return new Result<List<TransactionPayment>>()
                .WithBadRequestResult(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.PaymentTransaction.Localize()));

        var transactionPaymentsToBeRemoved = Payments.Where(x => transactionPaymentIds.Contains(x.Id)).ToList();

        if (transactionPaymentsToBeRemoved.Count != transactionPaymentIds.Count)
            return new Result<List<TransactionPayment>>()
                .WithBadRequestResult(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.PaymentTransaction.Localize()));

        foreach (var payment in transactionPaymentsToBeRemoved)
        {
            Payments.Remove(payment);
        }

        RecalculateAmountLeftToPay();

        return new Result<List<TransactionPayment>>();
    }

    protected void RecalculateAmountLeftToPay()
    {
        AmountLeftToPay = TotalAmount - Payments.Sum(x => x.PayedAmount);
    }
}
