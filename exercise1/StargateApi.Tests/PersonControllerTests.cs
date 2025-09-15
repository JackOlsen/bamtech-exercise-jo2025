using Microsoft.AspNetCore.Mvc;
using StargateApi.Tests.TestUtilities;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateApi.Tests;

[TestClass]
public sealed class PersonControllerTests
{
    private StargateContext _context = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize() => 
        (_context, _client) = StargateApiWebApplicationFactory.GetStargateContextAndApiClient();

    // TODO: Basic input validation tests, i.e.: create person with empty name, etc.

    [TestMethod]
    public async Task GetPeople_NoPeople()
    {
        // Act
        var response = await _client.GetAsync("person");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getPeopleResult = await response.DeserializeResponseContentAsync<GetPeopleResult>();
        Assert.AreEqual(0, getPeopleResult.People.Count);
    }

    [TestMethod]
    public async Task GetPeople_Single()
    {
        // Arrange
        var personName = "TestPerson";
        _context.People.Add(new Person(personName));
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("person");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getPeopleResult = await response.DeserializeResponseContentAsync<GetPeopleResult>();
        Assert.AreEqual(1, getPeopleResult.People.Count);
        getPeopleResult.People.Single().AssertIsExpectedPersonAstronaut(
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
        // Arrange
        var person1Name = "TestPerson1";
        _context.People.Add(new Person(person1Name));
        var person2Name = "TestPerson2";
        _context.People.Add(new Person(person2Name));
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("person");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var getPeopleResult = await response.DeserializeResponseContentAsync<GetPeopleResult>();
        Assert.AreEqual(2, getPeopleResult.People.Count);
        getPeopleResult.People[0].AssertIsExpectedPersonAstronaut(
            expectedCareerEndDate: null,
            expectedCareerStartDate: null,
            expectedCurrentDutyTitle: string.Empty,
            expectedCurrentRank: string.Empty,
            expectedName: person1Name,
            expectedPersonId: 1);
        getPeopleResult.People[1].AssertIsExpectedPersonAstronaut(
            expectedCareerEndDate: null,
            expectedCareerStartDate: null,
            expectedCurrentDutyTitle: string.Empty,
            expectedCurrentRank: string.Empty,
            expectedName: person2Name,
            expectedPersonId: 2);
    }

    [TestMethod]
    public async Task GetPersonByName_Unknown()
    {
        // Act
        var unknownPersonResponse = await _client.GetAsync("person/UnknownName");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, unknownPersonResponse.StatusCode);
        var problemDetails = await unknownPersonResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.IsNotNull(problemDetails);
        Assert.AreEqual("Not Found", problemDetails.Title);
        Assert.AreEqual("No person found with name 'UnknownName'.", problemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [TestMethod]
    public async Task GetPersonByName()
    {
        // Arrange
        var personName = "TestPerson";
        _context.People.Add(new Person(personName));
        await _context.SaveChangesAsync();

        // Act
        var knownPersonResponse = await _client.GetAsync($"person/{personName}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, knownPersonResponse.StatusCode);
        var getPersonByNameResultKnown = await knownPersonResponse.DeserializeResponseContentAsync<GetPersonByNameResult>();
        Assert.IsNotNull(getPersonByNameResultKnown.Person);
        getPersonByNameResultKnown.Person.AssertIsExpectedPersonAstronaut(
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
        // Arrange
        var personName = "TestPerson";

        // Act
        var response = await _client.PostAsJsonAsync(
            requestUri: "person",
            content: new CreatePerson
            {
                Name = personName
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location);
        Assert.AreEqual($"/person/{personName}", response.Headers.Location.ToString());
        var createPersonResult = await response.DeserializeResponseContentAsync<CreatePersonResult>();
        Assert.AreEqual(1, createPersonResult.Id);
    }

    [TestMethod]
    public async Task CreatePerson_Duplicate()
    {
        // Arrange
        var personName = "TestPerson";
        _context.People.Add(new Person(personName));
        await _context.SaveChangesAsync();

        // Act
        var createResponse = await _client.PostAsJsonAsync(
            requestUri: "person",
            content: new CreatePerson
            {
                Name = personName
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.Conflict, createResponse.StatusCode);
        var problemDetails = await createResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.IsNotNull(problemDetails);
        Assert.AreEqual("Conflict", problemDetails.Title);
        Assert.AreEqual("Duplicate astronaut name 'TestPerson'.", problemDetails.Detail);
        Assert.AreEqual((int)HttpStatusCode.Conflict, problemDetails.Status);
    }

    [TestMethod]
    public async Task UpdatePerson()
    {
        // Arrange
        var person1Name = "TestPerson1";
        _context.People.Add(new Person(person1Name));
        var person2Name = "TestPerson2";
        _context.People.Add(new Person(person2Name));
        await _context.SaveChangesAsync();
        var newName = "NewName";

        // Act
        var updatePersonUnknownResponse = await _client.PutAsJsonAsync(
            requestUri: "person",
            content: new UpdatePerson
            {
                CurrentName = "UnknownName",
                NewName = "NewName"
            });

        var updatePersonDuplicateResponse = await _client.PutAsJsonAsync(
            requestUri: "person",
            content: new UpdatePerson
            {
                CurrentName = person1Name,
                NewName = person2Name
            });

        var updatePersonResponse = await _client.PutAsJsonAsync(
            requestUri: "person",
            content: new UpdatePerson
            {
                CurrentName = person1Name,
                NewName = newName
            });

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, updatePersonUnknownResponse.StatusCode);
        var unknownPersonProblemDetails = await updatePersonUnknownResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.AreEqual((int)HttpStatusCode.NotFound, unknownPersonProblemDetails.Status);
        Assert.AreEqual("No person found with name 'UnknownName'.", unknownPersonProblemDetails.Detail);

        Assert.AreEqual(HttpStatusCode.Conflict, updatePersonDuplicateResponse.StatusCode);
        var duplicateProblemDetails = await updatePersonDuplicateResponse.DeserializeResponseContentAsync<ProblemDetails>();
        Assert.AreEqual((int)HttpStatusCode.Conflict, duplicateProblemDetails.Status);
        Assert.AreEqual("Duplicate astronaut name 'TestPerson2'.", duplicateProblemDetails.Detail);

        Assert.AreEqual(HttpStatusCode.OK, updatePersonResponse.StatusCode);
        var updatePersonResult = await updatePersonResponse.DeserializeResponseContentAsync<UpdatePersonResult>();
        Assert.AreEqual(1, updatePersonResult.Id);

        _context.ChangeTracker.Clear();
        var updatedPerson = await _context.People.FindAsync(1);
        Assert.IsNotNull(updatedPerson);
        Assert.AreEqual(newName, updatedPerson.Name);
    }    
}
