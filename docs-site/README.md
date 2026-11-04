# Puppeteer Sharp docs

The documentation site is [Astro Starlight](https://starlight.astro.build/). The published site root is a dark marketing page at [https://www.puppeteersharp.com/](https://www.puppeteersharp.com/). Guides and the API reference start at [/docs/](https://www.puppeteersharp.com/docs/).

## Preview locally

From this directory:

```bash
npm ci
npm run dev
```

Open http://localhost:4321/ for the landing page, and http://localhost:4321/docs/ for the guides. The path is the site root because `siteBase` in `site.mjs` is `/`. The landing page stays dark. Guide and API pages still follow the system color scheme.

Search is built with the production output:

```bash
npm run build
npm run preview
```

`npm run dev` and `npm run build` copy `docfx_project/index.md` to `src/content/docs/docs/index.md` (the docs home) and `docfx_project/docs/` into that same directory. The example catalog is `docs/examples.md`. Those Markdown files stay the MarkdownSnippets source of truth (`on-push-do-docs.yml` still runs `mdsnippets` on the repo). Snippet updates land in the published pages on the next site build. The site root is the marketing page in `src/pages/index.astro`.

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

### DNS

DNS for `puppeteersharp.com` is Azure DNS (`ns1-09.azure-dns.com`, `ns2-09.azure-dns.net`, `ns3-09.azure-dns.org`, `ns4-09.azure-dns.info`). The nameservers stay in place so mail and the other records are unchanged.

The cutover to Vercel is complete. Azure DNS has:

- Apex `A` records `216.198.79.1` and `64.29.17.1`
- A `www` CNAME to `6df6a3e9cedb0ea5.vercel-dns-017.com`

Vercel project **puppeteer-sharp** serves `https://www.puppeteersharp.com/` and `https://puppeteersharp.com/`. Canonical URLs stay on `www.puppeteersharp.com`. `https://puppeteer-sharp.vercel.app/` is the same deployment. Vercel publishes that deployment on push to `master`.

Old DocFX `.html` URLs redirect to the new paths. The root DocFX table of contents (`toc.html`) lands on `/docs/`, because the site root is the marketing page. Member anchors from DocFX (`#PuppeteerSharp_Page_GoToAsync_...`) are kept on the generated pages. A redirect preserves the hash when JavaScript runs.

## GitHub Pages

GitHub Pages is retired for this site. `.github/workflows/docs.yml` still builds on every pull request and on every push to `master`, and uploads the `docs-site` artifact for CI. It does not deploy. `actions/upload-pages-artifact` and `actions/deploy-pages` are gone.

`docs-site/public/CNAME` was removed. That file is what Pages used to claim `www.puppeteersharp.com`. Vercel does not read it.

Repository Pages settings will be disabled separately after this change merges, so a leftover Pages source cannot publish over the Vercel site. The retired `publish-docs.yml` workflow no longer deploys on tags.
