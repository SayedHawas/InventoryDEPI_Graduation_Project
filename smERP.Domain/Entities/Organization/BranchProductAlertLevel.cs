using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Domain.Entities.Organization;

public class BranchProductInstanceAlertLevel
{
    public int BranchId { get; }
    public int ProductInstanceId { get; }
    public int AlertLevel { get; internal set; }
    public virtual ProductInstance ProductInstance { get; internal set; } = null!;

    private BranchProductInstanceAlertLevel() { }

    internal BranchProductInstanceAlertLevel(int branchId, int productInstanceId, int alertLevel)
    {
        BranchId = branchId;
        ProductInstanceId = productInstanceId;
        AlertLevel = alertLevel;
    }
}
