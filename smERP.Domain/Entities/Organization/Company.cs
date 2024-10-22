using smERP.SharedKernel.Responses;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using System.Net;

namespace smERP.Domain.Entities.Organization;

public class Company : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string? IconImage { get; private set; }
    public string? CoverImage { get; private set; }
    public bool IsHidden { get; private set; }
    //public ICollection<Branch> Branches { get; private set; }
    //public ICollection<Department> Departments { get; private set; }

    private Company(string name)
    {
        Name = name;
        IsHidden = true;
        //Branches = new List<Branch>();
        //Departments = new List<Department>();
    }

    //public static BaseResponse<Company> Create(string name)
    //{
    //    if (string.IsNullOrWhiteSpace(name))
    //    {
    //        var localizedCompanyName = SharedResourcesKeys.CompanyName.Localize();
    //        return BaseResponse<Company>.Failure(HttpStatusCode.BadRequest, SharedResourcesKeys.BadRequest.Localize(), [SharedResourcesKeys.Required_FelidName.Localize(localizedCompanyName)]);
    //    }
    //    return BaseResponse<Company>.Created(new Company(name), SharedResourcesKeys.Created.Localize());
    //}

    //public BaseResponse<Company> UpdateName(string name, BaseResponseHandler responseHandler)
    //{
    //    if (string.IsNullOrWhiteSpace(name))
    //    {
    //        var localizedCompanyName = SharedResourcesKeys.CompanyName.Localize();
    //        return BaseResponse<Company>.Failure(HttpStatusCode.BadRequest, SharedResourcesKeys.BadRequest.Localize(), [SharedResourcesKeys.Required_FelidName.Localize(localizedCompanyName)]);
    //    }
    //    Name = name;
    //    return BaseResponse<Company>.Success(this, SharedResourcesKeys.UpdatedSuccess.Localize());
    //}

    public static IResult<Company> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            var localizedCompanyName = SharedResourcesKeys.CompanyName.Localize();

            var result = new Result<Company>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(localizedCompanyName))
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);

            return result;
        }
        return new Result<Company>(new Company(name))
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public IResult<Company> UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            var localizedCompanyName = SharedResourcesKeys.CompanyName.Localize();

            var result = new Result<Company>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(localizedCompanyName))
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);

            return result;
        }
        Name = name;

        return new Result<Company>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public void UpdateIconImage(string? iconImage)
    {
        IconImage = iconImage;
    }

    public void UpdateCoverImage(string? coverImage)
    {
        CoverImage = coverImage;
    }

    //public void AddBranch(Branch branch)
    //{
    //    Branches.Add(branch);
    //}

    //public void AddDepartment(Department department)
    //{
    //    Departments.Add(department);
    //}
}


