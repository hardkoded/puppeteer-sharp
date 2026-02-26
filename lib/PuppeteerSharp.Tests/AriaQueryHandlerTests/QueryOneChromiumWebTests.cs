using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class QueryOneChromiumWebTests : PuppeteerPageBaseTest
    {
        private const string SetupPageContent = @"
          <h2 id=""shown"">title</h2>
          <h2 id=""hidden"" aria-hidden=""true"">title</h2>
          <div id=""node1"" aria-labelledby=""node2""></div>
          <div id=""node2"" aria-label=""bar""></div>
          <div id=""node3"" aria-label=""foo""></div>
          <div id=""node4"" class=""container"">
          <div id=""node5"" role=""button"" aria-label=""foo""></div>
          <div id=""node6"" role=""button"" aria-label=""foo""></div>
          <!-- Accessible name not available when element is hidden -->
          <div id=""node7"" hidden role=""button"" aria-label=""foo""></div>
          <div id=""node8"" role=""button"" aria-label=""bar""></div>
          </div>
          <button id=""node10"">text content</button>
          <h1 id=""node11"">text content</h1>
          <!-- Accessible name not available when role is ""presentation"" -->
          <h1 id=""node12"" role=""presentation"">text content</h1>
          <!-- Elements inside shadow dom should be found -->
          <script>
          const div = document.createElement('div');
          const shadowRoot = div.attachShadow({mode: 'open'});
          const h1 = document.createElement('h1');
          h1.textContent = 'text content';
          h1.id = 'node13';
          shadowRoot.appendChild(h1);
          document.documentElement.appendChild(div);
          </script>
          <img id=""node20"" src="""" alt=""Accessible Name"">
          <input id=""node21"" type=""submit"" value=""Accessible Name"">
          <label id=""node22"" for=""node23"">Accessible Name</label>
          <!-- Accessible name for the <input> is ""Accessible Name"" -->
          <input id=""node23"">
          <div id=""node24"" title=""Accessible Name""></div>
          <div role=""tree"">
          <div role=""treeitem"" id=""node30"">
          <div role=""treeitem"" id=""node31"">
          <div role=""treeitem"" id=""node32"">item1</div>
          <div role=""treeitem"" id=""node33"">item2</div>
          </div>
          <div role=""treeitem"" id=""node34"">item3</div>
          </div>
          </div>
          <!-- Accessible name for the <div> is ""item1 item2 item3"" -->
          <div aria-describedby=""node30""></div>";

        private async Task<string[]> GetIdsAsync(IElementHandle[] elements)
        {
            return await Task.WhenAll(elements.Select(element => element.EvaluateFunctionAsync<string>("element => element.id")));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler queryOne (Chromium web test)", "should find by name \"foo\"")]
        public async Task ShouldFindByNameFoo()
        {
            await Page.SetContentAsync(SetupPageContent);
            var found = await Page.QuerySelectorAllAsync("aria/foo");
            var ids = await GetIdsAsync(found);
            Assert.That(ids, Is.EqualTo(new[] { "node3", "node5", "node6" }));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler queryOne (Chromium web test)", "should find by name \"bar\"")]
        public async Task ShouldFindByNameBar()
        {
            await Page.SetContentAsync(SetupPageContent);
            var found = await Page.QuerySelectorAllAsync("aria/bar");
            var ids = await GetIdsAsync(found);
            Assert.That(ids, Is.EqualTo(new[] { "node1", "node2", "node8" }));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler queryOne (Chromium web test)", "should find treeitem by name")]
        public async Task ShouldFindTreeitemByName()
        {
            await Page.SetContentAsync(SetupPageContent);
            var found = await Page.QuerySelectorAllAsync("aria/item1 item2 item3");
            var ids = await GetIdsAsync(found);
            Assert.That(ids, Is.EqualTo(new[] { "node30" }));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler queryOne (Chromium web test)", "should find by role \"button\"")]
        public async Task ShouldFindByRoleButton()
        {
            await Page.SetContentAsync(SetupPageContent);
            var found = await Page.QuerySelectorAllAsync("aria/[role=\"button\"]");
            var ids = await GetIdsAsync(found);
            Assert.That(ids, Is.EqualTo(new[] { "node5", "node6", "node8", "node10", "node21" }));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler queryOne (Chromium web test)", "should find by role \"heading\"")]
        public async Task ShouldFindByRoleHeading()
        {
            await Page.SetContentAsync(SetupPageContent);
            var found = await Page.QuerySelectorAllAsync("aria/[role=\"heading\"]");
            var ids = await GetIdsAsync(found);
            Assert.That(ids, Is.EqualTo(new[] { "shown", "node11", "node13" }));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler queryOne (Chromium web test)", "should find both ignored and unignored")]
        public async Task ShouldFindBothIgnoredAndUnignored()
        {
            await Page.SetContentAsync(SetupPageContent);
            var found = await Page.QuerySelectorAllAsync("aria/title");
            var ids = await GetIdsAsync(found);
            Assert.That(ids, Is.EqualTo(new[] { "shown" }));
        }
    }
}
