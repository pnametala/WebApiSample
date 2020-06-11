using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using NUnit.Framework;
using WebApplicationSample.Controllers;
using WebApplicationSample.Models;

namespace WebApplicationSampleUnitTest.Controllers
{
    public class MatchControllerTests
    {
        private delegate void ObjectValidatorDelegate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model);
        private ObjectValidatorDelegate _validatorDelegate;
        
        private Mock<IObjectModelValidator> _moqValidator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validatorDelegate += ObjectValidatorExecutor; 
            
            _moqValidator = new Mock<IObjectModelValidator>();
            _moqValidator.Setup(x => 
                x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
                .Callback(_validatorDelegate);
        }
        private void ObjectValidatorExecutor(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model) 
        { 
            var validationResults = new List<ValidationResult>(); 
            var b = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);
            
            foreach (var result in validationResults) 
            {
                actionContext.ModelState.AddModelError(result.MemberNames.FirstOrDefault(), result.ErrorMessage); 
            } 
        }

        private MatchController GetController()
        {
            return new MatchController { ObjectValidator = _moqValidator.Object };
        }
        
        [TestCaseSource(nameof(SuccessfulRequests))]
        public void SendingPost_WithValidModel_ShouldExpectOkResult(string str, string substr)
        {
            // Arrange
            var match = new MatchEntity
            {
                Text = str,
                Subtext = substr
            };
            // Act
            var result = GetController().Post(match);
            // Assert
            Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        }
        
        [TestCaseSource(nameof(InvalidRequests))]
        public void SendingPost_WithInvalidModel_ModelStateShouldHaveErrors(string str, string substr)
        {
            // Arrange
            var match = new MatchEntity
            {
                Text = str,
                Subtext = substr
            };
            // Act
            var controller = GetController();
            controller.TryValidateModel(match);
            var result = controller.Post(match);
            
            // Assert
            Assert.That(result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));
        }
        
        [TestCaseSource(nameof(MatchScenarios))]
        public void SendingPost_WithValidMatchScenarios_ShouldExpectCorrectNumberOfOccurrences(string str, string substr, int occurrences)
        {
            // Arrange
            var match = new MatchEntity
            {
                Text = str,
                Subtext = substr
            };
            // Act
            var controller = GetController();
            controller.TryValidateModel(match);
            var actionResult = controller.Post(match);
            var okResult = actionResult as OkObjectResult;
            var matchResult = okResult?.Value as MatchResult;
                
            // Assert
            Assert.That(matchResult?.Occurrences.Count, Is.EqualTo(occurrences));
        }

        private static IEnumerable SuccessfulRequests
        {
            get
            {
                yield return new TestCaseData("blablabla", "bla");
                yield return new TestCaseData("My My My my", "?");
                yield return new TestCaseData("big/phrase/separated/by/slash/", "/");
            }
        }
        
        private static IEnumerable InvalidRequests
        {
            get
            {
                yield return new TestCaseData("", "");
                yield return new TestCaseData("", "?");
                yield return new TestCaseData("Any String", "");
            }
        }

        private static IEnumerable MatchScenarios
        {
            get
            {
                yield return new TestCaseData("bla bla bla", "bla", 3);
                yield return new TestCaseData("Hello, may I help you?", ",", 1);
                yield return new TestCaseData("?@?@?@?@?@?@?", "@", 6);
            }
        }
    }
}