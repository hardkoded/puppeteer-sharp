/**
 * Canonical host and base path. `base` is also read by the remark plugin and the redirect pages.
 *
 * Keep both at the root. www.puppeteersharp.com and the Vercel project serve this
 * build at `/`. Astro writes that base into every link, asset, and Pagefind bundle,
 * so one build cannot also live under /puppeteer-sharp/. The hardkoded.com path
 * redirects to this origin; see README.md.
 */
export const siteOrigin = "https://www.puppeteersharp.com";
export const siteBase = "/";
