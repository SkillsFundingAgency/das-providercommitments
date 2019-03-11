# Digital Apprenticeships Service

## Employer Apprenticeship Service

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Employer Apprenticeship Service|
| Build | ![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge) |
| Web  | https://manage-apprenticeships.service.gov.uk/  |

## Approvals

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| Account API |
| Client  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Account.Api.Client)](https://www.nuget.org/packages/SFA.DAS.Account.Api.Client)  |


### Using Hashed Ids as Controller Method Parameters


#### Situation Described

You have a controller method which requires either an **account Id** or an **account legal entity Id** which, because these are "hashed" to obscure them, will need unhashing before they can be used.
The page describes how to get this done automatically.

#### Steps to have hashed values automically un-hahsed

1. Define a method parameter of type UnhashedAccount or UnhashedAccountLegalEntity. This can be a direct method parameter or can be a property on a model (or sub-model) defined as a parameter.

2. Use a request parameter named either **EmployerAccountPublicHashedId** or **EmployerAccountLegalEntityPublicHashedId**. This can be supplied as either a route parameter or query string parameter.  

The hashed value in the request parameter will automatically be un-hashed and the un-hashed value will be used to populate the method parameter defined in 1.

- You can include both account id and account legal entity id
- You can include them multiple times if needed


#### Example

    [Route("{providerId}/unapproved")]
    [Authorize()]
    public class UnapprovedController : Controller
    {
        private readonly IMediator _mediator;

        public UnapprovedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("add-apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getEmployerTask = GetEmployerIfRequired(
                request.AccountLegalEntity.AccountLegalEntityId);  

The above method requires the account legal entity id, which is expected to be provided as the **AccountLegalEntity** property of the model **AddDraftApprenticeshipRequest** (shown below). The model name is not significant, nor is the name of the property - AccountLegalEntity could have any name.


    public class AddDraftApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public UnhashedAccount Account { get; set; }
        public UnhashedAccountLegalEntity AccountLegalEntity { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }

The important aspect in the model is that the property _type_ of the AccountLegalEntity; it is type **UnhashedAccountLegalEntity**. This is a class that looks as follows:

    [Unhash]
    public class UnhashedAccountLegalEntity 
    {
        [Required]
        public long? AccountLegalEntityId { get; set; }
    }

This is just a simple class but has two important characteristics:

1. It has the **[Unhash]** attribute. This instructs the unhashing model binder provider that it should use the **unhashing model provider** to populate this model.
2. The property is named **AccountLegalEntityId**. The property names are significant. They must correspond to properties that are available in the **IHashingValues** for the request. The values that are available are defined in **RouteValueKeys**.   



## Next
* [How does it work?](UnhashingModelBindingImplementation.md "How does it work?")
