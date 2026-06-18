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
	js: () => import("./booking.dashboard-xbCd0DWu.js"),
	meta: {
		label: "Bookings",
		pathname: "umbraco-extension-dashboard"
	},
	conditions: [{
		alias: "Umb.Condition.SectionAlias",
		match: "Umb.Section.Content"
	}]
}], n = [...e, ...t];
//#endregion
export { n as manifests };

//# sourceMappingURL=umbraco-extension.js.map