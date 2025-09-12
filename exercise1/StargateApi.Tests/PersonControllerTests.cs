using StargateApi.Tests.TestUtilities;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateApi.Tests;

[TestClass]
public sealed class PersonControllerTests
{
    private StargateContext _context = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize()
    {
        (_context, _client) = StargateApiWebApplicationFactory.GetStargateContextAndApiClient();
    }

    [TestMethod]
    public async Task GetPeople_NoPeople()
    {
        var response = await _client.GetAsync("person");
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getPeopleResult = await response.DeserializeResponseContentAsync<GetPeopleResult>();
        Assert.AreEqual(0, getPeopleResult.People.Count);
    }

    [TestMethod]
    public async Task GetPeople_Single()
    {
        var personName = "TestPerson";
        _context.People.Add(new Person(personName));
        await _context.SaveChangesAsync();

        var response = await _client.GetAsync("person");
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getPeopleResult = await response.DeserializeResponseContentAsync<GetPeopleResult>();
        Assert.AreEqual(1, getPeopleResult.People.Count);
        AssertPersonAstronaut(
            actual: getPeopleResult.People.Single(),
            expectedCareerEndDate: null,
            expectedCareerStartDate: null,
            expectedCurrentDutyTitle: string.Empty,
            expectedCurrentRank: string.Empty,
            expectedName: personName,
            expectedPersonId: 1);
    }

    [TestMethod]
    public async Task GetPeople_Multiple()
    {
        var person1Name = "TestPerson1";
        _context.People.Add(new Person(person1Name));
        var person2Name = "TestPerson2";
        _context.People.Add(new Person(person2Name));
        await _context.SaveChangesAsync();

        var response = await _client.GetAsync("person");
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getPeopleResult = await response.DeserializeResponseContentAsync<GetPeopleResult>();
        Assert.AreEqual(2, getPeopleResult.People.Count);
        AssertPersonAstronaut(
            actual: getPeopleResult.People[0],
            expectedCareerEndDate: null,
            expectedCareerStartDate: null,
            expectedCurrentDutyTitle: string.Empty,
            expectedCurrentRank: string.Empty,
            expectedName: person1Name,
            expectedPersonId: 1);
        AssertPersonAstronaut(
            actual: getPeopleResult.People[1],
            expectedCareerEndDate: null,
            expectedCareerStartDate: null,
            expectedCurrentDutyTitle: string.Empty,
            expectedCurrentRank: string.Empty,
            expectedName: person2Name,
            expectedPersonId: 2);
    }

    private static void AssertPersonAstronaut(
        PersonAstronaut actual,
        DateTime? expectedCareerEndDate,
        DateTime? expectedCareerStartDate,
        string expectedCurrentDutyTitle,
        string expectedCurrentRank,
        string expectedName,
        int expectedPersonId)
    {
        Assert.AreEqual(expectedCareerEndDate, actual.CareerEndDate);
        Assert.AreEqual(expectedCareerStartDate, actual.CareerStartDate);
        Assert.AreEqual(expectedCurrentDutyTitle, actual.CurrentDutyTitle);
        Assert.AreEqual(expectedCurrentRank, actual.CurrentRank);
        Assert.AreEqual(expectedName, actual.Name);
        Assert.AreEqual(expectedPersonId, actual.PersonId);
    }
}
