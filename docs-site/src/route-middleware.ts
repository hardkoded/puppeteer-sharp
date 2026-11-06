import { defineRouteMiddleware } from "@astrojs/starlight/route-data";

/** Show only the sidebar of the section (Docs or API) the current page belongs to. */
export const onRequest = defineRouteMiddleware((context) => {
    const route = context.locals.starlightRoute;
    const inApi = route.id === "api" || route.id.startsWith("api/");
    const apiGroup = route.sidebar.find((e) => e.type === "group" && e.label === "API");
    if (inApi && apiGroup && apiGroup.type === "group") {
        route.sidebar = apiGroup.entries;
    } else {
        route.sidebar = route.sidebar.filter((e) => e !== apiGroup);
    }
});
