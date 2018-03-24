using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CookiesTests : PuppeteerBaseTest
    {
        /*
         describe('Cookies', function() {
    afterEach(async({page, server}) => {
      const cookies = await page.cookies(server.PREFIX + '/grid.html', server.CROSS_PROCESS_PREFIX);
      for (const cookie of cookies)
        await page.deleteCookie(cookie);
    });
    it('should set and get cookies', async({page, server}) => {
      await page.goto(server.PREFIX + '/grid.html');
      expect(await page.cookies()).toEqual([]);
      await page.evaluate(() => {
        document.cookie = 'username=John Doe';
      });
      expect(await page.cookies()).toEqual([{
        name: 'username',
        value: 'John Doe',
        domain: 'localhost',
        path: '/',
        expires: -1,
        size: 16,
        httpOnly: false,
        secure: false,
        session: true }
      ]);
      await page.setCookie({
        name: 'password',
        value: '123456'
      });
      expect(await page.evaluate('document.cookie')).toBe('username=John Doe; password=123456');
      const cookies = await page.cookies();
      expect(cookies.sort((a, b) => a.name.localeCompare(b.name))).toEqual([{
        name: 'password',
        value: '123456',
        domain: 'localhost',
        path: '/',
        expires: -1,
        size: 14,
        httpOnly: false,
        secure: false,
        session: true
      }, {
        name: 'username',
        value: 'John Doe',
        domain: 'localhost',
        path: '/',
        expires: -1,
        size: 16,
        httpOnly: false,
        secure: false,
        session: true
      }]);
    });

    it('should set a cookie with a path', async({page, server}) => {
      await page.goto(server.PREFIX + '/grid.html');
      await page.setCookie({
        name: 'gridcookie',
        value: 'GRID',
        path: '/grid.html'
      });
      expect(await page.cookies()).toEqual([{
        name: 'gridcookie',
        value: 'GRID',
        domain: 'localhost',
        path: '/grid.html',
        expires: -1,
        size: 14,
        httpOnly: false,
        secure: false,
        session: true
      }]);
      expect(await page.evaluate('document.cookie')).toBe('gridcookie=GRID');
      await page.goto(server.PREFIX + '/empty.html');
      expect(await page.cookies()).toEqual([]);
      expect(await page.evaluate('document.cookie')).toBe('');
      await page.goto(server.PREFIX + '/grid.html');
      expect(await page.evaluate('document.cookie')).toBe('gridcookie=GRID');
    });


    it('should delete a cookie', async({page, server}) => {
      await page.goto(server.PREFIX + '/grid.html');
      await page.setCookie({
        name: 'cookie1',
        value: '1'
      }, {
        name: 'cookie2',
        value: '2'
      }, {
        name: 'cookie3',
        value: '3'
      });
      expect(await page.evaluate('document.cookie')).toBe('cookie1=1; cookie2=2; cookie3=3');
      await page.deleteCookie({name: 'cookie2'});
      expect(await page.evaluate('document.cookie')).toBe('cookie1=1; cookie3=3');
    });

    it('should not set a cookie on a blank page', async function({page}) {
      let error = null;
      await page.goto('about:blank');
      try {
        await page.setCookie({name: 'example-cookie', value: 'best'});
      } catch (e) {
        error = e;
      }
      expect(error).toBeTruthy();
      expect(error.message).toEqual('Protocol error (Network.deleteCookies): At least one of the url and domain needs to be specified undefined');
    });

    it('should not set a cookie with blank page URL', async function({page, server}) {
      let error = null;
      await page.goto(server.PREFIX + '/grid.html');
      try {
        await page.setCookie(
            {name: 'example-cookie', value: 'best'},
            {url: 'about:blank', name: 'example-cookie-blank', value: 'best'}
        );
      } catch (e) {
        error = e;
      }
      expect(error).toBeTruthy();
      expect(error.message).toEqual(
          `Blank page can not have cookie "example-cookie-blank"`
      );
    });

    it('should not set a cookie on a data URL page', async function({page}) {
      let error = null;
      await page.goto('data:,Hello%2C%20World!');
      try {
        await page.setCookie({name: 'example-cookie', value: 'best'});
      } catch (e) {
        error = e;
      }
      expect(error).toBeTruthy();
      expect(error.message).toEqual(
          'Protocol error (Network.deleteCookies): At least one of the url and domain needs to be specified undefined'
      );
    });

    it('should not set a cookie with blank page URL', async function({page, server}) {
      let error = null;
      await page.goto(server.PREFIX + '/grid.html');
      try {
        await page.setCookie({name: 'example-cookie', value: 'best'}, {url: 'about:blank', name: 'example-cookie-blank', value: 'best'});
      } catch (e) {
        error = e;
      }
      expect(error).toBeTruthy();
      expect(error.message).toEqual(`Blank page can not have cookie "example-cookie-blank"`);
    });

    it('should set a cookie on a different domain', async({page, server}) => {
      await page.goto(server.PREFIX + '/grid.html');
      await page.setCookie({name: 'example-cookie', value: 'best',  url: 'https://www.example.com'});
      expect(await page.evaluate('document.cookie')).toBe('');
      expect(await page.cookies()).toEqual([]);
      expect(await page.cookies('https://www.example.com')).toEqual([{
        name: 'example-cookie',
        value: 'best',
        domain: 'www.example.com',
        path: '/',
        expires: -1,
        size: 18,
        httpOnly: false,
        secure: true,
        session: true
      }]);
    });

    it('should set cookies from a frame', async({page, server}) => {
      await page.goto(server.PREFIX + '/grid.html');
      await page.setCookie({name: 'localhost-cookie', value: 'best'});
      await page.evaluate(src => {
        let fulfill;
        const promise = new Promise(x => fulfill = x);
        const iframe = document.createElement('iframe');
        document.body.appendChild(iframe);
        iframe.onload = fulfill;
        iframe.src = src;
        return promise;
      }, server.CROSS_PROCESS_PREFIX);
      await page.setCookie({name: '127-cookie', value: 'worst', url: server.CROSS_PROCESS_PREFIX});
      expect(await page.evaluate('document.cookie')).toBe('localhost-cookie=best');
      expect(await page.frames()[1].evaluate('document.cookie')).toBe('127-cookie=worst');

      expect(await page.cookies()).toEqual([{
        name: 'localhost-cookie',
        value: 'best',
        domain: 'localhost',
        path: '/',
        expires: -1,
        size: 20,
        httpOnly: false,
        secure: false,
        session: true
      }]);

      expect(await page.cookies(server.CROSS_PROCESS_PREFIX)).toEqual([{
        name: '127-cookie',
        value: 'worst',
        domain: '127.0.0.1',
        path: '/',
        expires: -1,
        size: 15,
        httpOnly: false,
        secure: false,
        session: true
      }]);

    });
  });
         */

        [Fact]
        public async Task ShouldGetAndSetCookies()
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.Empty(await page.GetCookiesAsync());

            await page.EvaluateFunctionAsync(@"() =>
            {
                document.cookie = 'username=John Doe';
            }");
            var cookie = Assert.Single(await page.GetCookiesAsync());
            Assert.Equal(cookie.Name, "username");
            Assert.Equal(cookie.Value, "John Doe");
            Assert.Equal(cookie.Domain, "localhost");
            Assert.Equal(cookie.Path, "/");
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(cookie.Size, 16);
            Assert.Equal(cookie.HttpOnly, false);
            Assert.Equal(cookie.Secure, false);
            Assert.Equal(cookie.Session, true);

            await page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });
            Assert.Equal("username=John Doe; password=123456", await page.EvaluateExpressionAsync<string>("document.cookie"));
            var cookies = (await page.GetCookiesAsync()).OrderBy(c => c.Name).ToList();
            Assert.Equal(2, cookies.Count);

            Assert.Equal(cookies[0].Name, "password");
            Assert.Equal(cookies[0].Value, "123456");
            Assert.Equal(cookies[0].Domain, "localhost");
            Assert.Equal(cookies[0].Path, "/");
            Assert.Equal(cookies[0].Expires, -1);
            Assert.Equal(cookies[0].Size, 14);
            Assert.Equal(cookies[0].HttpOnly, false);
            Assert.Equal(cookies[0].Secure, false);
            Assert.Equal(cookies[0].Session, true);

            Assert.Equal(cookies[1].Name, "username");
            Assert.Equal(cookies[1].Value, "John Doe");
            Assert.Equal(cookies[1].Domain, "localhost");
            Assert.Equal(cookies[1].Path, "/");
            Assert.Equal(cookies[1].Expires, -1);
            Assert.Equal(cookies[1].Size, 16);
            Assert.Equal(cookies[1].HttpOnly, false);
            Assert.Equal(cookies[1].Secure, false);
            Assert.Equal(cookies[1].Session, true);
        }

        [Fact]
        public async Task ShouldSetACookieWithAPath()
        {

        }

        [Fact]
        public async Task ShouldDeleteACookie()
        {

        }

        [Fact]
        public async Task ShouldNotSetACookieOnABlankPage()
        {

        }

        [Fact]
        public async Task ShouldNotSetACookieWithBlankPageURL()
        {

        }

        [Fact]
        public async Task ShouldNotSetACookieOnADataURLPage()
        {

        }

        [Fact] // need a better name for this one
        public async Task ShouldNotSetACookieWithBlankPageURL2()
        {

        }

        [Fact]
        public async Task ShouldSetACookieOnADifferentDomain()
        {

        }

        [Fact]
        public async Task ShouldSetCookiesFromAFrame()
        {

        }
    }
}
