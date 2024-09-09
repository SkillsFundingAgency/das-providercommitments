using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

[TestFixture]
public class UpdateDataLockRequestViewModelMapperTests
{
    private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
    private UpdateDateLockRequest _updateDataLockRequest;
    private UpdateDataLockRequestViewModelMapper _mapper;
    private GetDataLockSummariesResponse _dataLockSummariesResponse;
    private GetApprenticeshipResponse _apprenticeshipResponse;
    private GetAllTrainingProgrammesResponse _allTrainingProgrammesResponse;
    private List<DataLock> _dataLocksWithCourseMismatch;
    private List<TrainingProgramme> _trainingProgrammes;
    private GetPriceEpisodesResponse _priceEpisodesResponse;
    private List<PriceEpisode> _priceEpisodes;
    private List<DataLock> _dataLocksWithPriceMismatch;
    private Fixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();

        _updateDataLockRequest = _fixture.Create<UpdateDateLockRequest>();
        _dataLocksWithPriceMismatch = new List<DataLock>
        {
            new()
            { 
                IsResolved = false, 
                DataLockStatus = Status.Fail, 
                ErrorCode = DataLockErrorCode.Dlock07, 
                IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                ApprenticeshipId = 123, 
                IlrTotalCost = 1500.00M 
            }
        };

        _dataLocksWithCourseMismatch = new List<DataLock>
        {
            new()
            {
                IsResolved = false,
                DataLockStatus = Status.Fail,
                ErrorCode = DataLockErrorCode.Dlock04,
                IlrTrainingCourseCode = "548",
                IlrEffectiveFromDate = DateTime.Now.Date.AddDays(15)
            }
        };           

        _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
            .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
            .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
            .Create();

        _apprenticeshipResponse = _fixture.Build<GetApprenticeshipResponse>()
            .With(p => p.CourseCode, "548")
            .With(p => p.CourseName, "DevOps engineer")
            .With(p => p.HasHadDataLockSuccess, false)
            .With(p => p.EndDate, DateTime.Now.Date.AddDays(100))
            .Create();

        _trainingProgrammes = new List<TrainingProgramme>
        {
            new() { CourseCode = "548", ProgrammeType = ProgrammeType.Standard, Name = "DevOps engineer" }
        };
        _allTrainingProgrammesResponse = _fixture.Build<GetAllTrainingProgrammesResponse>().With(x => x.TrainingProgrammes, _trainingProgrammes).Create();

        _priceEpisodes = new List<PriceEpisode>
        {
            new() { FromDate = DateTime.Now.Date, ToDate = null, Cost = 1000.0M }
        };
        _priceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
            .With(x => x.PriceEpisodes, _priceEpisodes)
            .Create();

