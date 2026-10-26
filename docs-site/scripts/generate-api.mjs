import { spawn } from "node:child_process";
import { mkdir, readdir, readFile, rm, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { parseAllDocuments } from "yaml";
import { redirectHtml } from "./redirect.mjs";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const yamlDir = path.join(root, ".api-yaml");
const staging = path.join(root, "src/content/docs/.api-staging");
const apiDir = path.join(root, "src/content/docs/api");
const sidebarFile = path.join(root, "src/generated/api-sidebar.json");
const redirectStaging = path.join(root, ".redirect-staging");
const redirectDir = path.join(root, "public/api");
const skipMetadata = process.argv.includes("--skip-metadata");

const typeKinds = new Set(["Class", "Struct", "Interface", "Enum", "Delegate", "Record"]);
const kindOrder = ["Class", "Record", "Struct", "Interface", "Enum", "Delegate"];
const kindHeading = {
    Class: "Classes",
    Record: "Records",
    Struct: "Structs",
    Interface: "Interfaces",
    Enum: "Enums",
    Delegate: "Delegates",
};
const memberGroups = [
    ["Constructor", "Constructors"],
    ["Field", "Fields"],
    ["Property", "Properties"],
    ["Event", "Events"],
    ["Method", "Methods"],
    ["Operator", "Operators"],
];
const namespaceBlurbs = {
    PuppeteerSharp: "Browser, page, frame, element, network, and launch APIs.",
    "PuppeteerSharp.Input": "Keyboard, mouse, and touch input.",
    "PuppeteerSharp.Media": "Screenshots, PDF paper formats, and margin options.",
    "PuppeteerSharp.Mobile": "Device descriptors for mobile emulation.",
    "PuppeteerSharp.PageAccessibility": "Accessibility tree snapshots.",
    "PuppeteerSharp.PageCoverage": "JavaScript and CSS coverage.",
    "PuppeteerSharp.Cdp": "Chrome DevTools Protocol browser and page implementation.",
    "PuppeteerSharp.Cdp.Messaging": "Public Chrome DevTools Protocol message types.",
    "PuppeteerSharp.Bidi": "WebDriver BiDi browser and page implementation.",
    "PuppeteerSharp.Locators": "Auto-waiting locators for finding and acting on elements.",
    "PuppeteerSharp.BrowserData": "Browser versions, builds, and download metadata.",
    "PuppeteerSharp.QueryHandlers": "CSS, XPath, text, and pierce query handlers.",
    "PuppeteerSharp.Transport": "WebSocket and pipe transports.",
    "PuppeteerSharp.Helpers": "Shared helpers used by the public API.",
};
const assemblyOrder = ["PuppeteerSharp"];

process.env.PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD ??= "1";
process.env.DOTNET_CLI_TELEMETRY_OPTOUT ??= "1";
process.env.DOTNET_NOLOGO ??= "1";

if (!skipMetadata) {
    await run("dotnet", ["tool", "restore"]);
    await run("dotnet", ["tool", "run", "docfx", "--", "metadata", "docfx.json"]);
}

const byUid = new Map();
const refs = new Map();
for await (const file of walk(yamlDir)) {
    if (!file.endsWith(".yml") && !file.endsWith(".yaml")) continue;
    if (path.basename(file) === "toc.yml") continue;
    const documents = parseYaml(await readFile(file, "utf8"), file);
    for (const data of documents) {
        if (!data || typeof data !== "object") continue;
        for (const item of data.items ?? []) {
            if (item?.uid) byUid.set(item.uid, item);
        }
        for (const ref of data.references ?? []) {
            if (ref?.uid && !refs.has(ref.uid)) refs.set(ref.uid, ref);
        }
    }
}

const namespaces = [...byUid.values()].filter((item) => item.type === "Namespace" && isPuppeteerSharpUid(item.uid));
const types = [...byUid.values()].filter((item) => typeKinds.has(item.type) && isDocumentedType(item));
if (types.length === 0) {
    throw new Error(`No public PuppeteerSharp types were found in ${yamlDir}. DocFX metadata did not produce an API model.`);
}

const namespacePage = new Map();
const typePage = new Map();
const emittedOverload = new Set();
for (const ns of namespaces) {
    if (types.some((type) => type.namespace === ns.uid)) namespacePage.set(ns.uid, `api/${ns.uid}`);
}
for (const type of types) {
    if (!namespacePage.has(type.namespace)) namespacePage.set(type.namespace, `api/${type.namespace}`);
    typePage.set(type.uid, `api/${type.namespace}/${fileStem(type)}`);
}

await rm(staging, { recursive: true, force: true });
await mkdir(staging, { recursive: true });
await rm(redirectStaging, { recursive: true, force: true });
await mkdir(redirectStaging, { recursive: true });

const usedLegacy = new Map();
for (const [uid, pageId] of [...namespacePage, ...typePage]) {
    await writeLegacyRedirect(uid, `/${pageId}/`);
}
await writeFile(path.join(redirectStaging, "toc.html"), redirectHtml("/api/"));

const sidebar = [{ label: "Overview", slug: "api" }];
const namespacesByAssembly = new Map();
for (const ns of [...namespacePage.keys()].sort(compareUid)) {
    const sample = types.find((type) => type.namespace === ns);
    const assembly = sample?.assemblies?.[0] ?? ns.split(".")[0];
    if (!namespacesByAssembly.has(assembly)) namespacesByAssembly.set(assembly, []);
    namespacesByAssembly.get(assembly).push(ns);
    sidebar.push({ label: ns, slug: `api/${ns}` });
}

await writeFile(
    path.join(staging, "index.md"),
    renderApiIndex(namespacesByAssembly),
);

for (const ns of namespacePage.keys()) {
    const directory = path.join(staging, ns);
    await mkdir(directory, { recursive: true });
    await writeFile(path.join(directory, "index.md"), renderNamespace(ns));
    for (const type of types.filter((item) => item.namespace === ns)) {
        await writeFile(path.join(directory, `${fileStem(type)}.md`), renderType(type));
    }
}

await writeFile(sidebarFile, `${JSON.stringify(sidebar, null, 2)}\n`);
await rm(apiDir, { recursive: true, force: true });
await renameDirectory(staging, apiDir);
await rm(redirectDir, { recursive: true, force: true });
await renameDirectory(redirectStaging, redirectDir);

const required = [
    path.join(apiDir, "index.md"),
    path.join(apiDir, "PuppeteerSharp", "Page.md"),
    path.join(apiDir, "PuppeteerSharp", "Browser.md"),
    path.join(apiDir, "PuppeteerSharp", "Puppeteer.md"),
    path.join(apiDir, "PuppeteerSharp", "IPage.md"),
];
for (const file of required) {
    await readFile(file);
}

console.log(`Wrote ${typePage.size} type pages in ${namespacePage.size} namespaces.`);

function renderApiIndex(grouped) {
    const lines = [
        frontmatter({
            title: "API reference",
            description: "Public types in the PuppeteerSharp assembly.",
            editUrl: false,
            sidebar: { label: "Overview", order: 0 },
        }),
        "Generated from the XML documentation of `PuppeteerSharp`. Regenerate it with `npm run api` from `docs-site/` after a public API change. Guides and examples live under [Examples](/docs/).",
        "",
    ];
    const assemblies = [...grouped.keys()].sort(compareAssembly);
    for (const assembly of assemblies) {
        lines.push(`## ${assembly}`, "");
        for (const ns of grouped.get(assembly)) {
            const blurb = namespaceBlurbs[ns] || plain(byUid.get(ns)?.summary) || `Public types in the \`${ns}\` namespace.`;
            lines.push(`- [${ns}](/api/${ns}/) — ${blurb}`);
        }
        lines.push("");
    }
    return `${lines.join("\n").trim()}\n`;
}

function renderNamespace(ns) {
    const pageId = namespacePage.get(ns);
    const inNamespace = types.filter((type) => type.namespace === ns).sort((a, b) => compareUid(displayName(a), displayName(b)));
    const blurb = namespaceBlurbs[ns] || plain(byUid.get(ns)?.summary) || `Public types in the \`${ns}\` namespace.`;
    const lines = [
        frontmatter({
            title: ns,
            description: truncate(blurb, 160),
            editUrl: false,
            sidebar: { label: ns },
        }),
        `[API reference](/api/)`,
        "",
        blurb,
        "",
    ];
    for (const kind of kindOrder) {
        const group = inNamespace.filter((type) => type.type === kind);
        if (group.length === 0) continue;
        lines.push(`## ${kindHeading[kind]}`, "");
        lines.push("| Type | Summary |", "| --- | --- |");
        for (const type of group) {
            const summary = tableCell(plain(type.summary) || "");
            lines.push(`| ${linkToUid(pageId, type.uid, displayName(type))} | ${summary} |`);
        }
        lines.push("");
    }
    return `${lines.join("\n").trim()}\n`;
}

function renderType(type) {
    const pageId = typePage.get(type.uid);
    const summary = plain(type.summary);
    const lines = [
        frontmatter({
            title: displayName(type),
            description: truncate(summary || `${displayName(type)} in ${type.namespace}.`, 160),
            editUrl: false,
            prev: false,
            next: false,
            sidebar: { label: displayName(type), hidden: true },
        }),
        `<a id="${docfxAnchor(type.uid)}"></a>`,
        "",
        `Namespace: ${linkToUid(pageId, type.namespace)}`,
        "",
    ];
    if (typePagesParent(type)) {
        lines.push(`Declared in ${linkToUid(pageId, type.parent)}.`, "");
    }
    const signature = syntaxText(type);
    if (signature) lines.push(fence(signature), "");
    const renderedSummary = renderComment(type.summary, pageId);
    if (renderedSummary) lines.push(renderedSummary, "");
    pushBlock(lines, renderTypeParameters(type, pageId));
    pushBlock(lines, renderHierarchy(type, pageId));
    pushBlock(lines, renderParameters(type, pageId));
    pushBlock(lines, renderReturns(type, pageId, type.type === "Delegate" ? "Returns" : "Value"));
    pushBlock(lines, renderExceptions(type, pageId));
    pushBlock(lines, renderExtra(type, pageId));
    const source = sourceHref(type);
    if (source) lines.push(`[View source](${source})`, "");

    const children = (type.children ?? []).map((uid) => byUid.get(uid)).filter(Boolean);
    const visibleChildren = children.filter((child) => isVisibleMember(child, type));
    const nested = visibleChildren.filter((child) => typeKinds.has(child.type));
    const members = visibleChildren.filter((child) => !typeKinds.has(child.type) && !String(child.uid).endsWith("*"));
    if (nested.length > 0) {
        lines.push("## Nested types", "");
        for (const child of nested) lines.push(`- ${linkToUid(pageId, child.uid, displayName(child))}`);
        lines.push("");
    }
    const consumed = new Set();
    for (const [kind, heading] of memberGroups) {
        const group = members.filter((member) => member.type === kind);
        if (group.length === 0) continue;
        group.forEach((member) => consumed.add(member.uid));
        lines.push(`## ${heading}`, "");
        for (const member of group) lines.push(renderMember(member, pageId));
    }
    const rest = members.filter((member) => !consumed.has(member.uid));
    if (rest.length > 0) {
        lines.push("## Members", "");
        for (const member of rest) lines.push(renderMember(member, pageId));
    }
    return `${lines.join("\n").trim()}\n`;
}

function renderMember(member, pageId) {
    const memberAnchor = docfxAnchor(member.uid);
    const anchors = [`<a id="${memberAnchor}"></a>`];
    const overload = member.overload ? String(member.overload) : "";
    if (overload && !emittedOverload.has(overload)) {
        emittedOverload.add(overload);
        const overloadAnchor = docfxAnchor(overload);
        if (overloadAnchor !== memberAnchor) anchors.unshift(`<a id="${overloadAnchor}"></a>`);
    }
    const heading = memberHeading(member);
    const parts = [anchors.join("\n"), "", `### ${heading}`, ""];
    const signature = syntaxText(member);
    if (signature) parts.push(fence(signature), "");
    const summary = renderComment(member.summary, pageId);
    if (summary) parts.push(summary, "");
    pushBlock(parts, renderTypeParameters(member, pageId));
    pushBlock(parts, renderParameters(member, pageId));
    const valueLabel = member.type === "Method" || member.type === "Operator" || member.type === "Constructor" ? "Returns" : "Value";
    pushBlock(parts, renderReturns(member, pageId, valueLabel));
    pushBlock(parts, renderExceptions(member, pageId));
    pushBlock(parts, renderExtra(member, pageId));
    const source = sourceHref(member);
    if (source) parts.push(`[View source](${source})`, "");
    return parts.join("\n");
}

function renderHierarchy(item, pageId) {
    const lines = [];
    if (item.inheritance?.length) {
        lines.push(`**Inheritance:** ${item.inheritance.map((uid) => linkToUid(pageId, uid)).join(" → ")}`);
    }
    if (item.implements?.length) {
        lines.push(`**Implements:** ${item.implements.map((uid) => linkToUid(pageId, uid)).join(", ")}`);
    }
    if (item.derivedClasses?.length) {
        lines.push(`**Derived:** ${item.derivedClasses.map((uid) => linkToUid(pageId, uid)).join(", ")}`);
    }
    return lines.join("\n\n");
}

function renderTypeParameters(item, pageId) {
    const parameters = item.syntax?.typeParameters ?? [];
    if (parameters.length === 0) return "";
    const lines = ["**Type parameters**", "", "| Name | Description |", "| --- | --- |"];
    for (const parameter of parameters) {
        lines.push(`| \`${parameter.id}\` | ${tableCell(renderComment(parameter.description, pageId))} |`);
    }
    return lines.join("\n");
}

function renderParameters(item, pageId) {
    const parameters = item.syntax?.parameters ?? [];
    if (parameters.length === 0) return "";
    const lines = ["**Parameters**", "", "| Name | Type | Description |", "| --- | --- | --- |"];
    for (const parameter of parameters) {
        const type = parameter.type ? linkToUid(pageId, parameter.type) : "";
        lines.push(`| \`${parameter.id}\` | ${type} | ${tableCell(renderComment(parameter.description, pageId))} |`);
    }
    return lines.join("\n");
}

function renderReturns(item, pageId, label) {
    const value = item.syntax?.return;
    if (!value?.type && !value?.description) return "";
    const type = value.type ? linkToUid(pageId, value.type) : "";
    const description = renderComment(value.description, pageId);
    if (!description) return `**${label}:** ${type}`;
    return `**${label}:** ${type}${type ? " — " : ""}${description}`;
}

function renderExceptions(item, pageId) {
    const exceptions = item.exceptions ?? [];
    if (exceptions.length === 0) return "";
    const lines = ["**Exceptions**", "", "| Type | Condition |", "| --- | --- |"];
    for (const exception of exceptions) {
        lines.push(`| ${linkToUid(pageId, exception.type || exception.commentId || "")} | ${tableCell(renderComment(exception.description, pageId))} |`);
    }
    return lines.join("\n");
}

function renderExtra(item, pageId) {
    const parts = [];
    if (item.obsolete || /\[Obsolete/.test(syntaxText(item))) {
        const message = typeof item.obsolete === "string" ? renderComment(item.obsolete, pageId) : "This API is obsolete.";
        parts.push(`:::caution[Obsolete]\n${message}\n:::`);
    }
    const remarks = renderComment(item.remarks, pageId);
    if (remarks) parts.push(remarks);
    const example = renderComment(item.example, pageId);
    if (example) parts.push(`**Example**\n\n${example}`);
    return parts.join("\n\n");
}

function linkToUid(fromId, uid, text) {
    const id = String(uid ?? "").replace(/^([A-Z!]):/, "");
    const label = escapeLinkText(text || labelFor(id));
    const located = locate(id);
    if (located) {
        const href = hrefBetween(fromId, located.id, located.hash);
        return `[${label}](${href})`;
    }
    const ref = refs.get(id);
    if (ref?.href && /^https?:\/\//.test(ref.href)) return `[${label}](${ref.href})`;
    return `\`${label.replace(/`/g, "")}\``;
}

function locate(uid) {
    if (typePage.has(uid)) return { id: typePage.get(uid), hash: "" };
    if (namespacePage.has(uid)) return { id: namespacePage.get(uid), hash: "" };
    const item = byUid.get(uid);
    if (!item?.parent) return null;
    if (typePage.has(item.parent)) return { id: typePage.get(item.parent), hash: `#${docfxAnchor(uid)}` };
    return null;
}

function hrefBetween(fromId, toId, hash = "") {
    if (fromId === toId) return hash || "./";
    return `/${toId}/${hash}`;
}

function labelFor(uid) {
    const ref = refs.get(uid);
    if (ref?.name) return ref.name;
    const item = byUid.get(uid);
    if (item) return displayName(item);
    const short = uid.split(".").pop() || uid;
    return short.replace(/`(\d+)/g, "<T>");
}

function displayName(item) {
    if (item.type === "Namespace") return item.uid;
    const ns = item.namespace ? `${item.namespace}.` : "";
    if (item.fullName?.startsWith(ns)) return item.fullName.slice(ns.length).replace(/\+/g, ".");
    return item.nameWithType || item.name || item.uid;
}

function memberHeading(member) {
    const name = member.name || displayName(member);
    return name.includes("<") || name.includes(">") || name.includes("`") ? `\`${name.replace(/`/g, "")}\`` : name;
}

function fileStem(type) {
    const ns = type.namespace ? `${type.namespace}.` : "";
    const uid = String(type.uid || "");
    let rest = uid.startsWith(ns) ? uid.slice(ns.length) : type.fullName?.startsWith(ns) ? type.fullName.slice(ns.length) : type.name || uid;
    // DocFX publishes E2E.ExploreBounds`1 as E2E.ExploreBounds-1.html. fullName is ExploreBounds<T>.
    rest = rest.replace(/\+/g, ".").replace(/`(\d+)/g, "-$1").replace(/[<>]/g, "");
    rest = rest.replace(/[^\w.-]+/g, "-").replace(/^[.-]+|[.-]+$/g, "");
    if (!rest || rest.toLowerCase() === "index") rest = `${rest || "type"}-type`;
    return rest;
}

function isDocumentedType(item) {
    if (!isPuppeteerSharpUid(item.namespace || item.uid)) return false;
    return isVisible(item, null);
}

function isVisibleMember(item, parent) {
    if (String(item.uid).endsWith("*")) return false;
    if (parent?.type === "Enum" && item.type === "Field") return true;
    return isVisible(item, parent);
}

function isVisible(item) {
    const mods = modifiers(item);
    if (mods.includes("private")) return false;
    if (mods.includes("public") || mods.includes("protected")) return true;
    if (mods.includes("internal")) return false;
    return /^\s*(?:\[[^\]]+\]\s*)*(public|protected)\b/m.test(syntaxText(item));
}

function modifiers(item) {
    const dotted = item["modifiers.csharp"];
    if (Array.isArray(dotted)) return dotted;
    if (Array.isArray(item.modifiers?.csharp)) return item.modifiers.csharp;
    return [];
}

function syntaxText(item) {
    const syntax = item.syntax;
    if (!syntax) return "";
    if (typeof syntax.content === "string") return syntax.content.trim();
    if (typeof syntax["content.csharp"] === "string") return syntax["content.csharp"].trim();
    return "";
}

function typePagesParent(type) {
    return type.parent && typePage.has(type.parent);
}

function isPuppeteerSharpUid(uid) {
    const value = String(uid || "");
    return value === "PuppeteerSharp" || value.startsWith("PuppeteerSharp.");
}

function sourceHref(item) {
    const source = item.source;
    if (!source) return "";
    let file = "";
    if (source.remote?.path) file = String(source.remote.path).replace(/\\/g, "/");
    else if (source.path) file = String(source.path).replace(/\\/g, "/");
    const match = file.match(/(?:^|\/)(lib\/PuppeteerSharp\/.+)$/);
    if (match) file = match[1];
    else if (file && !file.includes("..") && !file.startsWith("lib/")) file = `lib/PuppeteerSharp/${file.replace(/^\//, "")}`;
    if (!file || file.includes("..") || !file.startsWith("lib/PuppeteerSharp/")) return "";
    const repo = String(source.remote?.repo || "https://github.com/hardkoded/puppeteer-sharp.git").replace(/\.git$/, "");
    // Metadata records the branch DocFX ran on. Published links stay on master after that branch is gone.
    const branch = "master";
    let href = `${repo}/blob/${branch}/${file.replace(/^\//, "")}`;
    if (typeof source.startLine === "number") href += `#L${source.startLine + 1}`;
    return href;
}

function renderComment(value, fromId) {
    let text = asText(value).replace(/\r\n/g, "\n").trim();
    if (!text) return "";
    text = text.replace(/<pre><code(?:\s+class="lang-(\w+)")?>([\s\S]*?)<\/code><\/pre>/gi, (_, lang, code) => fence(decode(code), lang || "csharp"));
    text = text.replace(/\[([^\]]+)\]\(xref:([^)]+)\)/g, (_, label, uid) => linkToUid(fromId, uid, label));
    text = text.replace(/<xref href="([^"]+)"[^>]*>([\s\S]*?)<\/xref>/gi, (_, href, inner) => linkToUid(fromId, decode(href), decode(inner).trim() || undefined));
    text = text.replace(/<xref href="([^"]+)"[^>]*\/>/gi, (_, href) => linkToUid(fromId, decode(href)));
    text = text.replace(/<see(?:also)?\s+cref="([^"]+)"\s*\/>/gi, (_, cref) => linkToUid(fromId, cref));
    text = text.replace(/<para>([\s\S]*?)<\/para>/gi, (_, inner) => `\n\n${inner.trim()}\n\n`);
    text = text.replace(/<br\s*\/?>/gi, "\n");
    text = text.replace(/<(c|code)>([\s\S]*?)<\/\1>/gi, (_, _tag, code) => `\`${decode(code).replace(/`/g, "")}\``);
    text = text.replace(/<paramref\s+name="([^"]+)"\s*\/>/gi, "`$1`");
    text = text.replace(/<typeparamref\s+name="([^"]+)"\s*\/>/gi, "`$1`");
    text = text.replace(/<(strong|b)>([\s\S]*?)<\/\1>/gi, "**$2**");
    text = text.replace(/<(em|i)>([\s\S]*?)<\/\1>/gi, "*$2*");
    text = text.replace(/<list(?:\s+type="(bullet|number)")?>([\s\S]*?)<\/list>/gi, (_, type, inner) => listToMarkdown(inner, type === "number"));
    text = escapeAnglesOutsideCode(text).replace(/[ \t]+\n/g, "\n").trim();
    return text;
}

