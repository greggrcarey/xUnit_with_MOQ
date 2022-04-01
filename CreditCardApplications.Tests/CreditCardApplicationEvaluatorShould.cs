using Xunit;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptHighIncomeApplications()
        {
            /*
             * CreditCardApplicationEvaluator requires an expensive service that is unable to be passed
             * as null for testing. Mocking a FrequentFlyerNumberValidor Service will allow proper testing 
             * of the CreditCardApplicationEvaluator 
             */
            var sut = new CreditCardApplicationEvaluator();
            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };
            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }
        [Fact]
        public void ReferYoungApplications()
        {
            CreditCardApplicationEvaluator sut = new();

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
    }
}