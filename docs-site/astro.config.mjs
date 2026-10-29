// @ts-check
import { defineConfig } from "astro/config";
import starlight from "@astrojs/starlight";
import { satteri } from "@astrojs/markdown-satteri";
import { baseLinksPlugin } from "./src/base-links.mjs";
import { siteBase, siteOrigin } from "./site.mjs";
import apiSidebar from "./src/generated/api-sidebar.json";
import guidesSidebar from "./src/generated/guides-sidebar.json";

// https://astro.build/config
export default defineConfig({
    site: siteOrigin,
    base: siteBase,
    trailingSlash: "always",
    markdown: {
        processor: satteri({
            mdastPlugins: [baseLinksPlugin()],
        }),
    },
    integrations: [
        starlight({
            title: "Puppeteer Sharp",
            description:
                "Puppeteer Sharp is a .NET port of the official Node.JS Puppeteer API. Control Chrome and Firefox with a high-level, easy-to-use API.",
            logo: {
                src: "./src/assets/logo.png",
                alt: "Hardkoded",
            },
            favicon: "/favicon.png",
            customCss: [
                "@fontsource/inter/400.css",
                "@fontsource/inter/500.css",
                "@fontsource/inter/600.css",
                "@fontsource/inter/700.css",
                "@fontsource/dm-mono/400.css",
                "./src/styles/custom.css",
            ],
            expressiveCode: {
                themes: ["github-light", "github-dark"],
                useStarlightDarkModeSwitch: true,
                useStarlightUiThemeColors: true,
            },
            social: [
                { icon: "github", label: "GitHub", href: "https://github.com/hardkoded/puppeteer-sharp" },
            ],
            editLink: {
                baseUrl: "https://github.com/hardkoded/puppeteer-sharp/edit/master/docs-site/",
            },
            credits: false,
            components: {
                Footer: "./src/components/Footer.astro",
            },
            sidebar: [
                ...guidesSidebar,
                {
                    label: "API",
                    collapsed: true,
                    items: apiSidebar,
                },
            ],
        }),
    ],
});
