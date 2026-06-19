import { LitElement as l, html as a } from "@umbraco-cms/backoffice/external/lit";
import { U as o } from "./sdk.gen-DJTPZEen.js";
const u = class u extends l {
  constructor() {
    super(...arguments), this.eventTypeName = "", this.eventTypes = [], this.photoPackages = [], this.photoPackageName = "", this.photoCount = 0, this.photoPrice = 0, this.hourlyPrice = 0, this.loading = !1;
  }
  connectedCallback() {
    super.connectedCallback(), this.loadEventTypes(), this.loadPhotoPackages();
  }
  async loadEventTypes() {
    const { data: t, error: e } = await o.GetEventTypes();
    if (e) {
      this.error = "Could not load event types";
      return;
    }
    this.eventTypes = t ?? [];
  }
  async loadPhotoPackages() {
    const { data: t, error: e } = await o.GetAllPhotoPackages();
    if (e) {
      this.error = "Could not load photo packages";
      return;
    }
    this.photoPackages = t ?? [];
  }
  async createEventType() {
    this.error = void 0, this.success = void 0;
    const t = this.eventTypeName.trim();
    if (!t) {
      this.error = "Event type name is required";
      return;
    }
    if (this.eventTypes.some(
      (i) => i.eventTypeName?.toLowerCase() === t.toLowerCase()
    )) {
      this.error = "An event type with this name already exists";
      return;
    }
    try {
      this.loading = !0;
      const { error: i } = await o.CreateEventType({
        body: {
          eventTypeName: t
        }
      });
      if (i) {
        this.error = "Could not create event type";
        return;
      }
      this.success = "Event type created successfully", this.eventTypeName = "", await this.loadEventTypes();
    } catch {
      this.error = "Unexpected error while creating event type";
    } finally {
      this.loading = !1;
    }
  }
  selectEventType(t) {
    this.selectedEventTypeId = t, this.photoPackageName = "", this.photoCount = 0, this.photoPrice = 0, this.hourlyPrice = 0, this.error = void 0, this.success = void 0;
  }
  async createPhotoPackage() {
    if (this.error = void 0, this.success = void 0, !this.selectedEventTypeId) {
      this.error = "Select an event type first";
      return;
    }
    if (!this.photoPackageName.trim()) {
      this.error = "Photo package name is required";
      return;
    }
    if (this.photoPackages.some(
      (e) => e.eventTypeId === this.selectedEventTypeId && e.photoPackageName?.toLowerCase() === this.photoPackageName.trim().toLowerCase()
    )) {
      this.error = "This photo package already exists for this event type";
      return;
    }
    try {
      this.loading = !0;
      const { error: e } = await o.CreatePhotoPackage({
        body: {
          eventTypeId: this.selectedEventTypeId,
          photoPackageName: this.photoPackageName.trim(),
          photoCount: this.photoCount,
          photoPrice: this.photoPrice,
          hourlyPrice: this.hourlyPrice
        }
      });
      if (e) {
        this.error = "Could not create photo package";
        return;
      }
      this.success = "Photo package created successfully", this.photoPackageName = "", this.photoCount = 0, this.photoPrice = 0, this.hourlyPrice = 0, await this.loadPhotoPackages();
    } catch {
      this.error = "Unexpected error while creating photo package";
    } finally {
      this.loading = !1;
    }
  }
  render() {
    const t = this.eventTypes.find(
      (e) => e.id === this.selectedEventTypeId
    );
    return a`
      <uui-box headline="Create Event Type">
        <uui-input
          label="Event type name"
          placeholder="Event type name"
          .value=${this.eventTypeName}
          @input=${(e) => this.eventTypeName = e.target.value}
        ></uui-input>

        <uui-button
          look="primary"
          color="positive"
          ?disabled=${this.loading}
          @click=${this.createEventType}
        >
          ${this.loading ? "Creating..." : "Create Event Type"}
        </uui-button>
      </uui-box>

      <uui-box headline="Event Types" style="margin-top: 24px;">
        <p>Total event types: ${this.eventTypes.length}</p>

        <uui-table>
          <uui-table-head>
            <uui-table-head-cell>Name</uui-table-head-cell>
            <uui-table-head-cell>Action</uui-table-head-cell>
          </uui-table-head>

          ${this.eventTypes.map(
      (e) => a`
              <uui-table-row>
                <uui-table-cell>${e.eventTypeName}</uui-table-cell>

                <uui-table-cell>
                  <uui-button
                    look="primary"
                    color="default"
                    @click=${() => this.selectEventType(e.id)}
                  >
                    Add package
                  </uui-button>
                </uui-table-cell>
              </uui-table-row>
            `
    )}
        </uui-table>
      </uui-box>

      ${this.selectedEventTypeId ? a`
            <uui-box headline="Create Photo Package" style="margin-top: 24px;">
              <p>
                Creating package for:
                <strong>${t?.eventTypeName}</strong>
              </p>

              <uui-input
                label="Package name"
                placeholder="Package name"
                .value=${this.photoPackageName}
                @input=${(e) => this.photoPackageName = e.target.value}
              ></uui-input>

              <uui-input
                type="number"
                label="Photo count"
                placeholder="Photo count"
                .value=${this.photoCount.toString()}
                @input=${(e) => this.photoCount = Number(
      e.target.value
    )}
              ></uui-input>

              <uui-input
                type="number"
                label="Photo price"
                placeholder="Photo price"
                .value=${this.photoPrice.toString()}
                @input=${(e) => this.photoPrice = Number(
      e.target.value
    )}
              ></uui-input>

              <uui-input
                type="number"
                label="Hourly price"
                placeholder="Hourly price"
                .value=${this.hourlyPrice.toString()}
                @input=${(e) => this.hourlyPrice = Number(
      e.target.value
    )}
              ></uui-input>

              <uui-button
                look="primary"
                color="positive"
                ?disabled=${this.loading}
                @click=${this.createPhotoPackage}
              >
                ${this.loading ? "Creating..." : "Create Photo Package"}
              </uui-button>
            </uui-box>
          ` : ""}

      <uui-box headline="Photo Packages" style="margin-top: 24px;">
        <p>Total photo packages: ${this.photoPackages.length}</p>

        <uui-table>
          <uui-table-head>
            <uui-table-head-cell>Package</uui-table-head-cell>
            <uui-table-head-cell>Event type</uui-table-head-cell>
            <uui-table-head-cell>Photo count</uui-table-head-cell>
            <uui-table-head-cell>Photo price</uui-table-head-cell>
            <uui-table-head-cell>Hourly price</uui-table-head-cell>
          </uui-table-head>

          ${this.photoPackages.map((e) => {
      const i = this.eventTypes.find(
        (s) => s.id === e.eventTypeId
      );
      return a`
              <uui-table-row>
                <uui-table-cell>
                  ${e.photoPackageName}
                </uui-table-cell>

                <uui-table-cell>
                  ${i?.eventTypeName ?? e.eventTypeId}
                </uui-table-cell>

                <uui-table-cell>
                  ${e.photoCount ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${e.photoPrice ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${e.hourlyPrice ?? "-"}
                </uui-table-cell>
              </uui-table-row>
            `;
    })}
        </uui-table>
      </uui-box>

      ${this.error ? a`<p style="color: red">${this.error}</p>` : ""}
      ${this.success ? a`<p style="color: green">${this.success}</p>` : ""}
    `;
  }
};
u.properties = {
  eventTypeName: { state: !0 },
  eventTypes: { state: !0 },
  photoPackages: { state: !0 },
  selectedEventTypeId: { state: !0 },
  photoPackageName: { state: !0 },
  photoCount: { state: !0 },
  photoPrice: { state: !0 },
  hourlyPrice: { state: !0 },
  error: { state: !0 },
  success: { state: !0 },
  loading: { state: !0 }
};
let r = u;
customElements.define("events-dashboard", r);
export {
  r as default
};
//# sourceMappingURL=events.dashboard-Dh0TLhhj.js.map
