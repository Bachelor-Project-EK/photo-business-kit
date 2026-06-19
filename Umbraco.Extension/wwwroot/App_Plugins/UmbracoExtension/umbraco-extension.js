//#region src/entrypoints/manifest.ts
var e = [{
	name: "Umbraco Extension Entrypoint",
	alias: "Umbraco.Extension.Entrypoint",
	type: "backofficeEntryPoint",
	js: () => import("./entrypoint-D0rGmMF2.js")
}], t = [{
	name: "Umbraco Extension Dashboard",
	alias: "Umbraco.Extension.Dashboard",
	type: "dashboard",
	js: () => import("./booking.dashboard-BdgVduwN.js"),
	meta: {
		label: "Bookings",
		pathname: "umbraco-extension-dashboard"
	},
	conditions: [{
		alias: "Umb.Condition.SectionAlias",
		match: "Umb.Section.Content"
	}]
}, {
	name: "Umbraco Extension Events Dashboard",
	alias: "Umbraco.Extension.EventsDashboard",
	type: "dashboard",
	js: () => import("./events.dashboard-DE2L3odO.js"),
	meta: {
		label: "Events",
		pathname: "umbraco-extension-events-dashboard"
	},
	conditions: [{
		alias: "Umb.Condition.SectionAlias",
		match: "Umb.Section.Content"
	}]
}], n = [...e, ...t];
//#endregion
export { n as manifests };

//# sourceMappingURL=umbraco-extension.js.map