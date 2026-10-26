import { mkdir, readFile, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { redirectHtml } from "./redirect.mjs";

const docsSite = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const repoRoot = path.resolve(docsSite, "..");
const contentDir = path.join(docsSite, "src/content/docs");
const articlesDir = path.join(repoRoot, "docfx_project/docs");
const editBase = "https://github.com/hardkoded/puppeteer-sharp/edit/master/";

const groups = [
    {
        label: "Basic Examples",
        items: [
            ["Page.ScreenshotAsync.md", "How to take screenshots and save it as a file"],
            ["Page.EvaluateExpressionAsync.GetAllLinks.md", "How to extract all the link from a page"],
            ["Page.EvaluateFunctionAsync.ComplexJSObjects.md", "How to map complex javascript objects to .NET objects"],
            ["Page.WaitForSelectorAsync.Searching.md", "How to wait for a selector"],
            ["ReuseChrome.md", "Download and Reuse Chrome from a custom location"],
            ["ChromeExtension.md", "How to test a Chrome Extension"],
            ["Page.WaitForFunctionAsync.md", "How to wait for a javascript expression to be true"],
            ["Page.Request.md", "How to log network requests"],
            ["DownloadFetcher.Download.md", "How to download an specific browser"],
            ["AOT.md", "Using PuppeteerSharp with AOT compilation"],
        ],
    },
    {
        label: "Locators",
        items: [
            ["Locators.md", "Getting Started with Locators"],
            ["LocatorActions.md", "Locator Actions"],
        ],
    },
    {
        label: "Advanced",
        items: [["LogCDPCommunication.md", "How to log CDP communication"]],
    },
    {
        label: "Troubleshooting",
        items: [["IssuesGeneratingPdfFiles.md", "Issues generating PDF files"]],
    },
];

const articles = groups.flatMap((group) => group.items.map(([file, title]) => ({ file, title })));

const pages = [
    {
        source: path.join(repoRoot, "docfx_project/index.md"),
        output: "index.md",
        title: "Browser Automation for .NET",
        label: "Home",
        order: 0,
        editPath: "docfx_project/index.md",
    },
    {
        source: path.join(articlesDir, "index.md"),
        output: path.join("docs", "index.md"),
        title: "Examples",
        label: "Examples",
        editPath: "docfx_project/docs/index.md",
    },
    ...articles.map((article) => ({
        source: path.join(articlesDir, article.file),
        output: path.join("docs", article.file),
        title: article.title,
        label: article.title,
        editPath: `docfx_project/docs/${article.file}`,
        legacy: `docs/${article.file.replace(/\.md$/, ".html")}`,
        target: `/docs/${article.file.replace(/\.md$/, "")}/`,
    })),
];

const pageTargets = new Map();
for (const page of pages) {
    const fileName = path.basename(page.source);
    if (fileName === "index.md" && page.output === "index.md") continue;
    pageTargets.set(fileName, page.target);
}

const sectionRedirects = [
    ["toc.html", "/"],
    ["docs/toc.html", "/docs/"],
    ["docs.html", "/docs/"],
];

await mkdir(contentDir, { recursive: true });

for (const page of pages) {
    const raw = await readFile(page.source, "utf8");
    const body = rewriteLinks(stripTitle(raw.replace(/^\uFEFF/, "")));
    const description = describe(body, page.title);
    const frontmatter = [
        "---",
        `title: ${JSON.stringify(page.title)}`,
        `description: ${JSON.stringify(description)}`,
        `editUrl: ${JSON.stringify(`${editBase}${page.editPath}`)}`,
        "sidebar:",
        `  label: ${JSON.stringify(page.label)}`,
        page.order === undefined ? null : `  order: ${page.order}`,
        "---",
        "",
    ]
        .filter((line) => line !== null)
        .join("\n");
    const destination = path.join(contentDir, page.output);
    await mkdir(path.dirname(destination), { recursive: true });
    await writeFile(destination, `${frontmatter}${body.trim()}\n`);
    if (page.legacy && page.target) await writeLegacy(page.legacy, page.target);
}

for (const [legacy, target] of sectionRedirects) {
    await writeLegacy(legacy, target);
}

const sidebar = [
    { label: "Examples", slug: "docs" },
    ...groups.map((group) => ({
        label: group.label,
        items: group.items.map(([file, label]) => ({
            label,
            slug: `docs/${file.replace(/\.md$/, "")}`,
        })),
    })),
];
await mkdir(path.join(docsSite, "src/generated"), { recursive: true });
await writeFile(path.join(docsSite, "src/generated/guides-sidebar.json"), `${JSON.stringify(sidebar, null, 2)}\n`);

console.log(`Synced ${pages.length} guide pages into ${path.relative(repoRoot, contentDir)}`);

function stripTitle(markdown) {
    return markdown.replace(/^#\s+[^\n]*\n+/, "");
}

function describe(markdown, fallback) {
    const paragraph = markdown
        .split(/\n\s*\n/)
        .map((block) => block.trim())
        .find((block) => block && !block.startsWith("#") && !block.startsWith("```") && !block.startsWith("|") && !block.startsWith("<") && !block.startsWith("<!--") && !/^_?Contributors:/i.test(block));
    const text = (paragraph ?? fallback)
        .replace(/\[([^\]]+)\]\([^)]+\)/g, "$1")
        .replace(/[`*_]/g, "")
        .replace(/\s+/g, " ")
        .trim();
    return text.length > 160 ? `${text.slice(0, 157).trimEnd()}...` : text;
}

function rewriteLinks(markdown) {
    return markdown.replace(/\[([^\]]+)\]\(([^)\s]+)\)/g, (full, text, href) => {
        if (/^[a-z][a-z0-9+.-]*:/i.test(href) || href.startsWith("#") || href.startsWith("/")) return full;
        const hashAt = href.indexOf("#");
        const file = hashAt >= 0 ? href.slice(0, hashAt) : href;
        const hash = hashAt >= 0 ? href.slice(hashAt) : "";
        const normalized = file.replace(/^\.\//, "");
        const target = pageTargets.get(normalized);
        if (!target) return full;
        return `[${text}](${target}${hash})`;
    });
}

async function writeLegacy(relativePath, target) {
    const destination = path.join(docsSite, "public", relativePath);
    await mkdir(path.dirname(destination), { recursive: true });
    await writeFile(destination, redirectHtml(target));
}
