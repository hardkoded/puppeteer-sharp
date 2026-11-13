// Refreshes the data behind the /sponsors/ page of the docs site:
//   docs-site/src/data/contributors.json  (all human contributors, from the REST API)
//   docs-site/src/data/sponsors.json      ("github" list only, from GitHub Sponsors)
// The "featured" list in sponsors.json is curated by hand (companies, one-time sponsors, logos)
// and is never touched here.
//
// Env: GITHUB_TOKEN (contributors), SPONSORS_TOKEN (read:user + sponsors scope; optional).
import { readFile, writeFile } from "node:fs/promises";

const owner = "hardkoded";
const repo = "puppeteer-sharp";
const dataDir = new URL("../../docs-site/src/data/", import.meta.url);
const excludedLogins = new Set(["actions-user"]);

async function rest(path, token) {
    const headers = { Accept: "application/vnd.github+json", "User-Agent": "puppeteer-sharp-site" };
    if (token) headers.Authorization = `Bearer ${token}`;
    const response = await fetch(`https://api.github.com${path}`, { headers });
    if (!response.ok) throw new Error(`${path}: ${response.status}`);
    return response.json();
}

async function contributors() {
    const all = [];
    for (let page = 1; ; page++) {
        const batch = await rest(`/repos/${owner}/${repo}/contributors?per_page=100&page=${page}`, process.env.GITHUB_TOKEN);
        all.push(...batch);
        if (batch.length < 100) break;
    }
    return all
        .filter((c) => c.type === "User" && !excludedLogins.has(c.login))
        .map((c) => ({ contributions: c.contributions, id: c.id, login: c.login, url: c.html_url }));
}

async function githubSponsors(token) {
    const query = `query($login: String!) {
      repositoryOwner(login: $login) {
        ... on Sponsorable {
          sponsorshipsAsMaintainer(first: 100, includePrivate: false, activeOnly: true) {
            nodes { sponsorEntity { ... on User { login name url } ... on Organization { login name url } } }
          }
        }
      }
    }`;
    const response = await fetch("https://api.github.com/graphql", {
        method: "POST",
        headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json", "User-Agent": "puppeteer-sharp-site" },
        body: JSON.stringify({ query, variables: { login: owner } }),
    });
    const body = await response.json();
    if (!response.ok || body.errors) throw new Error(`sponsors query failed: ${JSON.stringify(body.errors ?? response.status)}`);
    return body.data.repositoryOwner.sponsorshipsAsMaintainer.nodes
        .map((n) => n.sponsorEntity)
        .filter(Boolean)
        .map((s) => ({ login: s.login, name: s.name || s.login, url: s.url }));
}

await writeFile(new URL("contributors.json", dataDir), JSON.stringify(await contributors(), null, 2) + "\n");

const sponsorsFile = new URL("sponsors.json", dataDir);
const current = JSON.parse(await readFile(sponsorsFile, "utf8"));
if (process.env.SPONSORS_TOKEN) {
    current.github = await githubSponsors(process.env.SPONSORS_TOKEN);
    await writeFile(sponsorsFile, JSON.stringify(current, null, 2) + "\n");
} else {
    console.warn("SPONSORS_TOKEN not set; keeping the existing GitHub sponsors list.");
}