function listToMarkdown(inner, numbered) {
    const items = [...inner.matchAll(/<item>([\s\S]*?)<\/item>/gi)];
    if (items.length === 0) return inner;
    return `\n${items.map((match, index) => {
        const body = match[1].replace(/<\/?(description|term|para)>/gi, " ").replace(/\s+/g, " ").trim();
        return `${numbered ? `${index + 1}.` : "-"} ${body}`;
    }).join("\n")}\n`;
}

function escapeAnglesOutsideCode(markdown) {
    const parts = markdown.split(/(```[\s\S]*?```|`[^`\n]*`)/g);
    return parts.map((part, index) => (index % 2 === 1 ? part : part.replace(/</g, "&lt;").replace(/>/g, "&gt;"))).join("");
}

function decode(value) {
    return String(value)
        .replace(/&#x([0-9a-f]+);/gi, (_, hex) => String.fromCodePoint(Number.parseInt(hex, 16)))
        .replace(/&#(\d+);/g, (_, dec) => String.fromCodePoint(Number(dec)))
        .replace(/&lt;/g, "<")
        .replace(/&gt;/g, ">")
        .replace(/&quot;/g, '"')
        .replace(/&apos;/g, "'")
        .replace(/&amp;/g, "&");
}

function asText(value) {
    if (value == null) return "";
    if (Array.isArray(value)) return value.map(asText).filter(Boolean).join("\n\n");
    if (typeof value === "string") return value;
    return "";
}

function plain(value) {
    return renderComment(value, "api")
        .replace(/\[([^\]]+)\]\([^)]*\)/g, "$1")
        .replace(/<a\s+id="[^"]+"><\/a>/g, "")
        .replace(/[`*_]/g, "")
        .replace(/&lt;/g, "<")
        .replace(/&gt;/g, ">")
        .replace(/&amp;/g, "&")
        .replace(/\s+/g, " ")
        .trim();
}

