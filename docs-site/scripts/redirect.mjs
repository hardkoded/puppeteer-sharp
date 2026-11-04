import { siteBase, siteOrigin } from "../site.mjs";

/** HTML page that sends an old DocFX URL to a Starlight path. `sitePath` is root-relative, such as `/guide/`. */
export function redirectHtml(sitePath) {
    if (!sitePath.startsWith("/")) {
        throw new Error(`Redirect path must start with /: ${sitePath}`);
    }

    const base = siteBase.endsWith("/") ? siteBase.slice(0, -1) : siteBase;
    const urlPath = `${base}${sitePath}`;
    const canonical = `${siteOrigin}${urlPath}`;
    return `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<link rel="canonical" href="${escapeAttr(canonical)}">
<meta http-equiv="refresh" content="0; url=${escapeAttr(urlPath)}">
<title>Redirecting…</title>
<script>location.replace(${JSON.stringify(urlPath)} + (location.hash || ""));</script>
</head>
<body>
<p><a href="${escapeAttr(urlPath)}">Continue</a></p>
</body>
</html>
`;
}

function escapeAttr(value) {
    return value.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;");
}
