import { t as e } from "./api-PQrrlcoC.js";
import { LitElement as t, css as n, html as r } from "@umbraco-cms/backoffice/external/lit";
//#region src/dashboards/create-booking.element.ts
var i = class extends t {
	constructor(...e) {
		super(...e), this.photoPackages = [], this.email = "", this.photoPackageId = "", this.startDate = "", this.endDate = "", this.comment = "", this.loading = !1;
	}
	static {
		this.properties = {
			photoPackages: { state: !0 },
			email: { state: !0 },
			photoPackageId: { state: !0 },
			startDate: { state: !0 },
			endDate: { state: !0 },
			comment: { state: !0 },
			error: { state: !0 },
			success: { state: !0 },
			loading: { state: !0 }
		};
	}
	connectedCallback() {
		super.connectedCallback(), this.loadPhotoPackages();
	}
	async loadPhotoPackages() {
		let { data: t, error: n } = await e.GetAllPhotoPackages();
		if (n) {
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
			let { error: t } = await e.CreateBooking({
				query: { email: this.email },
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
			this.success = "Booking created successfully", this.email = "", this.photoPackageId = "", this.startDate = "", this.endDate = "", this.comment = "", this.dispatchEvent(new CustomEvent("booking-created", {
				bubbles: !0,
				composed: !0
			}));
		} catch {
			this.error = "Unexpected error while creating booking";
		} finally {
			this.loading = !1;
		}
	}
	render() {
		return r`
      <uui-box headline="Create Booking">
        <div class="form-grid">
          <div class="field">
            <label>Email</label>

            <input
              type="email"
              placeholder="customer@example.com"
              .value=${this.email}
              @input=${(e) => this.email = e.target.value}
            />
          </div>

          <div class="field">
            <label>Photo package</label>

            <select
              .value=${this.photoPackageId}
              @change=${(e) => this.photoPackageId = e.target.value}
            >
              <option value="">Select photo package</option>

              ${this.photoPackages.map((e) => r`
                  <option value=${e.id}>
                    ${e.photoPackageName ?? e.name}
                  </option>
                `)}
            </select>
          </div>

          <div class="field">
            <label>Start date</label>

            <input
              type="datetime-local"
              .value=${this.startDate}
              @input=${(e) => this.startDate = e.target.value}
            />
          </div>

          <div class="field">
            <label>End date</label>

            <input
              type="datetime-local"
              .value=${this.endDate}
              @input=${(e) => this.endDate = e.target.value}
            />
          </div>

          <div class="field full">
            <label>Comment</label>

            <textarea
              .value=${this.comment}
              @input=${(e) => this.comment = e.target.value}
            ></textarea>
          </div>
        </div>

        ${this.error ? r`<p class="error">${this.error}</p>` : ""}
        ${this.success ? r`<p class="success">${this.success}</p>` : ""}

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
	static {
		this.styles = n`
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
};
customElements.define("create-booking", i);
//#endregion
//#region src/dashboards/booking.dashboard.ts
var a = class extends t {
	constructor(...e) {
		super(...e), this.bookings = [], this.loading = !0, this.selectedStatus = "propose", this.savingStatus = !1, this.statuses = [
			"approve",
			"propose",
			"reject"
		];
	}
	static {
		this.properties = {
			bookings: { state: !0 },
			loading: { state: !0 },
			error: { state: !0 },
			selectedBooking: { state: !0 },
			selectedStatus: { state: !0 },
			savingStatus: { state: !0 }
		};
	}
	connectedCallback() {
		super.connectedCallback(), this.loadBookings();
	}
	async loadBookings() {
		let { data: t, error: n } = await e.GetBookings();
		if (this.loading = !1, n) {
			this.error = "Could not load bookings";
			return;
		}
		this.bookings = t ?? [];
	}
	openStatusPopup(e) {
		this.selectedBooking = e, this.selectedStatus = e.status ?? "propose";
	}
	closeStatusPopup() {
		this.savingStatus || (this.selectedBooking = void 0, this.selectedStatus = "propose");
	}
	async saveStatus() {
		if (!this.selectedBooking) return;
		this.savingStatus = !0;
		let t = this.selectedBooking.id, n = this.selectedStatus, { error: r } = await e.ChangeBookingStatus({ path: {
			bookingId: t,
			action: n
		} });
		if (this.savingStatus = !1, r) {
			this.error = "Could not update booking status";
			return;
		}
		this.bookings = this.bookings.map((e) => e.id === t ? {
			...e,
			status: n
		} : e), this.closeStatusPopup();
	}
	render() {
		return this.loading ? r`
        <uui-box headline="Bookings">
          <uui-loader></uui-loader>
        </uui-box>
      ` : this.error ? r`
        <uui-box headline="Bookings">
          <p>${this.error}</p>
        </uui-box>
      ` : r`
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

          ${this.bookings.map((e) => r`
              <uui-table-row>
                <uui-table-cell>${e.status}</uui-table-cell>

                <uui-table-cell>
                  ${e.photoPackage?.name ?? "-"}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(e.startDate)}
                </uui-table-cell>

                <uui-table-cell>
                  ${this.formatDate(e.endDate)}
                </uui-table-cell>

                <uui-table-cell>${e.comment}</uui-table-cell>

                <uui-table-cell>
                  <uui-button
                    look="secondary"
                    label="Change status"
                    @click=${() => this.openStatusPopup(e)}
                  >
                    Change status
                  </uui-button>
                </uui-table-cell>
              </uui-table-row>
            `)}
        </uui-table>
      </uui-box>

      ${this.renderStatusPopup()}
    `;
	}
	renderStatusPopup() {
		return this.selectedBooking ? r`
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
              @change=${(e) => this.selectedStatus = e.target.value}
            >
              ${this.statuses.map((e) => r`
                  <option value=${e}>${e}</option>
                `)}
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
	formatDate(e) {
		return new Date(e).toLocaleString();
	}
	static {
		this.styles = n`
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
};
customElements.define("booking-dashboard", a);
//#endregion
export { a as default };

//# sourceMappingURL=booking.dashboard-BdgVduwN.js.map