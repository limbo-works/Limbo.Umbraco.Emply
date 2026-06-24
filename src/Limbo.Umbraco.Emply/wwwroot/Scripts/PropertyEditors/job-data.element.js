const template = document.createElement("template");
template.innerHTML = `
  <style>
    :host { display: block; font-family: inherit; }
    .empty, .error { padding: 10px 12px; border: 1px solid #d8d7d9; border-radius: 3px; background: #f9f7f7; }
    .error { border-color: #c63632; }
    table { width: 100%; border-collapse: collapse; margin-bottom: 12px; }
    th, td { padding: 8px 10px; border-bottom: 1px solid #e9e7e7; text-align: left; vertical-align: top; }
    th { width: 160px; color: #413659; font-weight: 600; }
    small, .muted { color: #817f85; }
    button { min-height: 32px; padding: 0 12px; border: 1px solid #817f85; border-radius: 3px; background: #fff; cursor: pointer; }
    pre { display: none; max-height: 520px; overflow: auto; margin-top: 12px; padding: 12px; border: 1px solid #d8d7d9; border-radius: 3px; background: #f9f7f7; white-space: pre-wrap; word-break: break-word; }
    :host([show-json]) pre { display: block; }
  </style>
  <div id="content"></div>
`;

function parseStoredJson(value) {
  if (!value) return { data: null, raw: "" };
  if (typeof value !== "string") return { data: value, raw: JSON.stringify(value, null, 2) };

  const raw = value.startsWith("_") ? value.substring(1) : value;
  if (!raw.trim()) return { data: null, raw: "" };

  return { data: JSON.parse(raw), raw };
}

function localize(value, preferredLocale = "da-DK") {
  if (!value) return "";

  const localizations = Array.isArray(value.localization) ? value.localization : [];
  const match = localizations.find(x => x.locale === preferredLocale) ?? localizations[0];

  return match?.value ?? value.value ?? value.title ?? String(value ?? "");
}

function formatDate(value) {
  if (!value) return "N/A";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return String(value);

  const label = new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(date);
  const diffSeconds = Math.round((date.getTime() - Date.now()) / 1000);
  const units = [
    ["year", 31536000],
    ["month", 2592000],
    ["week", 604800],
    ["day", 86400],
    ["hour", 3600],
    ["minute", 60]
  ];
  const formatter = new Intl.RelativeTimeFormat(undefined, { numeric: "auto" });

  for (const [unit, seconds] of units) {
    if (Math.abs(diffSeconds) >= seconds) {
      return `${label} <small>(${formatter.format(Math.round(diffSeconds / seconds), unit)})</small>`;
    }
  }

  return `${label} <small>(${formatter.format(diffSeconds, "second")})</small>`;
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function getCategories(data) {
  if (!Array.isArray(data?.data)) return [];

  const categories = [];
  for (const item of data.data) {
    const title = localize(item?.title);
    if (title !== "Stillingskategori") continue;
    if (!Array.isArray(item.value)) continue;

    for (const value of item.value) {
      const label = localize(value?.title);
      if (label) categories.push(label);
    }
  }

  return categories;
}

function makeRows(data) {
  const categories = getCategories(data);
  const edited = data?.edited ?? data?.updated ?? data?.created;

  return [
    ["Job ID", data?.jobId == null ? "N/A" : escapeHtml(data.jobId)],
    ["Titel", localize(data?.title) ? escapeHtml(localize(data.title)) : "N/A"],
    ["Oprettet", formatDate(data?.created)],
    ["Opdateret", formatDate(edited)],
    ["Deadline", formatDate(data?.deadlineUTC ?? data?.deadlineUtc ?? data?.deadline)],
    ["Location", data?.location?.address ? escapeHtml(data.location.address) : "N/A"],
    [categories.length === 1 ? "Kategori" : "Kategorier", categories.length ? escapeHtml(categories.join(", ")) : "N/A"]
  ];
}

class LimboUmbracoEmplyJobDataPropertyEditor extends HTMLElement {

  #value;
  #root;

  set value(value) {
    this.#value = value;
    this.#render();
  }

  get value() {
    return this.#value;
  }

  connectedCallback() {
    if (!this.#root) {
      this.#root = this.attachShadow({ mode: "open" });
      this.#root.appendChild(template.content.cloneNode(true));
    }
    this.#render();
  }

  #render() {
    if (!this.#root) return;

    const content = this.#root.getElementById("content");

    try {
      const { data, raw } = parseStoredJson(this.#value);

      if (!data) {
        content.innerHTML = `<div class="empty muted">N/A</div>`;
        return;
      }

      const rows = makeRows(data)
        .map(([label, value]) => `<tr><th>${escapeHtml(label)}</th><td>${value === "N/A" ? `<span class="muted">N/A</span>` : value}</td></tr>`)
        .join("");

      const size = raw ? ` <small>(${(raw.length / 1024).toFixed(2)} KB)</small>` : "";
      const prettyJson = escapeHtml(JSON.stringify(data, null, 2));

      content.innerHTML = `
        <table>${rows}</table>
        <button id="toggle" type="button">Show JSON${size}</button>
        <pre>${prettyJson}</pre>
      `;

      content.querySelector("#toggle")?.addEventListener("click", () => {
        const showJson = this.toggleAttribute("show-json");
        content.querySelector("#toggle").innerHTML = `${showJson ? "Hide" : "Show"} JSON${size}`;
      });
    } catch (error) {
      content.innerHTML = `<div class="error">Could not parse Emply job JSON.</div>`;
    }
  }

}

customElements.define("limbo-umbraco-emply-job-data-property-editor", LimboUmbracoEmplyJobDataPropertyEditor);
export { LimboUmbracoEmplyJobDataPropertyEditor as element };
