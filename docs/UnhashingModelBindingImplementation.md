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


### How does automatic un-hashing work?

There is a custom model provider which looks for a marker attribute on any binding contexts (a binding context is each instance where a model property must be bound to the input request). There are many built-in model binder providers (and model binders) but the hashing model binder provider is inserted first in the list and so gets first-dibs. If the hashing model binder provider sees a UnhashAttribute on the model being bound to it will use the HashingModelBinder to set up the model otherwise it will let the next model binder provider have a go.

The hashing model binder gets injected with a IHashingContextProvider which allows it to get hold of an IHashingValues for the current request.

The IHashingContextProvider will create a new instance of IHashingValues and will give each IHashedPropertyModelBinder an opportunity to inspect the request for a hashed value that it is interested in. There are tow implementations of IHashedPropertyModelBinder - one to get the hashed account id and a second to get the hashed public account legal entity id. The IHashedPropertyModelBinder is responsible for un-hashing the request parameter.

Once the model binder has obtained a IHashingValues from the IHashingContextProvider all hashed values will have been obtained from the request parameters. The model binder will then inspect the model being populated and set each property to the value in the IHashedValues using the property name as a key into IHashValues.

### Not on IHashedService

There are two instances of the IHashingService (and only one implementation as well). One instance can un-hash public hashed account ids and the other can un-hash public account legal entity ids. An IoC policy has been created to ensure the appropriate instance of IHashingService will be injected. This is based on the parameter name - the type will be IHashingService but the name will be either:

1. publicAccountLegalEntityIdHashingService
2. publicAccountIdHashingService
 
These names are very specific. If they are called anything else then the policy will not bind one the pre-configured instances and the instead the service being created will get a default instance which will not have any config and will not be able to process the hashed ids coming into commitments.

