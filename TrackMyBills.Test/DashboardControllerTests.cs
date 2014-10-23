using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TrackMyBills.Controllers;
using System.Web.Mvc;
using TrackMyBills.Models;
using System.Web.Routing;
using System.Web;
using Moq;
using TrackMyBills.Services;

namespace TrackMyBills.Test
{
    [TestFixture]
    public class DashboardControllerTests
    {
        #region Dashboard

        [Test]
        public static void ShouldReturnLoginViewFromDashboardViewWhenNoLoggedInUserCredentialInSession()
        {
            //arrange
            var dc = new DashboardController(new Mock<IBudgetService>().Object, new Mock<IBillService>().Object, new Mock<IAuditService>().Object);
            
            //act
            var result = dc.Dashboard();
            
            //var result = resultOrig as RedirectToRouteResult;

            //assert
            Assert.IsNotNull(result);
            //Assert.AreEqual("Login", result.RouteValues["action"]);
        }

        [Test]
        public static void ShouldReturnDashboardViewFromDashboardViewWhenLoggedInUserCredentialInSession()
        {
            //arrange
            var dc = new DashboardController(new Mock<IBudgetService>().Object, new Mock<IBillService>().Object, new Mock<IAuditService>().Object);

            var mockContext = new Mock<HttpContextBase>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockContext.Setup(m => m.Session).Returns(mockSession.Object);


            var controllerContext = new ControllerContext(mockContext.Object, new RouteData(), dc);
            dc.ControllerContext = controllerContext;

            dc.HttpContext.Session["LoggedInUser"] = new UserSecurityModel() { UserKey = "valid" };

            //act
            var result = dc.Dashboard() as ViewResult;

            //assert
            Assert.AreEqual("Dashboard", result.ViewName);
        }

        #endregion

        #region Budget
        
        [Test]
        public static void ShouldReturnJsonWithNewBudgetItemGuidWhenSavingPayDetails()
        {
            //Arrange
            var mockBudgetSvc = new Mock<IBudgetService>();
            var expectedGuid = Guid.NewGuid();
            var mockBudget = new BudgetModel();
            mockBudgetSvc.Setup(m => m.Save(mockBudget)).Returns(expectedGuid);
            var dc = new DashboardController(mockBudgetSvc.Object, new Mock<IBillService>().Object, new Mock<IAuditService>().Object);

            //Act
            var result = dc.SaveBudget(mockBudget) as JsonResult;

            //Assert
            Assert.AreEqual(expectedGuid, result.Data);
        }

        [Test]
        public static void ShouldReturnNewRecordAsJsonWhenAddingNewBudgetItem()
        {

        }

        [Test]
        public static void ShouldReturnEditedRecordAsJsonWhenSavingExistingBudgetItem()
        {

        }

        [Test]
        public static void ShouldReturnJsonTrueWhenDeletingBudgetItem()
        {

        }

        [Test]
        public static void ShouldReturnBudgetViewWhenViewingBudget()
        {
            //arrange
            var dc = new DashboardController(new Mock<IBudgetService>().Object, new Mock<IBillService>().Object, new Mock<IAuditService>().Object);

            //act
            var result = dc.Budget() as ViewResult;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Budget", result.ViewName);
        }

        [Test]
        public static void BudgetViewShouldContainBudgetModel()
        {
            //arrange
            var mockBudgetService = new Mock<IBudgetService>();
            var fakeBudgetList = new List<BudgetModel>();
            mockBudgetService.Setup(m => m.GetBudgetByUserKey("valid")).Returns(fakeBudgetList);
            var dc = new DashboardController(mockBudgetService.Object, new Mock<IBillService>().Object, new Mock<IAuditService>().Object);

            //act
            var result = dc.Budget() as ViewResult;

            //assert
            Assert.IsNotNull(result.Model);
            var modelAsTyped = result.Model as List<BudgetModel>;
            Assert.IsNotNull(modelAsTyped);
        }

        #endregion

        #region Bill

        //[Test]
        //public static void ShouldReturnJsonWithNewBillItemGuidWhenSavingBill()
        //{
        //    //Arrange
        //    var mockBillSvc = new Mock<IBillService>();
        //    var expectedGuid = Guid.NewGuid();
        //    var mockBill = new BillSaveModel();
        //    mockBillSvc.Setup(m => m.Save(mockBill)).Returns(expectedGuid);
        //    var dc = new DashboardController(new Mock<IBudgetService>().Object, mockBillSvc.Object);

        //    //Act
        //    var result = dc.SaveBill(mockBill);

        //    //Assert
        //    Assert.AreEqual(expectedGuid, result.Data);
        //}

        [Test]
        public static void ShouldReturnBillViewWhenViewingBills()
        {
            //arrange
            var dc = new DashboardController(new Mock<IBudgetService>().Object, new Mock<IBillService>().Object, new Mock<IAuditService>().Object);

            //act
            var result = dc.Bill() as ViewResult;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Bill", result.ViewName);
        }

        [Test]
        public static void BillViewShouldContainBillModel()
        {
            //arrange
            var mockBillService = new Mock<IBillService>();
            var fakeBillList = new List<BillModel>();
            mockBillService.Setup(m => m.GetCurrentBillsByUserKey("valid")).Returns(fakeBillList);
            var dc = new DashboardController(new Mock<IBudgetService>().Object, mockBillService.Object, new Mock<IAuditService>().Object);

            //act
            var result = dc.Bill() as ViewResult;

            //assert
            Assert.IsNotNull(result.Model);
            var modelAsTyped = result.Model as List<BillModel>;
            Assert.IsNotNull(modelAsTyped);
        }

        #endregion

    }
}
