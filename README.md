# Digital Apprenticeships Service

## Context

Functionality within ProviderCommitments requires Commitments Api v2 (https://github.com/SkillsFundingAgency/das-commitments/blob/master/src/CommitmentsV2/readme.md)

ProviderCommitments has no home or landing page. The user is typically redirected there at various points from Provider Apprenticeship Service (https://github.com/SkillsFundingAgency/das-providerapprenticeshipsservice). However, a useful page is given below (where `10005077` is the UKPRN of the Provider):
* Manage your apprentices: https://localhost:5001/10005077/apprentices


## Getting Started

* Clone this repo: https://github.com/SkillsFundingAgency/das-providercommitments
* Obtain cloud config for:
  * SFA.DAS.ProviderCommitments
  * SFA.DAS.ProviderUrlHelper
  * SFA.DAS.Encoding
* Start Microsoft Azure Storage Emulator
* Run SFA.DAS.ProviderCommitments.Web project (run under kestrel)

#### LocalRunning
You can also stub the provider authentication by setting the following in the `appsettings.json`

```
  "UseStubProviderAuth": true,
  "UseLocalRegistry": true
```

The use local registry also loads the development container registration for non MI API usage

## See Also
* [Using Hashes in Controller Methods](docs/UnhashingModelBinding.md "Unhashing Model Binding")
