import type { UmbDashboardElement } from "@umbraco-cms/backoffice/dashboard";
import { LitElement, css, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbracoExtensionService } from "../api/index.js";
import type { Booking } from "../api/types.gen";

export default class BookingDashboardElement
  extends LitElement
  implements UmbDashboardElement
{
  static properties = {
    bookings: { state: true },
    loading: { state: true },
    error: { state: true },
  };

  private bookings: Booking[] = [];
  private loading = true;
  private error?: string;

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

  render() {
    if (this.loading) {
      return html`<uui-box headline="Bookings"
        ><uui-loader></uui-loader
      ></uui-box>`;
    }

    if (this.error) {
      return html`<uui-box headline="Bookings"><p>${this.error}</p></uui-box>`;
    }

    return html`
      <uui-box headline="Bookings">
        <uui-table>
          <uui-table-head>
            <uui-table-head-cell>Status</uui-table-head-cell>
            <uui-table-head-cell>Package</uui-table-head-cell>
            <uui-table-head-cell>Start</uui-table-head-cell>
            <uui-table-head-cell>End</uui-table-head-cell>
            <uui-table-head-cell>Comment</uui-table-head-cell>
          </uui-table-head>

          ${this.bookings.map(
            (booking) => html`
              <uui-table-row>
                <uui-table-cell>${booking.status}</uui-table-cell>
                <uui-table-cell
                  >${booking.photoPackage?.name ?? "-"}</uui-table-cell
                >
                <uui-table-cell
                  >${this.formatDate(booking.startDate)}</uui-table-cell
                >
                <uui-table-cell
                  >${this.formatDate(booking.endDate)}</uui-table-cell
                >
                <uui-table-cell>${booking.comment}</uui-table-cell>
              </uui-table-row>
            `,
          )}
        </uui-table>
      </uui-box>
    `;
  }

  private formatDate(value: string) {
    return new Date(value).toLocaleString();
  }

  static styles = css`
    uui-table {
      width: 100%;
    }
  `;
}

customElements.define("booking-dashboard", BookingDashboardElement);