function tableCell(value) {
    return value.replace(/\|/g, "\\|").replace(/\s+/g, " ").trim();
}

function truncate(value, max) {
    const text = value.replace(/\s+/g, " ").trim();
    if (text.length <= max) return text;
    return `${text.slice(0, max - 3).trimEnd()}...`;
}

function escapeLinkText(value) {
    return String(value).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\[/g, "\\[").replace(/\]/g, "\\]");
}

function fence(code, lang = "csharp") {
    return `\n\`\`\`${lang}\n${String(code).replace(/\n$/, "")}\n\`\`\`\n`;
}

function pushBlock(lines, block) {
    if (!block) return;
    lines.push(block, "");
}

function frontmatter(fields) {
    const lines = ["---"];
    for (const [key, value] of Object.entries(fields)) {
        if (value && typeof value === "object" && !Array.isArray(value)) {
            lines.push(`${key}:`);
            for (const [childKey, childValue] of Object.entries(value)) {
                lines.push(`  ${childKey}: ${JSON.stringify(childValue)}`);
            }
        } else {
            lines.push(`${key}: ${JSON.stringify(value)}`);
        }
    }
    lines.push("---", "");
    return lines.join("\n");
}

function docfxAnchor(uid) {
    return String(uid).replace(/[^A-Za-z0-9_]/g, "_");
}

