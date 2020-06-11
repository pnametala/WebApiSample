using System.Collections;
using NUnit.Framework;
using WebApplicationSample.Models;

namespace WebApplicationSampleUnitTest.Models
{
    public class MatchEntityTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(SuccessfulMatchScenarios))]
        public void GivenInput_WithExistingSubstring_ShouldExpectAtLeastOneMatch(string str, string substr, int expectedOccurrences)
        {
            // Arrange
            var match = new MatchEntity
            {
                Text = str,
                Subtext = substr
            };
            
            // Act
            var occurrences = match.FindOccurrences();
            // Assert
            Assert.That(occurrences.Count, Is.EqualTo(expectedOccurrences));
        }
        
        [TestCaseSource(nameof(NoMatchScenarios))]
        public void GivenInput_WithNonExistingSubstring_ShouldExpectNoMatches(string str, string substr)
        {
            // Arrange
            var match = new MatchEntity
            {
                Text = str,
                Subtext = substr
            };
            
            // Act
            var occurrences = match.FindOccurrences();
            // Assert
            Assert.That(occurrences.Count, Is.EqualTo(0));
        }

        private static IEnumerable SuccessfulMatchScenarios
        {
            get
            {
                yield return new TestCaseData("blablabla", "bla", 3);
                yield return new TestCaseData("My My My my", "my", 4);
                yield return new TestCaseData("big/phrase/separated/by/slash/", "/", 5);
            }
        }
        
        private static IEnumerable NoMatchScenarios
        {
            get
            {
                yield return new TestCaseData("blabla", "blablablabla");
                yield return new TestCaseData("My My My my", "?");
                yield return new TestCaseData("Any String", "");
            }
        }
    }
}