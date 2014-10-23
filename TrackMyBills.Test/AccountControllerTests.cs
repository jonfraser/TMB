using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TrackMyBills.Controllers;
using Moq;
using TrackMyBills.Services;
using System.Web.Mvc;

namespace TrackMyBills.Test
{
    [TestFixture]
    public class AccountControllerTests
    {
        [Test]
        public static void ShouldReturnDashboardViewWhenLoggedInWithValidUserKey()
        {
            //arrange
            var validUserKey = "valid";

            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(m=> m.Login(validUserKey)).Returns(true);

            var ac = new AccountController(mockAccountService.Object);
                        
            //act
            var result = ac.Login(validUserKey,"") as RedirectToRouteResult;

            //assert
            Assert.AreEqual("Dashboard", result.RouteValues["action"]);
            Assert.AreEqual("Dashboard", result.RouteValues["controller"]);
        }

        [Test]
        public static void ShouldReturnLoginViewWhenLoggedInWithInvalidUserKey()
        {
            //arrange
            var invalidUserKey = "invalid";

            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(m=> m.Login(invalidUserKey)).Returns(false);

            var ac = new AccountController(mockAccountService.Object);
                        
            //act
            var result = ac.Login(invalidUserKey,"") as ViewResult;

            //assert
            Assert.AreEqual("Login", result.ViewName);
        }

        [Test]
        public static void ShouldReturnDashboardViewWhenLoggedInWithValidUsernameAndPassword()
        {
            //arrange
            var validUsername = "valid";
            var validPassword = "valid";
            var validUserKey = "valid";

            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(m => m.Login(validUserKey)).Returns(true);
            mockAccountService.Setup(m => m.ComputeUserKey(validUsername, validPassword)).Returns(validUserKey);

            var ac = new AccountController(mockAccountService.Object);

            //act
            var result = ac.Login(validUsername,validPassword) as RedirectToRouteResult;

            //assert
            Assert.AreEqual("Dashboard", result.RouteValues["action"]);
            Assert.AreEqual("Dashboard", result.RouteValues["controller"]);
        }

        [Test]
        public static void ShouldReturnLoginViewWhenLoggedInWithInvalidUsernameAndPassword()
        {
            //arrange
            var invalidUsername = "invalid";
            var invalidPassword = "invalid";
            var invalidUserKey = "invalid";

            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(m => m.Login(invalidUserKey)).Returns(false);
            mockAccountService.Setup(m => m.ComputeUserKey(invalidUsername, invalidPassword)).Returns(invalidUserKey);

            var ac = new AccountController(mockAccountService.Object);

            //act
            var result = ac.Login(invalidUsername, invalidPassword) as ViewResult;

            //assert
            Assert.AreEqual("Login", result.ViewName);

        }

    }
}