function compareUid(a, b) {
    return a.localeCompare(b, "en");
}

function compareAssembly(a, b) {
    const ai = assemblyOrder.indexOf(a);
    const bi = assemblyOrder.indexOf(b);
    if (ai !== -1 || bi !== -1) return (ai === -1 ? 99 : ai) - (bi === -1 ? 99 : bi);
    return compareUid(a, b);
}

async function writeLegacyRedirect(uid, sitePath) {
    const fileName = `${String(uid).replace(/`/g, "-").replace(/[<>:"|?*\\]/g, "-")}.html`;
    if (fileName.toLowerCase() === "index.html") return;
    if (usedLegacy.has(fileName)) {
        throw new Error(`Legacy redirect ${fileName} is used by both ${usedLegacy.get(fileName)} and ${uid}.`);
    }
    usedLegacy.set(fileName, uid);
    await writeFile(path.join(redirectStaging, fileName), redirectHtml(sitePath));
}

function parseYaml(text, file) {
    let source = text.replace(/^\uFEFF/, "");
    if (source.startsWith("###")) {
        const newline = source.indexOf("\n");
        source = newline === -1 ? "" : source.slice(newline + 1);
    }
    const documents = parseAllDocuments(source);
    return documents.map((document) => {
        if (document.errors?.length) {
            throw new Error(`${file}: ${document.errors.map((error) => error.message).join("; ")}`);
        }
        return document.toJS();
    });
}

async function* walk(directory) {
    let entries;
    try {
        entries = await readdir(directory, { withFileTypes: true });
    } catch (error) {
        if (error.code === "ENOENT") throw new Error(`Missing ${directory}. Run npm run api without --skip-metadata.`);
        throw error;
    }
    for (const entry of entries) {
        const full = path.join(directory, entry.name);
        if (entry.isDirectory()) yield* walk(full);
        else yield full;
    }
}

async function renameDirectory(from, to) {
    const { rename } = await import("node:fs/promises");
    await rename(from, to);
}

function run(command, args) {
    return new Promise((resolve, reject) => {
        const child = spawn(command, args, {
            cwd: root,
            env: process.env,
            stdio: "inherit",
            detached: true,
        });
        const timer = setTimeout(() => {
            kill(child);
            reject(new Error(`${command} ${args.join(" ")} timed out after 25 minutes`));
        }, 25 * 60 * 1000);
        child.on("error", (error) => {
            clearTimeout(timer);
            reject(error);
        });
        child.on("exit", (code, signal) => {
            clearTimeout(timer);
            if (code === 0) resolve();
            else reject(new Error(`${command} ${args.join(" ")} failed (${signal || code})`));
        });
    });
}

function kill(child) {
    if (!child.pid) return;
    try {
        process.kill(-child.pid, "SIGTERM");
    } catch {
        child.kill("SIGTERM");
    }
}
