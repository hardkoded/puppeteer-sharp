# Puppeteer Sharp docs

The documentation site is [Astro Starlight](https://starlight.astro.build/). The published site is [https://www.puppeteersharp.com/](https://www.puppeteersharp.com/).

## Preview locally

From this directory:

```bash
npm install
npm run dev
```

Open http://localhost:4321/ . The site is served at the root because `siteBase` in `site.mjs` is `/`.

Search is built with the production output:

```bash
npm run build
npm run preview
```

`npm run dev` and `npm run build` copy `docfx_project/index.md` and `docfx_project/docs/` into the site before Astro starts. Those Markdown files stay the MarkdownSnippets source of truth (`on-push-do-docs.yml` still runs `mdsnippets` on the repo). Snippet updates land in the published pages on the next site build.

## Regenerate the API reference

The API pages are generated. They are not edited by hand.

```bash
npm run api
```

That needs the .NET 10 SDK. It restores the local DocFX tool (`dotnet-tools.json`, DocFX 2.80.1), runs `docfx metadata` on `lib/PuppeteerSharp/PuppeteerSharp.csproj` for `net10.0`, and writes Markdown into `src/content/docs/api/` plus the collapsed sidebar at `src/generated/api-sidebar.json`. Commit the generated pages and the sidebar file.

`npm run api -- --skip-metadata` rebuilds the Markdown from `docs-site/.api-yaml` without running DocFX again.

## Hosting

`site.mjs` sets `siteOrigin` to `https://www.puppeteersharp.com` and `siteBase` to `/`. The production build is a static site at that root. Vercel serves it with no `vercel.json` and no `@astrojs/vercel` adapter: clean URLs, trailing slashes, Pagefind (`baseUrl: "/"`), and `404.html` already resolve. The live project is `puppeteer-sharp`, with root directory `docs-site`, at https://puppeteer-sharp.vercel.app/.

`www.hardkoded.com/puppeteer-sharp/` is a redirect to this origin. The other Hardkoded docs are built with a `/<repo>` base because that path is their only public URL. This site already has a custom domain at `/`, and Astro bakes `base` into links, `/_astro` assets, and the Pagefind bundle. A multi-zone rewrite that fetches this project's root would leave the browser requesting those root paths on the blog host. Keep the redirect on the blog project (`hardkoded`, repo `kblok/kblok.github.io`):

```json
{
  "redirects": [
    {
      "source": "/puppeteer-sharp",
      "destination": "https://www.puppeteersharp.com/",
      "permanent": true
    },
    {
      "source": "/puppeteer-sharp/",
      "destination": "https://www.puppeteersharp.com/",
      "permanent": true
    },
    {
      "source": "/puppeteer-sharp/:path*",
      "destination": "https://www.puppeteersharp.com/:path*",
      "permanent": true
    }
  ]
}
```

### Move www.puppeteersharp.com to this Vercel project

DNS for `puppeteersharp.com` is Azure DNS (`ns1-09.azure-dns.com`, `ns2-09.azure-dns.net`, `ns3-09.azure-dns.org`, `ns4-09.azure-dns.info`). `www` is a CNAME to `kblok.github.io`, which is why GitHub Pages still serves the domain. The apex has no address record. Leave the nameservers in place so mail and other records stay put.

1. In Vercel project **puppeteer-sharp**, open **Settings → Domains**. Add `www.puppeteersharp.com`. Add `puppeteersharp.com` and redirect it to `www`.
2. Copy the records Vercel shows for this project. `www` is a CNAME to that project's `*.vercel-dns-*.com` host. The apex is an A record; Vercel's anycast address for apex domains is `76.76.21.21` when the domain card shows that value.
3. In Azure DNS, replace the `www` CNAME (`kblok.github.io`) with the Vercel CNAME. Add the apex A record when the apex domain is on the project.
4. Wait until Vercel marks the domain valid. `https://www.puppeteersharp.com/` and `https://puppeteer-sharp.vercel.app/` then serve the same deployment. Canonical URLs stay on `www.puppeteersharp.com`.
5. Remove the custom domain from the GitHub Pages settings of this repository. `public/CNAME` is what Pages uses to claim the host; delete that file in a follow-up once Pages no longer lists the domain. Vercel does not read it. The Pages workflow can keep building until then.

Change DNS only after the Vercel production deployment is Ready, so the cutover is the DNS TTL and not a missing site.

## GitHub Pages

`.github/workflows/docs.yml` builds this site on every pull request and on every push to `master`. It deploys only from a push to `master`, using GitHub Actions (`actions/deploy-pages`). That deployment is what `www.puppeteersharp.com` serves until the DNS cutover above.

In the repository settings, GitHub Pages **Source** must be **GitHub Actions**, not the `gh-pages` branch. The retired `publish-docs.yml` workflow no longer deploys on tags, so a tag push cannot overwrite this deployment.

Old DocFX `.html` URLs redirect to the new paths. Member anchors from DocFX (`#PuppeteerSharp_Page_GoToAsync_...`) are kept on the generated pages. A redirect preserves the hash when JavaScript runs.
