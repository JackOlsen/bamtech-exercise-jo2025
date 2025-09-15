using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StargateApi.Tests.TestUtilities;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;
using System.Net;
using static StargateAPI.Business.Constants;

namespace StargateApi.Tests;

[TestClass]
public class AstronautDutyControllerTests
{
    private StargateContext _context = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize() => 
        (_context, _client) = StargateApiWebApplicationFactory.GetStargateContextAndApiClient();

    // TODO: Basic input validation tests, i.e.: create person with empty name, etc.

    [TestMethod]
    public async Task GetAstronautDutiesByName_NotFound()
    {
        // Act
        var response = await _client.GetAsync("astronautduty/UnknownName");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails = await response.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.IsNotNull(problemDetails);
        Assert.AreEqual("Not Found", problemDetails.Title);
        Assert.AreEqual("No astronaut found with name 'UnknownName'.", problemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [TestMethod]
    public async Task GetAstronautDutiesByName()
    {
        // Arrange
        var person1Name = "TestPerson1";
        var currentRank = "SGT";
        var currentDutyTitle = "Technician";
        var careerStartDate = new DateTime(2020, 12, 8);
        var careerEndDate = new DateTime(2015, 6, 5);

        var duty1StartDate = new DateTime(2020, 1, 1);
        var duty1EndDate = new DateTime(2021, 1, 1);
        var duty1Title = "Commander";
        var duty1Rank = "1LT";

        var duty2StartDate = new DateTime(2021, 2, 1);
        var duty2EndDate = (DateTime?)null;
        var duty2Title = "Pilot";
        var duty2Rank = "CPT";

        _context.People.Add(new Person(person1Name)
        {
            AstronautDetail = new (
                currentRank: currentRank,
                currentDutyTitle: currentDutyTitle,
                careerStartDate: careerStartDate,
                careerEndDate: careerEndDate),
            AstronautDuties = 
            [
                new AstronautDuty(
                    dutyStartDate: duty1StartDate,
                    dutyTitle: duty1Title,
                    rank: duty1Rank,
                    dutyEndDate: duty1EndDate),
                new AstronautDuty(
                    dutyStartDate: duty2StartDate,
                    dutyTitle: duty2Title,
                    rank: duty2Rank,
                    dutyEndDate: duty2EndDate),
            ]
        });
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"astronautduty/{person1Name}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getAstronautDutiesByNameResult = await response.DeserializeResponseContentAsync<GetAstronautDutiesByNameResult>();

        getAstronautDutiesByNameResult.Person.AssertIsExpectedPersonAstronaut(
            expectedCareerEndDate: careerEndDate,
            expectedCareerStartDate: careerStartDate,
            expectedCurrentDutyTitle: currentDutyTitle,
            expectedCurrentRank: currentRank,
            expectedName: person1Name,
            expectedPersonId: 1);

        Assert.AreEqual(2, getAstronautDutiesByNameResult.AstronautDuties.Count);

        getAstronautDutiesByNameResult.AstronautDuties[0].AssertIsExpectedAstronautDuty(
            expectedDutyEndDate: duty2EndDate,
            expectedDutyStartDate: duty2StartDate,
            expectedDutyTitle: duty2Title,
            expectedRank: duty2Rank,
            expectedPersonId: 1,
            expectedId: 2);

        getAstronautDutiesByNameResult.AstronautDuties[1].AssertIsExpectedAstronautDuty(
            expectedDutyEndDate: duty1EndDate,
            expectedDutyStartDate: duty1StartDate,
            expectedDutyTitle: duty1Title,
            expectedRank: duty1Rank,
            expectedPersonId: 1,
            expectedId: 1);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_NotFound()
    {
        // Act
        var response = await _client.PostAsJsonAsync(
            requestUri: "astronautduty", 
            content: new CreateAstronautDuty
            {
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2020, 1, 1),
                Name = "UnknownName",
                Rank = "1LT"
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails = await response.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.IsNotNull(problemDetails);
        Assert.AreEqual("Not Found", problemDetails.Title);
        Assert.AreEqual("No astronaut found with name 'UnknownName'.", problemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_Duplicate()
    {
        // Arrange
        var personName = "TestPerson";
        var currentDutyTitle = "DutyTitle";
        var currentDutyStartDate = new DateTime(2020, 1, 1);
        var currentRank = "1LT";
        _context.People.Add(new Person(personName)
        {
            AstronautDuties = 
            [
                new AstronautDuty(
                    dutyEndDate: null,
                    dutyStartDate: currentDutyStartDate,
                    dutyTitle: currentDutyTitle,
                    rank: currentRank)
            ]
        });
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.PostAsJsonAsync(
            requestUri: "astronautduty",
            content: new CreateAstronautDuty
            {
                DutyTitle = currentDutyTitle,
                DutyStartDate = currentDutyStartDate,
                Name = personName,
                Rank = "SGT"
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        var problemDetails = await response.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.IsNotNull(problemDetails);
        Assert.AreEqual("Conflict", problemDetails.Title);
        Assert.AreEqual("Duplicate astronaut duty.", problemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.Conflict, problemDetails.Status);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_Initial()
    {
        // Arrange
        var personName = "TestPerson";
        var dutyTitle = "DutyTitle";
        var dutyStartDate = new DateTime(2020, 1, 1);
        var rank = "1LT";
        _context.People.Add(new Person(personName));
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.PostAsJsonAsync(
            requestUri: "astronautduty",
            content: new CreateAstronautDuty
            {
                DutyTitle = dutyTitle,
                DutyStartDate = dutyStartDate,
                Name = personName,
                Rank = rank
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location);
        Assert.AreEqual($"/astronautduty/{personName}", response.Headers.Location.ToString());
        var createAstronautDutyResult = await response.DeserializeResponseContentAsync<CreateAstronautDutyResult>();
        Assert.AreEqual(1, createAstronautDutyResult.Id);

        _context.ChangeTracker.Clear();
        var person = await _context.People
            .Include(p => p.AstronautDetail)
            .Include(p => p.AstronautDuties)
            .Where(p => p.Name == personName)
            .FirstAsync();

        Assert.IsNotNull(person.AstronautDetail);
        person.AstronautDetail.AssertIsExpectedAstronautDetail(
            expectedCareerEndDate: null,
            expectedCareerStartDate: dutyStartDate,
            expectedCurrentDutyTitle: dutyTitle,
            expectedCurrentRank: rank,
            expectedPersonId: 1,
            expectedId: 1);

        Assert.AreEqual(1, person.AstronautDuties.Count);
        person.AstronautDuties.First().AssertIsExpectedAstronautDuty(
            expectedDutyEndDate: null,
            expectedDutyStartDate: dutyStartDate,
            expectedDutyTitle: dutyTitle,
            expectedRank: rank,
            expectedPersonId: 1,
            expectedId: 1);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_Subsequent()
    {
        // Arrange
        var personName = "TestPerson";
        var currentDutyTitle = "DutyTitle";
        var currentDutyStartDate = new DateTime(2020, 1, 1);
        var currentRank = "1LT";
        _context.People.Add(new Person(personName)
        {
            AstronautDuties =
            [
                new AstronautDuty(
                    dutyEndDate: null,
                    dutyStartDate: currentDutyStartDate,
                    dutyTitle: currentDutyTitle,
                    rank: currentRank)
            ]
        });
        await _context.SaveChangesAsync();

        var newDutyTitle = "NewDutyTitle";
        var newDutyStartDate = new DateTime(2021, 1, 1);
        var newRank = "CPT";

        // Act
        var response = await _client.PostAsJsonAsync(
            requestUri: "astronautduty",
            content: new CreateAstronautDuty
            {
                DutyTitle = newDutyTitle,
                DutyStartDate = newDutyStartDate,
                Name = personName,
                Rank = newRank
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location);
        Assert.AreEqual($"/astronautduty/{personName}", response.Headers.Location.ToString());
        var createAstronautDutyResult = await response.DeserializeResponseContentAsync<CreateAstronautDutyResult>();
        Assert.AreEqual(2, createAstronautDutyResult.Id);

        _context.ChangeTracker.Clear();
        var person = await _context.People
            .Include(p => p.AstronautDetail)
            .Include(p => p.AstronautDuties)
            .Where(p => p.Name == personName)
            .FirstAsync();

        Assert.IsNotNull(person.AstronautDetail);
        person.AstronautDetail.AssertIsExpectedAstronautDetail(
            expectedCareerEndDate: null,
            expectedCareerStartDate: newDutyStartDate,
            expectedCurrentDutyTitle: newDutyTitle,
            expectedCurrentRank: newRank,
            expectedPersonId: 1,
            expectedId: 1);

        var astronautDutiesList = person.AstronautDuties
            .OrderBy(d => d.Id)
            .ToList();
        Assert.AreEqual(2, astronautDutiesList.Count);

        astronautDutiesList[0].AssertIsExpectedAstronautDuty(
            // Updated to 1 day before the new duty start date
            expectedDutyEndDate: new DateTime(2020, 12, 31),
            expectedDutyStartDate: currentDutyStartDate,
            expectedDutyTitle: currentDutyTitle,
            expectedRank: currentRank,
            expectedPersonId: 1,
            expectedId: 1);

        astronautDutiesList[1].AssertIsExpectedAstronautDuty(
            expectedDutyEndDate: null,
            expectedDutyStartDate: newDutyStartDate,
            expectedDutyTitle: newDutyTitle,
            expectedRank: newRank,
            expectedPersonId: 1,
            expectedId: 2);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_Retired()
    {
        // Arrange
        var personName = "TestPerson";
        var currentDutyTitle = "DutyTitle";
        var currentDutyStartDate = new DateTime(2020, 1, 1);
        var currentRank = "1LT";
        _context.People.Add(new Person(personName)
        {
            AstronautDuties =
            [
                new AstronautDuty(
                    dutyEndDate: null,
                    dutyStartDate: currentDutyStartDate,
                    dutyTitle: currentDutyTitle,
                    rank: currentRank)
            ]
        });
        await _context.SaveChangesAsync();

        var newDutyStartDate = new DateTime(2021, 1, 1);
        var newRank = "CPT";

        // Act
        var response = await _client.PostAsJsonAsync(
            requestUri: "astronautduty",
            content: new CreateAstronautDuty
            {
                DutyTitle = DutyTitles.RETIRED,
                DutyStartDate = newDutyStartDate,
                Name = personName,
                Rank = newRank
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location);
        Assert.AreEqual($"/astronautduty/{personName}", response.Headers.Location.ToString());
        var createAstronautDutyResult = await response.DeserializeResponseContentAsync<CreateAstronautDutyResult>();
        Assert.AreEqual(2, createAstronautDutyResult.Id);

        _context.ChangeTracker.Clear();
        var person = await _context.People
            .Include(p => p.AstronautDetail)
            .Include(p => p.AstronautDuties)
            .Where(p => p.Name == personName)
            .FirstAsync();

        Assert.IsNotNull(person.AstronautDetail);
        person.AstronautDetail.AssertIsExpectedAstronautDetail(
            expectedCareerEndDate: newDutyStartDate,
            expectedCareerStartDate: newDutyStartDate,
            expectedCurrentDutyTitle: DutyTitles.RETIRED,
            expectedCurrentRank: newRank,
            expectedPersonId: 1,
            expectedId: 1);

        var astronautDutiesList = person.AstronautDuties
            .OrderBy(d => d.Id)
            .ToList();
        Assert.AreEqual(2, astronautDutiesList.Count);

        astronautDutiesList[0].AssertIsExpectedAstronautDuty(
            // Updated to 1 day before the new duty start date
            expectedDutyEndDate: new DateTime(2020, 12, 31),
            expectedDutyStartDate: currentDutyStartDate,
            expectedDutyTitle: currentDutyTitle,
            expectedRank: currentRank,
            expectedPersonId: 1,
            expectedId: 1);

        astronautDutiesList[1].AssertIsExpectedAstronautDuty(
            expectedDutyEndDate: null,
            expectedDutyStartDate: newDutyStartDate,
            expectedDutyTitle: DutyTitles.RETIRED,
            expectedRank: newRank,
            expectedPersonId: 1,
            expectedId: 2);
    }
}
