import type { UmbDashboardElement } from "@umbraco-cms/backoffice/dashboard";
import { LitElement, css, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbracoExtensionService } from "../api/index.js";
import type { Booking, BookingStatus } from "../api/types.gen";
import "./create-booking.element.js";

export default class BookingDashboardElement
  extends LitElement
  implements UmbDashboardElement
{
  static properties = {
    bookings: { state: true },
    loading: { state: true },
    error: { state: true },
    selectedBooking: { state: true },
    selectedStatus: { state: true },
    savingStatus: { state: true },
  };

  private bookings: Booking[] = [];
  private loading = true;
  private error?: string;

  private selectedBooking?: Booking;
  private selectedStatus: BookingStatus = "propose";
  private savingStatus = false;

  private statuses: BookingStatus[] = ["approve", "propose", "reject"];

  connectedCallback() {
    super.connectedCallback();
    this.loadBookings();
  }

  private async loadBookings() {
    const { data, error } = await UmbracoExtensionService.GetBookings();

    this.loading = false;

    if (error) {
      this.error = "Could not load bookings";
      return;
    }

    this.bookings = (data ?? []) as Booking[];
  }

  private openStatusPopup(booking: Booking) {
    this.selectedBooking = booking;
    this.selectedStatus = booking.status ?? "propose";
  }
  private closeStatusPopup() {
    if (this.savingStatus) return;

    this.selectedBooking = undefined;
    this.selectedStatus = "propose";
  }
  private async saveStatus() {
    if (!this.selectedBooking) return;

    this.savingStatus = true;

    const bookingId = this.selectedBooking.id;
    const newStatus = this.selectedStatus;

    const { error } = await UmbracoExtensionService.ChangeBookingStatus({
      path: {
        bookingId: bookingId,
        action: newStatus,
      },
    });

    this.savingStatus = false;

    if (error) {
      this.error = "Could not update booking status";
      return;
    }

    this.bookings = this.bookings.map((booking) =>
      booking.id === bookingId
        ? {
            ...booking,
            status: newStatus,
          }
        : booking,
    );

    this.closeStatusPopup();
  }

  render() {
    if (this.loading) {
      return html`
        <uui-box headline="Bookings">
          <uui-loader></uui-loader>
        </uui-box>
      `;
    }

    if (this.error) {
      return html`
        <uui-box headline="Bookings">
          <p>${this.error}</p>
        </uui-box>
      `;
    }

    return html`
      <create-booking @booking-created=${this.loadBookings}></create-booking>

      <uui-box headline="Bookings">
        <uui-table>
          <uui-table-head>
            <uui-table-head-cell>Status</uui-table-head-cell>
            <uui-table-head-cell>Package</uui-table-head-cell>
            <uui-table-head-cell>Start</uui-table-head-cell>
            <uui-table-head-cell>End</uui-table-head-cell>
            <uui-table-head-cell>Comment</uui-table-head-cell>
            <uui-table-head-cell>Action</uui-table-head-cell>
          </uui-table-head>

          ${this.bookings.map(
            (booking) => html`
              <uui-table-row>
                <uui-table-cell>${booking.status}</uui-table-cell>

                <uui-table-cell>
                  ${booking.photoPackage?.name ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(booking.startDate)}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(booking.endDate)}
                </uui-table-cell>

                <uui-table-cell>${booking.comment}</uui-table-cell>

                <uui-table-cell>
                  <uui-button
                    look="secondary"
                    label="Change status"
                    @click=${() => this.openStatusPopup(booking)}
                  >
                    Change status
                  </uui-button>
                </uui-table-cell>
              </uui-table-row>
            `,
          )}
        </uui-table>
      </uui-box>

      ${this.renderStatusPopup()}
    `;
  }

  private renderStatusPopup() {
    if (!this.selectedBooking) return null;

    return html`
      <div class="modal-backdrop">
        <div class="modal">
          <uui-box headline="Change booking status">
            <p>
              Current status:
              <strong>${this.selectedBooking.status}</strong>
            </p>

            <label>Status</label>

            <select
              .value=${this.selectedStatus}
              @change=${(event: Event) =>
                (this.selectedStatus = (event.target as HTMLSelectElement)
                  .value as BookingStatus)}
            >
              ${this.statuses.map(
                (status: BookingStatus) => html`
                  <option value=${status}>${status}</option>
                `,
              )}
            </select>

            <div class="actions">
              <uui-button
                look="secondary"
                label="Cancel"
                ?disabled=${this.savingStatus}
                @click=${this.closeStatusPopup}
              >
                Cancel
              </uui-button>

              <uui-button
                look="primary"
                color="positive"
                label="Save"
                ?disabled=${this.savingStatus}
                @click=${this.saveStatus}
              >
                ${this.savingStatus ? "Saving..." : "Save"}
              </uui-button>
            </div>
          </uui-box>
        </div>
      </div>
    `;
  }

  private formatDate(value: string) {
    return new Date(value).toLocaleString();
  }

  static styles = css`
    uui-table {
      width: 100%;
    }

    .modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.35);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 9999;
    }

    .modal {
      width: 420px;
      max-width: calc(100vw - 32px);
    }

    label {
      display: block;
      margin-bottom: 6px;
      font-weight: 600;
    }

    select {
      width: 100%;
      padding: 8px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      margin-bottom: 16px;
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
      margin-top: 16px;
    }
  `;
}

customElements.define("booking-dashboard", BookingDashboardElement);
