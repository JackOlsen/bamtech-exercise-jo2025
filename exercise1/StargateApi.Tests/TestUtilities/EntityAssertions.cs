using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateApi.Tests.TestUtilities;

public static class EntityAssertions
{
    public static void AssertIsExpectedPersonAstronaut(
        this PersonAstronaut actual,
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

    public static void AssertIsExpectedAstronautDuty(
        this AstronautDuty actual,
        DateTime? expectedDutyEndDate,
        DateTime expectedDutyStartDate,
        string expectedDutyTitle,
        string expectedRank,
        int expectedPersonId,
        int expectedId)
    {
        Assert.AreEqual(expectedDutyEndDate, actual.DutyEndDate);
        Assert.AreEqual(expectedDutyStartDate, actual.DutyStartDate);
        Assert.AreEqual(expectedDutyTitle, actual.DutyTitle);
        Assert.AreEqual(expectedRank, actual.Rank);
        Assert.AreEqual(expectedPersonId, actual.PersonId);
        Assert.AreEqual(expectedId, actual.Id);
    }

    public static void AssertIsExpectedAstronautDetail(
        this AstronautDetail actual,
        DateTime? expectedCareerEndDate,
        DateTime expectedCareerStartDate,
        string expectedCurrentDutyTitle,
        string expectedCurrentRank,
        int expectedPersonId,
        int expectedId)
    {
        Assert.AreEqual(expectedCareerEndDate, actual.CareerEndDate);
        Assert.AreEqual(expectedCareerStartDate, actual.CareerStartDate);
        Assert.AreEqual(expectedCurrentDutyTitle, actual.CurrentDutyTitle);
        Assert.AreEqual(expectedCurrentRank, actual.CurrentRank);
        Assert.AreEqual(expectedPersonId, actual.PersonId);
        Assert.AreEqual(expectedId, actual.Id);
    }
}
