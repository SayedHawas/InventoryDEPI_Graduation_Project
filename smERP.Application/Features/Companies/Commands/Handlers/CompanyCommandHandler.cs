//using MediatR;
//using smERP.Application.Contracts.Persistence;
//using smERP.Application.Features.Companies.Commands.Models;
//using smERP.Domain.Entities.Organization;
//using smERP.SharedKernel.Localizations.Resources;
//using smERP.SharedKernel.Responses;

//namespace smERP.Application.Features.Companies.Commands.Handlers;

//public class CompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork) : 
//    IRequestHandler<AddCompanyCommandModel, IResultBase>
//{
//    private readonly ICompanyRepository _companyRepository = companyRepository;
//    private readonly IUnitOfWork _unitOfWork = unitOfWork;

//    public async Task<IResultBase> Handle(AddCompanyCommandModel request, CancellationToken cancellationToken)
//    {
//        var companyToBeCreated = Company.Create(request.CompanyName);
//        if (companyToBeCreated.IsFailed) 
//            return companyToBeCreated;

//        await companyToBeCreated.WithTask(() => _companyRepository.Add(companyToBeCreated.Value, cancellationToken),SharedResourcesKeys.DatabaseError);
//        if (companyToBeCreated.IsFailed)
//            return companyToBeCreated;

//        await companyToBeCreated.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
//        if (companyToBeCreated.IsFailed)
//            return companyToBeCreated;

//        var result = companyToBeCreated.ChangeType(companyToBeCreated.Value.Id);
//        return result;
//    }
//}