        _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dataLockSummariesResponse);
        _mockCommitmentsApiClient.Setup(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_apprenticeshipResponse);
        _mockCommitmentsApiClient.Setup(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_allTrainingProgrammesResponse);
        _mockCommitmentsApiClient.Setup(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_priceEpisodesResponse);

        _mapper = new UpdateDataLockRequestViewModelMapper(_mockCommitmentsApiClient.Object);
    }

    [Test]
    public async Task ApprenticeshipHashedId_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.ApprenticeshipHashedId.Should().Be(_updateDataLockRequest.ApprenticeshipHashedId);
    }

    [Test]
    public async Task ApprenticeshipId_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.ApprenticeshipId.Should().Be(_updateDataLockRequest.ApprenticeshipId);
    }

    [Test]
    public async Task FirstName_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.FirstName.Should().Be(_apprenticeshipResponse.FirstName);
    }

    [Test]
    public async Task LastName_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.LastName.Should().Be(_apprenticeshipResponse.LastName);
    }

    [Test]
    public async Task ULN_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.ULN.Should().Be(_apprenticeshipResponse.Uln);
    }

    [Test]
    public async Task DateOfBirth_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.DateOfBirth.Should().Be(_apprenticeshipResponse.DateOfBirth);
    }

    [Test]
    public async Task CourseName_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.CourseName.Should().Be(_apprenticeshipResponse.CourseName);
    }

    [Test]
    public async Task ProviderId_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.ProviderId.Should().Be(_apprenticeshipResponse.ProviderId);
    }

    [Test]
    public async Task ProviderName_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert
        result.ProviderName.Should().Be(_apprenticeshipResponse.ProviderName);
    }

    [Test]
    public async Task GetDraftApprenticeshipIsCalled()
    {
        //Act
        await _mapper.Map(_updateDataLockRequest);

        //Assert
        _mockCommitmentsApiClient.Verify(m => m.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetApprenticeshipDatalockSummariesStatusIsCalled()
    {
        //Act
        await _mapper.Map(_updateDataLockRequest);

        //Assert
        _mockCommitmentsApiClient.Verify(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetAllTrainingProgrammesIsCalled()
    {
        //Act
        await _mapper.Map(_updateDataLockRequest);

        //Assert
        _mockCommitmentsApiClient.Verify(m => m.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetPriceEpisodesIsCalled()
    {
        //Act
        await _mapper.Map(_updateDataLockRequest);

        //Assert
        _mockCommitmentsApiClient.Verify(m => m.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once());
    }       

    [Test]
    public async Task Price_Datalock_Mismatch_IsMapped()
    {
        //Arrange            
        _dataLocksWithPriceMismatch = new List<DataLock>
        {
            new()
            { 
                IsResolved = false, 
                DataLockStatus = Status.Fail, 
                ErrorCode = DataLockErrorCode.Dlock07, 
                IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                ApprenticeshipId = 123, 
                IlrTotalCost = 1500.00M 
            },
        };
        _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
            .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
            .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
            .Create();

        _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dataLockSummariesResponse);
           
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        using (new AssertionScope())
        {
            //Assert            
            result.PriceDataLocks.Count().Should().Be(1);
            _priceEpisodes.FirstOrDefault().Cost.Should().Be(result.PriceDataLocks.FirstOrDefault().CurrentCost);
            _priceEpisodes.FirstOrDefault().ToDate.Should().Be(result.PriceDataLocks.FirstOrDefault().CurrentEndDate);
            _dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrTotalCost.Should().Be(result.PriceDataLocks.FirstOrDefault().IlrTotalCost);
            _dataLockSummariesResponse.DataLocksWithOnlyPriceMismatch.FirstOrDefault().IlrEffectiveFromDate.Should().Be(result.PriceDataLocks.FirstOrDefault().IlrEffectiveFromDate);
        }
    }

    [Test]
    public async Task Course_Datalock_Mismatch_IsMapped()
    {
        //Arrange
        _dataLocksWithCourseMismatch = new List<DataLock>
        {
            new()
            { 
                IsResolved = false, 
                DataLockStatus = Status.Fail, 
                ErrorCode = DataLockErrorCode.Dlock04,
                IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                ApprenticeshipId = 123, 
                IlrTrainingCourseCode = "548" 
            }
        };
        _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
            .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)              
            .Create();

        _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dataLockSummariesResponse);        

        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        using (new AssertionScope())
        {
            //Assert            
            result.CourseDataLocks.Count().Should().Be(1);
            _apprenticeshipResponse.CourseName.Should().Be(result.CourseDataLocks.FirstOrDefault().CurrentTrainingName);
            _priceEpisodes.FirstOrDefault().ToDate.Should().Be(result.CourseDataLocks.FirstOrDefault().CurrentEndDate);
            _allTrainingProgrammesResponse.TrainingProgrammes.FirstOrDefault().Name.Should().Be(result.CourseDataLocks.FirstOrDefault().IlrTrainingName);
            _dataLockSummariesResponse.DataLocksWithCourseMismatch.FirstOrDefault().IlrEffectiveFromDate.Should().Be(result.CourseDataLocks.FirstOrDefault().IlrEffectiveFromDate);
        }
    }
        
    [Test]
    public async Task Multiple_Price_Datalock_Mismatch_IsMapped()
    {
        //Arrange                        
        _dataLocksWithPriceMismatch = new List<DataLock>
        {
            new()
            { 
                IsResolved = false, 
                DataLockStatus = Status.Fail, 
                ErrorCode = DataLockErrorCode.Dlock07, 
                IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                ApprenticeshipId = 123, 
                IlrTotalCost = 1500.00M 
            },
            new()
            { 
                IsResolved = false, 
                DataLockStatus = Status.Fail, 
                ErrorCode = DataLockErrorCode.Dlock07, 
                IlrEffectiveFromDate = DateTime.Now.Date.AddDays(7), 
                ApprenticeshipId = 123, 
                IlrTotalCost = 1300.00M 
            }
        };
        _dataLockSummariesResponse = _fixture.Build<GetDataLockSummariesResponse>()
            .With(x => x.DataLocksWithCourseMismatch, _dataLocksWithCourseMismatch)
            .With(x => x.DataLocksWithOnlyPriceMismatch, _dataLocksWithPriceMismatch)
            .Create();

        _mockCommitmentsApiClient.Setup(m => m.GetApprenticeshipDatalockSummariesStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dataLockSummariesResponse);

        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        //Assert            
        result.PriceDataLocks.Count().Should().Be(2);
    }

    [Test]
    public async Task Course_And_price_Datalock_Mismatch_IsMapped()
    {
        //Act
        var result = await _mapper.Map(_updateDataLockRequest);

        using (new AssertionScope())
        {
            //Assert            
            result.CourseDataLocks.Count().Should().Be(1);
            result.PriceDataLocks.Count().Should().Be(1);
        }
    }
}