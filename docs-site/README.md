# Puppeteer Sharp docs

The documentation site is [Astro Starlight](https://starlight.astro.build/). The published site is [https://www.puppeteersharp.com/](https://www.puppeteersharp.com/).

## Preview locally

From this directory:

```bash
npm install
npm run dev
```

Open http://localhost:4321/ . The site is served at the root because `www.puppeteersharp.com` is a custom-domain GitHub Pages site, not a project subpath.

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

## GitHub Pages

`.github/workflows/docs.yml` builds this site on every pull request and on every push to `master`. It deploys only from a push to `master`, using GitHub Actions (`actions/deploy-pages`). Merging to `master` is what publishes the site.

The custom domain is `www.puppeteersharp.com` (`public/CNAME`). In the repository settings, GitHub Pages **Source** must be **GitHub Actions**, not the `gh-pages` branch. The retired `publish-docs.yml` workflow no longer deploys on tags, so a tag push cannot overwrite this deployment. After switching the source, confirm the custom domain is still `www.puppeteersharp.com`.

Old DocFX `.html` URLs redirect to the new paths. Member anchors from DocFX (`#PuppeteerSharp_Page_GoToAsync_...`) are kept on the generated pages. A redirect preserves the hash when JavaScript runs.
