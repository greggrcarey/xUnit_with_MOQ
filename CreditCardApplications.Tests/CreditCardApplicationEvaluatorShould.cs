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

            CreditCardApplicationEvaluator sut = new(mockValidator.Object);

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);//Requred for proper age testing

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
        [Fact]
        public void DeclineLowIncomeApplicaitons()
        {
            /* Without proper mock setup, this test fails because the the Frequent Flyer IsValid check happens before 
             * the Low Income Check, which causes an early return with an incorrect decesion. The .Setup method marks the 
             * IsValid property as true, which satisfies the validation check
             * if the FFNumber is different from the .Setup, the Validation will fail
             */
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();

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
            /* Without proper mock setup, this test fails because the the Frequent Flyer IsValid check happens before 
             * the Low Income Check, which causes an early return with an incorrect decesion. The .Setup method marks the 
             * IsValid property as true, which satisfies the validation check
             * if the FFNumber is different from the .Setup, the Validation will fail
             */
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true); //Takes any string
            //mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);// takes a string starting with y
            //mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))).Returns(true);// Range
            //mockValidator.Setup(x => x.IsValid(It.IsIn("a", "z", "x"))).Returns(true);//for Sets
            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);//for Regex

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
            Mock<IFrequentFlyerNumberValidator> mockValidator = new(MockBehavior.Strict);

            //With MockBehavior.Strict, any propertly access that is not explicitly set up 
            //fail with an exception. Setting it up here will pass the test.
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication();
            CreditCardApplicationDecision decesion = sut.Evaluate(application);

            //With MockBehavior.Loose (default) this test passed becuase of the early evaluation
            //of Frequent Flyer validation number, even if that isn't the correct outcome
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decesion);

        }
        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();

            bool isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            CreditCardApplicationDecision decesion = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decesion);
        }
    
    }
}