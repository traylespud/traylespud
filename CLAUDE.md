# CLAUDE.md

Notes for future Claude Code sessions working in this repo.

## What this project is

A tooled Tampermonkey userscript workspace built on **vite-plugin-monkey +
TypeScript**. It exists to make writing, type-checking, and building
userscripts pleasant.

## Layout & conventions

- Entry point: `src/main.ts`.
- Userscript metadata (name, `@match`, `@grant`, version, etc.) lives in the
  `userscript` block in `vite.config.ts` — not in a header comment.
- `npm run build` outputs an installable `dist/*.user.js`.
- `npm run typecheck` runs `tsc --noEmit` for a TypeScript check.
- `npm run dev` starts the Vite dev server for live-reload development.
- `GM_*` / `GM.*` API typings are injected automatically by the plugin.

## Adding more scripts

One userscript = one Vite build entry. To add another script, give it its own
entry file and its own `userscript` block, and build it separately. Keep each
script self-contained.

## Working with the user

The user is a **beginner**. Favor clear, plain-language explanations over
jargon. Keep scripts self-contained and easy to follow. Prefer explicit,
well-commented example code.

## Reference material

Anything used as input (target-page HTML, existing scripts, notes) goes in
`reference/`. It is not bundled into the build.
