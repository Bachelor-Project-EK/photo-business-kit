import { LitElement, css, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbracoExtensionService } from "../api/index.js";

export default class CreateBookingElement extends LitElement {
  static properties = {
    photoPackages: { state: true },
    email: { state: true },

    photoPackageId: { state: true },
    startDate: { state: true },
    endDate: { state: true },
    comment: { state: true },

    error: { state: true },
    success: { state: true },
    loading: { state: true },
  };

  private photoPackages: any[] = [];

  private email = "";
  private photoPackageId = "";
  private startDate = "";
  private endDate = "";
  private comment = "";

  private error?: string;
  private success?: string;
  private loading = false;

  connectedCallback() {
    super.connectedCallback();
    this.loadPhotoPackages();
  }

  private async loadPhotoPackages() {
    const { data, error } = await UmbracoExtensionService.GetAllPhotoPackages();

    if (error) {
      this.error = "Could not load photo packages";
      return;
    }

    this.photoPackages = (data ?? []) as any[];
  }

  private async createBooking() {
    this.error = undefined;
    this.success = undefined;

    if (!this.email.trim()) {
      this.error = "Email is required";
      return;
    }

    if (!this.photoPackageId) {
      this.error = "Select a photo package";
      return;
    }

    if (!this.startDate) {
      this.error = "Start date is required";
      return;
    }

    if (!this.endDate) {
      this.error = "End date is required";
      return;
    }

    try {
      this.loading = true;

      const { error } = await UmbracoExtensionService.CreateBooking({
        query: {
          email: this.email,
        },
        body: {
          startDate: new Date(this.startDate).toISOString(),
          endDate: new Date(this.endDate).toISOString(),
          comment: this.comment,
          photoPackageId: this.photoPackageId,
        },
      });

      if (error) {
        this.error = "Could not create booking";
        return;
      }

      this.success = "Booking created successfully";

      this.email = "";
      this.photoPackageId = "";
      this.startDate = "";
      this.endDate = "";
      this.comment = "";

      this.dispatchEvent(
        new CustomEvent("booking-created", {
          bubbles: true,
          composed: true,
        }),
      );
    } catch {
      this.error = "Unexpected error while creating booking";
    } finally {
      this.loading = false;
    }
  }

  render() {
    return html`
      <uui-box headline="Create Booking">
        <div class="form-grid">
          <div class="field">
            <label>Email</label>

            <input
              type="email"
              placeholder="customer@example.com"
              .value=${this.email}
              @input=${(event: Event) =>
                (this.email = (event.target as HTMLInputElement).value)}
            />
          </div>

          <div class="field">
            <label>Photo package</label>

            <select
              .value=${this.photoPackageId}
              @change=${(event: Event) =>
                (this.photoPackageId = (
                  event.target as HTMLSelectElement
                ).value)}
            >
              <option value="">Select photo package</option>

              ${this.photoPackages.map(
                (photoPackage) => html`
                  <option value=${photoPackage.id}>
                    ${photoPackage.photoPackageName ?? photoPackage.name}
                  </option>
                `,
              )}
            </select>
          </div>

          <div class="field">
            <label>Start date</label>

            <input
              type="datetime-local"
              .value=${this.startDate}
              @input=${(event: Event) =>
                (this.startDate = (event.target as HTMLInputElement).value)}
            />
          </div>

          <div class="field">
            <label>End date</label>

            <input
              type="datetime-local"
              .value=${this.endDate}
              @input=${(event: Event) =>
                (this.endDate = (event.target as HTMLInputElement).value)}
            />
          </div>

          <div class="field full">
            <label>Comment</label>

            <textarea
              .value=${this.comment}
              @input=${(event: Event) =>
                (this.comment = (event.target as HTMLTextAreaElement).value)}
            ></textarea>
          </div>
        </div>

        ${this.error ? html`<p class="error">${this.error}</p>` : ""}
        ${this.success ? html`<p class="success">${this.success}</p>` : ""}

        <div class="actions">
          <uui-button
            look="primary"
            color="positive"
            label="Create booking"
            ?disabled=${this.loading}
            @click=${this.createBooking}
          >
            ${this.loading ? "Creating..." : "Create Booking"}
          </uui-button>
        </div>
      </uui-box>
    `;
  }

  static styles = css`
    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
    }

    .field {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .field.full {
      grid-column: 1 / -1;
    }

    label {
      font-weight: 600;
    }

    select,
    input,
    textarea {
      width: 100%;
      padding: 8px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      box-sizing: border-box;
      font: inherit;
    }

    textarea {
      min-height: 90px;
      resize: vertical;
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      margin-top: 16px;
    }

    .error {
      color: red;
      margin-top: 16px;
    }

    .success {
      color: green;
      margin-top: 16px;
    }
  `;
}

customElements.define("create-booking", CreateBookingElement);
