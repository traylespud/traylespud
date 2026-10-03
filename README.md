# traylespud userscripts

A TypeScript-tooled workspace for writing [Tampermonkey](https://www.tampermonkey.net/)
userscripts. It uses [Vite](https://vitejs.dev/) and
[vite-plugin-monkey](https://github.com/lisonge/vite-plugin-monkey), so you get
modern tooling: TypeScript type-checking, full autocomplete for the `GM_*` API,
a live-reloading dev server, and a one-command build that produces an
installable userscript.

If you have never used any of this before, don't worry — follow the steps below
in order.

## What you need first

1. **Node.js 18 or newer.** Check with `node --version`. If you don't have it,
   install it from [nodejs.org](https://nodejs.org/).
2. **The Tampermonkey browser extension**, installed in your browser. Get it
   from [tampermonkey.net](https://www.tampermonkey.net/).

## Setup

Install the project dependencies once:

```bash
npm install
```

## Dev mode (write and test live)

```bash
npm run dev
```

This starts a Vite dev server and prints a URL in your terminal. With
Tampermonkey installed, open that URL — Tampermonkey auto-detects the dev
script and offers to install it. Once installed, visit a page that matches your
script (by default `https://example.com/`) and your script runs there. As you
edit `src/main.ts`, the page live-reloads with your changes.

## Build (make an installable script)

```bash
npm run build
```

This produces an installable file at `dist/*.user.js`. To install it, drag that
file into your browser (or open it) and Tampermonkey will prompt you to install.
This is the file you share or keep as the finished userscript.

## Type-check (optional but recommended)

```bash
npm run typecheck
```

Runs TypeScript with no output, just to catch type errors before you build.

## Changing which sites your script runs on

Open `vite.config.ts` and edit the `match` array inside the `userscript` block.
For example, to run on GitHub:

```ts
match: ['https://github.com/*'],
```

You can list several patterns. See the
[Tampermonkey `@match` docs](https://www.tampermonkey.net/documentation.php#meta:match)
for the pattern syntax.

## Granting `GM_*` powers

Some Tampermonkey features (storage, cross-origin requests, clipboard, etc.)
require an explicit grant. Add the ones you need to the `grant` array in
`vite.config.ts`, for example:

```ts
grant: ['GM_setValue', 'GM_getValue', 'GM_xmlhttpRequest'],
```

Then call them from `src/main.ts` — the types are already available.

## Adding another userscript later

This project builds **one userscript per Vite build**. The metadata for that
script lives in the `userscript` block in `vite.config.ts`. To add a second,
independent script, the simplest approach is to copy this repo's config
pattern: give the new script its own entry file (e.g. `src/other.ts`) and its
own `userscript` block, and run a separate build for it. Keep each script
self-contained so it's easy to reason about.

## The `reference/` folder

Use `reference/` to drop in anything that helps you build: saved HTML of a
target page, existing scripts you're adapting, or plain notes. Nothing there is
bundled into your userscript — it's just a scratch area for you.

## The `learning/` folder

Step-by-step lessons on how userscripts, extensions, and the browser work,
built around one real project. Start with [`learning/README.md`](learning/README.md).
