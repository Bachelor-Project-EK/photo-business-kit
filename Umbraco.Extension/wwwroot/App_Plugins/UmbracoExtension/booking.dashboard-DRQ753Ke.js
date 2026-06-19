import { LitElement as n, html as e, css as c } from "@umbraco-cms/backoffice/external/lit";
import { U as i } from "./sdk.gen-DJTPZEen.js";
const s = class s extends n {
  constructor() {
    super(...arguments), this.photoPackages = [], this.email = "", this.photoPackageId = "", this.startDate = "", this.endDate = "", this.comment = "", this.loading = !1;
  }
  connectedCallback() {
    super.connectedCallback(), this.loadPhotoPackages();
  }
  async loadPhotoPackages() {
    const { data: t, error: a } = await i.GetAllPhotoPackages();
    if (a) {
      this.error = "Could not load photo packages";
      return;
    }
    this.photoPackages = t ?? [];
  }
  async createBooking() {
    if (this.error = void 0, this.success = void 0, !this.email.trim()) {
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
      this.loading = !0;
      const { error: t } = await i.CreateBooking({
        query: {
          email: this.email
        },
        body: {
          startDate: new Date(this.startDate).toISOString(),
          endDate: new Date(this.endDate).toISOString(),
          comment: this.comment,
          photoPackageId: this.photoPackageId
        }
      });
      if (t) {
        this.error = "Could not create booking";
        return;
      }
      this.success = "Booking created successfully", this.email = "", this.photoPackageId = "", this.startDate = "", this.endDate = "", this.comment = "", this.dispatchEvent(
        new CustomEvent("booking-created", {
          bubbles: !0,
          composed: !0
        })
      );
    } catch {
      this.error = "Unexpected error while creating booking";
    } finally {
      this.loading = !1;
    }
  }
  render() {
    return e`
      <uui-box headline="Create Booking">
        <div class="form-grid">
          <div class="field">
            <label>Email</label>

            <input
              type="email"
              placeholder="customer@example.com"
              .value=${this.email}
              @input=${(t) => this.email = t.target.value}
            />
          </div>

          <div class="field">
            <label>Photo package</label>

            <select
              .value=${this.photoPackageId}
              @change=${(t) => this.photoPackageId = t.target.value}
            >
              <option value="">Select photo package</option>

              ${this.photoPackages.map(
      (t) => e`
                  <option value=${t.id}>
                    ${t.photoPackageName ?? t.name}
                  </option>
                `
    )}
            </select>
          </div>

          <div class="field">
            <label>Start date</label>

            <input
              type="datetime-local"
              .value=${this.startDate}
              @input=${(t) => this.startDate = t.target.value}
            />
          </div>

          <div class="field">
            <label>End date</label>

            <input
              type="datetime-local"
              .value=${this.endDate}
              @input=${(t) => this.endDate = t.target.value}
            />
          </div>

          <div class="field full">
            <label>Comment</label>

            <textarea
              .value=${this.comment}
              @input=${(t) => this.comment = t.target.value}
            ></textarea>
          </div>
        </div>

        ${this.error ? e`<p class="error">${this.error}</p>` : ""}
        ${this.success ? e`<p class="success">${this.success}</p>` : ""}

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
};
s.properties = {
  photoPackages: { state: !0 },
  email: { state: !0 },
  photoPackageId: { state: !0 },
  startDate: { state: !0 },
  endDate: { state: !0 },
  comment: { state: !0 },
  error: { state: !0 },
  success: { state: !0 },
  loading: { state: !0 }
}, s.styles = c`
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
let r = s;
customElements.define("create-booking", r);
const o = class o extends n {
  constructor() {
    super(...arguments), this.bookings = [], this.loading = !0, this.selectedStatus = "propose", this.savingStatus = !1, this.statuses = ["approve", "propose", "reject"];
  }
  connectedCallback() {
    super.connectedCallback(), this.loadBookings();
  }
  async loadBookings() {
    const { data: t, error: a } = await i.GetBookings();
    if (this.loading = !1, a) {
      this.error = "Could not load bookings";
      return;
    }
    this.bookings = t ?? [];
  }
  openStatusPopup(t) {
    this.selectedBooking = t, this.selectedStatus = t.status ?? "propose";
  }
  closeStatusPopup() {
    this.savingStatus || (this.selectedBooking = void 0, this.selectedStatus = "propose");
  }
  async saveStatus() {
    if (!this.selectedBooking) return;
    this.savingStatus = !0;
    const t = this.selectedBooking.id, a = this.selectedStatus, { error: d } = await i.ChangeBookingStatus({
      path: {
        bookingId: t,
        action: a
      }
    });
    if (this.savingStatus = !1, d) {
      this.error = "Could not update booking status";
      return;
    }
    this.bookings = this.bookings.map(
      (l) => l.id === t ? {
        ...l,
        status: a
      } : l
    ), this.closeStatusPopup();
  }
  render() {
    return this.loading ? e`
        <uui-box headline="Bookings">
          <uui-loader></uui-loader>
        </uui-box>
      ` : this.error ? e`
        <uui-box headline="Bookings">
          <p>${this.error}</p>
        </uui-box>
      ` : e`
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
      (t) => e`
              <uui-table-row>
                <uui-table-cell>${t.status}</uui-table-cell>

                <uui-table-cell>
                  ${t.photoPackage?.name ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(t.startDate)}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(t.endDate)}
                </uui-table-cell>

                <uui-table-cell>${t.comment}</uui-table-cell>

                <uui-table-cell>
                  <uui-button
                    look="secondary"
                    label="Change status"
                    @click=${() => this.openStatusPopup(t)}
                  >
                    Change status
                  </uui-button>
                </uui-table-cell>
              </uui-table-row>
            `
    )}
        </uui-table>
      </uui-box>

      ${this.renderStatusPopup()}
    `;
  }
  renderStatusPopup() {
    return this.selectedBooking ? e`
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
              @change=${(t) => this.selectedStatus = t.target.value}
            >
              ${this.statuses.map(
      (t) => e`
                  <option value=${t}>${t}</option>
                `
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
    ` : null;
  }
  formatDate(t) {
    return new Date(t).toLocaleString();
  }
};
o.properties = {
  bookings: { state: !0 },
  loading: { state: !0 },
  error: { state: !0 },
  selectedBooking: { state: !0 },
  selectedStatus: { state: !0 },
  savingStatus: { state: !0 }
}, o.styles = c`
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
let u = o;
customElements.define("booking-dashboard", u);
export {
  u as default
};
//# sourceMappingURL=booking.dashboard-DRQ753Ke.js.map
