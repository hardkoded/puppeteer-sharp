using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.AutofillTests
{
    public class AutofillTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("autofill.spec", "ElementHandle.autofill", "should fill out a credit card")]
        public async Task ShouldFillOutACreditCard()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/credit-card.html");
            var name = await Page.WaitForSelectorAsync("#name");
            await name.AutofillAsync(new AutofillData
            {
                CreditCard = new CreditCardData
                {
                    Number = "4444444444444444",
                    Name = "John Smith",
                    ExpiryMonth = "01",
                    ExpiryYear = "2030",
                    Cvc = "123",
                },
            });

            var result = await Page.EvaluateFunctionAsync<string>(@"() => {
                const result = [];
                for (const el of document.querySelectorAll('input')) {
                    result.push(el.value);
                }
                return result.join(',');
            }");
            Assert.That(result, Is.EqualTo("John Smith,4444444444444444,01,2030,Submit"));
        }
    }
}
