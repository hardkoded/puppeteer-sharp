import { siteBase } from "../site.mjs";

/**
 * Prefix root-relative Markdown links and images with the Pages base path.
 * Starlight already prefixes its own sidebar and pager links.
 */
export function baseLinksPlugin() {
    const base = siteBase.endsWith("/") ? siteBase.slice(0, -1) : siteBase;
    const rewrite = (node, context) => {
        const url = prefixBase(node.url, base);
        if (url !== node.url) context.setProperty(node, "url", url);
    };
    return {
        name: "base-links",
        link: rewrite,
        definition: rewrite,
        image: rewrite,
    };
}

function prefixBase(url, base) {
    if (typeof url !== "string" || !url.startsWith("/") || url.startsWith("//")) return url;
    if (url === base || url.startsWith(`${base}/`)) return url;
    return base + url;
}
