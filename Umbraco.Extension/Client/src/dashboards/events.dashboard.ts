import type { UmbDashboardElement } from "@umbraco-cms/backoffice/dashboard";
import { LitElement, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbracoExtensionService } from "../api/index.js";

export default class EventsDashboardElement
    extends LitElement
    implements UmbDashboardElement {
    static properties = {
        eventTypeName: { state: true },
        eventTypes: { state: true },
        photoPackages: { state: true },
        selectedEventTypeId: { state: true },

        photoPackageName: { state: true },
        photoCount: { state: true },
        photoPrice: { state: true },
        hourlyPrice: { state: true },

        error: { state: true },
        success: { state: true },
        loading: { state: true },
    };

    private eventTypeName = "";

    private eventTypes: any[] = [];
    private photoPackages: any[] = [];
    private selectedEventTypeId?: string;

    private photoPackageName = "";
    private photoCount = 0;
    private photoPrice = 0;
    private hourlyPrice = 0;

    private error?: string;
    private success?: string;
    private loading = false;

    connectedCallback() {
        super.connectedCallback();
        this.loadEventTypes();
        this.loadPhotoPackages();
    }

    private async loadEventTypes() {
        const { data, error } = await UmbracoExtensionService.GetEventTypes();

        if (error) {
            this.error = "Could not load event types";
            return;
        }

        this.eventTypes = (data ?? []) as any[];
    }

    private async loadPhotoPackages() {
        const { data, error } =
            await UmbracoExtensionService.GetAllPhotoPackages();

        if (error) {
            this.error = "Could not load photo packages";
            return;
        }

        this.photoPackages = (data ?? []) as any[];
    }

    private async createEventType() {
        this.error = undefined;
        this.success = undefined;

        const name = this.eventTypeName.trim();

        if (!name) {
            this.error = "Event type name is required";
            return;
        }

        const alreadyExists = this.eventTypes.some(
            (eventType) =>
                eventType.eventTypeName?.toLowerCase() === name.toLowerCase(),
        );

        if (alreadyExists) {
            this.error = "An event type with this name already exists";
            return;
        }

        try {
            this.loading = true;

            const { error } = await UmbracoExtensionService.CreateEventType({
                body: {
                    eventTypeName: name,
                },
            });

            if (error) {
                this.error = "Could not create event type";
                return;
            }

            this.success = "Event type created successfully";
            this.eventTypeName = "";

            await this.loadEventTypes();
        } catch {
            this.error = "Unexpected error while creating event type";
        } finally {
            this.loading = false;
        }
    }

    private selectEventType(eventTypeId: string) {
        this.selectedEventTypeId = eventTypeId;

        this.photoPackageName = "";
        this.photoCount = 0;
        this.photoPrice = 0;
        this.hourlyPrice = 0;

        this.error = undefined;
        this.success = undefined;
    }

    private async createPhotoPackage() {
        this.error = undefined;
        this.success = undefined;

        if (!this.selectedEventTypeId) {
            this.error = "Select an event type first";
            return;
        }

        if (!this.photoPackageName.trim()) {
            this.error = "Photo package name is required";
            return;
        }

        const alreadyExists = this.photoPackages.some(
            (photoPackage) =>
                photoPackage.eventTypeId === this.selectedEventTypeId &&
                photoPackage.photoPackageName?.toLowerCase() ===
                this.photoPackageName.trim().toLowerCase(),
        );

        if (alreadyExists) {
            this.error = "This photo package already exists for this event type";
            return;
        }

        try {
            this.loading = true;

            const { error } = await UmbracoExtensionService.CreatePhotoPackage({
                body: {
                    eventTypeId: this.selectedEventTypeId,
                    photoPackageName: this.photoPackageName.trim(),
                    photoCount: this.photoCount,
                    photoPrice: this.photoPrice,
                    hourlyPrice: this.hourlyPrice,
                },
            });

            if (error) {
                this.error = "Could not create photo package";
                return;
            }

            this.success = "Photo package created successfully";

            this.photoPackageName = "";
            this.photoCount = 0;
            this.photoPrice = 0;
            this.hourlyPrice = 0;

            await this.loadPhotoPackages();
        } catch {
            this.error = "Unexpected error while creating photo package";
        } finally {
            this.loading = false;
        }
    }

    render() {
        const selectedEventType = this.eventTypes.find(
            (eventType) => eventType.id === this.selectedEventTypeId,
        );

        return html`
      <uui-box headline="Create Event Type">
        <uui-input
          label="Event type name"
          placeholder="Event type name"
          .value=${this.eventTypeName}
          @input=${(e: Event) =>
                (this.eventTypeName = (e.target as HTMLInputElement).value)}
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
                    (eventType) => html`
              <uui-table-row>
                <uui-table-cell>${eventType.eventTypeName}</uui-table-cell>

                <uui-table-cell>
                  <uui-button
                    look="primary"
                    color="default"
                    @click=${() => this.selectEventType(eventType.id)}
                  >
                    Add package
                  </uui-button>
                </uui-table-cell>
              </uui-table-row>
            `,
                )}
        </uui-table>
      </uui-box>

      ${this.selectedEventTypeId
                ? html`
            <uui-box headline="Create Photo Package" style="margin-top: 24px;">
              <p>
                Creating package for:
                <strong>${selectedEventType?.eventTypeName}</strong>
              </p>

              <uui-input
                label="Package name"
                placeholder="Package name"
                .value=${this.photoPackageName}
                @input=${(e: Event) =>
                    (this.photoPackageName = (
                        e.target as HTMLInputElement
                    ).value)}
              ></uui-input>

              <uui-input
                type="number"
                label="Photo count"
                placeholder="Photo count"
                .value=${this.photoCount.toString()}
                @input=${(e: Event) =>
                    (this.photoCount = Number(
                        (e.target as HTMLInputElement).value,
                    ))}
              ></uui-input>

              <uui-input
                type="number"
                label="Photo price"
                placeholder="Photo price"
                .value=${this.photoPrice.toString()}
                @input=${(e: Event) =>
                    (this.photoPrice = Number(
                        (e.target as HTMLInputElement).value,
                    ))}
              ></uui-input>

              <uui-input
                type="number"
                label="Hourly price"
                placeholder="Hourly price"
                .value=${this.hourlyPrice.toString()}
                @input=${(e: Event) =>
                    (this.hourlyPrice = Number(
                        (e.target as HTMLInputElement).value,
                    ))}
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
          `
                : ""}

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

          ${this.photoPackages.map((photoPackage) => {
                    const eventType = this.eventTypes.find(
                        (eventType) => eventType.id === photoPackage.eventTypeId,
                    );

                    return html`
              <uui-table-row>
                <uui-table-cell>
                  ${photoPackage.photoPackageName}
                </uui-table-cell>

                <uui-table-cell>
                  ${eventType?.eventTypeName ?? photoPackage.eventTypeId}
                </uui-table-cell>

                <uui-table-cell>
                  ${photoPackage.photoCount ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${photoPackage.photoPrice ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${photoPackage.hourlyPrice ?? "-"}
                </uui-table-cell>
              </uui-table-row>
            `;
                })}
        </uui-table>
      </uui-box>

      ${this.error ? html`<p style="color: red">${this.error}</p>` : ""}
      ${this.success ? html`<p style="color: green">${this.success}</p>` : ""}
    `;
    }
}

customElements.define("events-dashboard", EventsDashboardElement);