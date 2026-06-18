import { LitElement, css, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbracoExtensionService } from "../api/index.js";

export default class CreateBookingElement extends LitElement {
  static properties = {
    email: { state: true },
    startDate: { state: true },
    endDate: { state: true },
    comment: { state: true },
    photoPackageId: { state: true },
    loading: { state: true },
    error: { state: true },
  };

  private email = "";
  private startDate = "";
  private endDate = "";
  private comment = "";
  private photoPackageId = "";
  private loading = false;
  private error?: string;

  private async createBooking() {
    this.error = undefined;

    if (!this.email || !this.startDate || !this.endDate) {
      this.error = "E-mail, start date and end date are required";
      return;
    }

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

    this.loading = false;

    if (error) {
      this.error = "Could not create booking";
      return;
    }

    this.email = "";
    this.startDate = "";
    this.endDate = "";
    this.comment = "";
    this.photoPackageId = "";

    this.dispatchEvent(
      new CustomEvent("booking-created", {
        bubbles: true,
        composed: true,
      }),
    );
  }
  render() {
    return html`
      <uui-box headline="Create booking">
        ${this.error ? html`<p class="error">${this.error}</p>` : null}

        <div class="form">
          <label>
            E-mail
            <input
              type="email"
              .value=${this.email}
              @input=${(event: Event) =>
                (this.email = (event.target as HTMLInputElement).value)}
            />
          </label>

          <label>
            Start date
            <input
              type="datetime-local"
              .value=${this.startDate}
              @input=${(event: Event) =>
                (this.startDate = (event.target as HTMLInputElement).value)}
            />
          </label>

          <label>
            End date
            <input
              type="datetime-local"
              .value=${this.endDate}
              @input=${(event: Event) =>
                (this.endDate = (event.target as HTMLInputElement).value)}
            />
          </label>

          <label>
            Comment
            <textarea
              .value=${this.comment}
              @input=${(event: Event) =>
                (this.comment = (event.target as HTMLTextAreaElement).value)}
            ></textarea>
          </label>
          <label>
            Photo package ID
            <input
              type="text"
              .value=${this.photoPackageId}
              @input=${(event: Event) =>
                (this.photoPackageId = (
                  event.target as HTMLInputElement
                ).value)}
            />
          </label>

          <uui-button
            look="primary"
            color="positive"
            label="Create booking"
            ?disabled=${this.loading}
            @click=${this.createBooking}
          >
            ${this.loading ? "Creating..." : "Create booking"}
          </uui-button>
        </div>
      </uui-box>
    `;
  }

  static styles = css`
    .form {
      display: grid;
      gap: 16px;
      max-width: 420px;
    }

    label {
      display: grid;
      gap: 6px;
      font-weight: 600;
    }

    input,
    textarea {
      padding: 8px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      font: inherit;
    }

    textarea {
      min-height: 80px;
    }

    .error {
      color: #d42054;
    }
  `;
}

customElements.define("create-booking", CreateBookingElement);
