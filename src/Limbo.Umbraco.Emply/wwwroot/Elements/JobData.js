const template = document.createElement("template");

template.innerHTML = `
  <style>
    :host {
      display: block;
      font-family: inherit;
    }

    .empty,
    .error {
      padding: 10px 12px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      background: #f9f7f7;
    }

    .error {
      border-color: #c63632;
    }

    table {
      width: 100%;
      border-collapse: collapse;
      margin-bottom: 12px;
    }

    th,
    td {
      padding: 8px 10px;
      border-bottom: 1px solid #e9e7e7;
      text-align: left;
      vertical-align: top;
    }

    th {
      width: 160px;
      color: #413659;
      font-weight: 600;
    }

    small,
    .muted {
      color: #817f85;
    }

    button {
      min-height: 32px;
      padding: 0 12px;
      border: 1px solid #817f85;
      border-radius: 3px;
      background: #fff;
      cursor: pointer;
    }

    button:hover {
      background: #f9f7f7;
    }

    pre {
      display: none;
      max-height: 520px;
      overflow: auto;
      margin-top: 12px;
      padding: 12px;
      border: 1px solid #d8d7d9;
      border-radius: 3px;
      background: #f9f7f7;
      white-space: pre-wrap;
      word-break: break-word;
    }

    :host([show-json]) pre {
      display: block;
    }
  </style>

  <div id="content"></div>
`;

function parseStoredJson(value) {
  if (!value) {
    return {
      data: null,
      raw: ""
    };
  }

  if (typeof value !== "string") {
    return {
      data: value,
      raw: JSON.stringify(value, null, 2)
    };
  }

  const raw = value.startsWith("_")
    ? value.substring(1)
    : value;

  if (!raw.trim()) {
    return {
      data: null,
      raw: ""
    };
  }

  return {
    data: JSON.parse(raw),
    raw
  };
}

function getText(value) {
  if (value == null) {
    return "";
  }

  if (
    typeof value === "string" ||
    typeof value === "number" ||
    typeof value === "boolean"
  ) {
    return String(value);
  }

  return value.value
    ?? value.title
    ?? value.name
    ?? value.label
    ?? "";
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function formatDateHtml(value) {
  if (!value) {
    return `<span class="muted">N/A</span>`;
  }

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return escapeHtml(value);
  }

  const label = new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(date);

  const diffSeconds = Math.round((date.getTime() - Date.now()) / 1000);

  const units = [
    ["year", 31536000],
    ["month", 2592000],
    ["week", 604800],
    ["day", 86400],
    ["hour", 3600],
    ["minute", 60]
  ];

  const formatter = new Intl.RelativeTimeFormat(undefined, {
    numeric: "auto"
  });

  for (const [unit, seconds] of units) {
    if (Math.abs(diffSeconds) >= seconds) {
      const relative = formatter.format(Math.round(diffSeconds / seconds), unit);
      return `${escapeHtml(label)} <small>(${escapeHtml(relative)})</small>`;
    }
  }

  const relative = formatter.format(diffSeconds, "second");
  return `${escapeHtml(label)} <small>(${escapeHtml(relative)})</small>`;
}

function getCategories(data) {
  if (!Array.isArray(data?.data)) {
    return [];
  }

  const categories = [];

  for (const item of data.data) {
    const title = getText(item?.title);

    if (title !== "Stillingskategori" && title !== "Job category") {
      continue;
    }

    if (!Array.isArray(item.value)) {
      continue;
    }

    for (const value of item.value) {
      const label = getText(value?.title ?? value);

      if (label) {
        categories.push(label);
      }
    }
  }

  return categories;
}

function textCell(value) {
  const text = getText(value);

  if (!text) {
    return `<span class="muted">N/A</span>`;
  }

  return escapeHtml(text);
}

function makeRows(data) {
  const categories = getCategories(data);
  const edited = data?.edited ?? data?.updated ?? data?.created;
  const deadline = data?.deadlineUTC ?? data?.deadlineUtc ?? data?.deadline;

  return [
    {
      label: "Job ID",
      value: data?.jobId == null
        ? `<span class="muted">N/A</span>`
        : escapeHtml(data.jobId)
    },
    {
      label: "Titel",
      value: textCell(data?.title)
    },
    {
      label: "Oprettet",
      value: formatDateHtml(data?.created)
    },
    {
      label: "Opdateret",
      value: formatDateHtml(edited)
    },
    {
      label: "Deadline",
      value: formatDateHtml(deadline)
    },
    {
      label: "Location",
      value: data?.location?.address
        ? escapeHtml(data.location.address)
        : `<span class="muted">N/A</span>`
    },
    {
      label: categories.length === 1 ? "Kategori" : "Kategorier",
      value: categories.length
        ? escapeHtml(categories.join(", "))
        : `<span class="muted">N/A</span>`
    }
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
    if (!this.#root) {
      return;
    }

    const content = this.#root.getElementById("content");

    try {
      const { data, raw } = parseStoredJson(this.#value);

      if (!data) {
        content.innerHTML = `<div class="empty muted">N/A</div>`;
        return;
      }

      const rows = makeRows(data)
        .map(row => `
          <tr>
            <th>${escapeHtml(row.label)}</th>
            <td>${row.value}</td>
          </tr>
        `)
        .join("");

      const size = raw
        ? ` <small>(${(raw.length / 1024).toFixed(2)} KB)</small>`
        : "";

      const prettyJson = escapeHtml(JSON.stringify(data, null, 2));

      content.innerHTML = `
        <table>
          ${rows}
        </table>

        <button id="toggle" type="button">
          Show JSON${size}
        </button>

        <pre>${prettyJson}</pre>
      `;

      const toggleButton = content.querySelector("#toggle");

      toggleButton?.addEventListener("click", () => {
        const showJson = this.toggleAttribute("show-json");

        toggleButton.innerHTML = `${showJson ? "Hide" : "Show"} JSON${size}`;
      });
    } catch (error) {
      content.innerHTML = `
        <div class="error">
          Could not parse Emply job JSON.
        </div>
      `;
    }
  }

}

customElements.define(
  "limbo-umbraco-emply-job-data-property-editor",
  LimboUmbracoEmplyJobDataPropertyEditor
);

export {
  LimboUmbracoEmplyJobDataPropertyEditor as element
};
