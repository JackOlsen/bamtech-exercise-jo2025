using Microsoft.AspNetCore.Mvc;
using StargateApi.Tests.TestUtilities;
using StargateAPI.Business.Commands;
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

    // TODO: Basic input validation tests, i.e.: create person with empty name, etc.

    [TestMethod]
    public async Task GetPeople_NoPeople()
    {
        var response = await _client.GetAsync("person");
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

    [TestMethod]
    public async Task GetPersonByName()
    {
        var personName = "TestPerson";
        _context.People.Add(new Person(personName));
        await _context.SaveChangesAsync();

        var unknownPersonResponse = await _client.GetAsync("person/UnknownName");
        Assert.AreEqual(HttpStatusCode.OK, unknownPersonResponse.StatusCode);
        var getPersonByNameResultUnknown = await unknownPersonResponse.DeserializeResponseContentAsync<GetPersonByNameResult>();
        Assert.IsNull(getPersonByNameResultUnknown.Person);

        var knownPersonResponse = await _client.GetAsync($"person/{personName}");
        Assert.AreEqual(HttpStatusCode.OK, knownPersonResponse.StatusCode);
        var getPersonByNameResultKnown = await knownPersonResponse.DeserializeResponseContentAsync<GetPersonByNameResult>();
        Assert.IsNotNull(getPersonByNameResultKnown.Person);
        AssertPersonAstronaut(
            actual: getPersonByNameResultKnown.Person,
            expectedCareerEndDate: null,
            expectedCareerStartDate: null,
            expectedCurrentDutyTitle: string.Empty,
            expectedCurrentRank: string.Empty,
            expectedName: personName,
            expectedPersonId: 1);
    }

    [TestMethod]
    public async Task CreatePerson()
    {
        var personName = "TestPerson";
        var createResponse = await _client.PostAsJsonAsync(
            requestUri: "person",
            content: new CreatePerson
            {
                Name = personName
            });
        Assert.AreEqual(HttpStatusCode.OK, createResponse.StatusCode);
        var createPersonResult = await createResponse.DeserializeResponseContentAsync<CreatePersonResult>();
        Assert.AreEqual(1, createPersonResult.Id);

        var createDuplicateResponse = await _client.PostAsJsonAsync(
            requestUri: "person",
            content: new CreatePerson
            {
                Name = personName
            });
        Assert.AreEqual(HttpStatusCode.Conflict, createDuplicateResponse.StatusCode);
        var duplicateProblemDetails = await createDuplicateResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.AreEqual("Duplicate astronaut name 'TestPerson'", duplicateProblemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.Conflict, duplicateProblemDetails.Status);
    }

    [TestMethod]
    public async Task UpdatePerson()
    {
        var person1Name = "TestPerson1";
        _context.People.Add(new Person(person1Name));
        var person2Name = "TestPerson2";
        _context.People.Add(new Person(person2Name));
        await _context.SaveChangesAsync();

        var unknownUpdatePersonResponse = await _client.PutAsJsonAsync(
            requestUri: "person",
            content: new UpdatePerson
            {
                CurrentName = "UnknownName",
                NewName = "NewName"
            });
        Assert.AreEqual(HttpStatusCode.NotFound, unknownUpdatePersonResponse.StatusCode);
        var unknownPersonProblemDetails = await unknownUpdatePersonResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.AreEqual("No person found with name 'UnknownName'.", unknownPersonProblemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.NotFound, unknownPersonProblemDetails.Status);

        var updatePersonDuplicateResponse = await _client.PutAsJsonAsync(
            requestUri: "person",
            content: new UpdatePerson
            {
                CurrentName = person1Name,
                NewName = person2Name
            });
        Assert.AreEqual(HttpStatusCode.Conflict, updatePersonDuplicateResponse.StatusCode);
        var duplicateProblemDetails = await updatePersonDuplicateResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.AreEqual("Duplicate astronaut name 'TestPerson2'", duplicateProblemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.Conflict, duplicateProblemDetails.Status);

        var newName = "NewName";
        var updatePersonResponse = await _client.PutAsJsonAsync(
            requestUri: "person",
            content: new UpdatePerson
            {
                CurrentName = person1Name,
                NewName = newName
            });
        Assert.AreEqual(HttpStatusCode.OK, updatePersonResponse.StatusCode);
        var updatePersonResult = await updatePersonResponse.DeserializeResponseContentAsync<UpdatePersonResult>();
        Assert.AreEqual(1, updatePersonResult.Id);

        _context.ChangeTracker.Clear();
        var updatedPerson = await _context.People.FindAsync(1);
        Assert.IsNotNull(updatedPerson);
        Assert.AreEqual(newName, updatedPerson.Name);
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
