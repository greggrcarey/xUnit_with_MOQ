using Moq;
using Xunit;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptHighIncomeApplications()
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator = new();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };
            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }
        [Fact]
        public void ReferYoungApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
     
            mockValidator.DefaultValue = DefaultValue.Mock;

            CreditCardApplicationEvaluator sut = new(mockValidator.Object);

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);//Requred for proper age testing

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
        [Fact]
        public void DeclineLowIncomeApplicaitons()
        {
            
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.Setup(x => x.IsValid("x")).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
        [Fact]
        public void DeclineLowIncomeApplicaitons_ArgumentMatching()
        {
           
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");


            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
      
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication();
            CreditCardApplicationDecision decesion = sut.Evaluate(application);


            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decesion);

        }
    
        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {
        
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");// less verbose

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);

        }
       
    }
}