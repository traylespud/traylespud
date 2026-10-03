# Learning plan: userscripts, extensions & the browser

A step-by-step path for learning how userscripts and browser extensions work
in Chromium browsers (Thorium), using one real project as the thread that ties
it together: **a script that automatically translates Pornolab from Russian to
English, with no clicks or prompts.**

Lessons are written **one at a time, when asked**. Each one ends with
something that actually runs in Thorium. Lesson files will appear in this
folder as `01-*.md`, `02-*.md`, and so on.

## Ground rules for this project

These keep the project private and keep NSFW content off Google's services:

- **The translator runs in your browser only.** The Google Cloud VM is a
  workbench for writing code; it never visits Pornolab or logs in with your
  account.
- **Only text from safe pages goes to the translation API**: the homepage and
  guide/FAQ threads that don't have pictures. The script will have an explicit
  "allowed pages" list, and anything not on it is left alone.
- **No images are ever sent anywhere.** The script only reads text.
- **Saved pages in `reference/` are text-only**: copies of the homepage and
  guide threads with images removed, used for testing.
- **The API key never goes in this repo.** It's stored in Tampermonkey's own
  storage (`GM_setValue`) on your computer.

## The lessons

| # | Lesson | What you'll understand | What you'll build |
|---|--------|------------------------|-------------------|
| 1 | **How a web page is built** | The DOM (the tree of elements the browser builds from HTML), text nodes, using DevTools (F12) to inspect a page | Find Pornolab's fixed menu text in DevTools |
| 2 | **Headers, requests & user agents** | What really happens when the browser loads a page: requests and responses, the headers on each (`User-Agent`, `Accept-Language`, `Cookie`, `Referer`, `Content-Type`), status codes, how cookies keep you logged in, what the user agent tells a site and why sites change behavior based on it, CORS (why one site can't freely read another), and watching all of it live in DevTools' **Network** tab | Read the requests Pornolab's homepage makes, and spot which headers carry your login and language |
| 3 | **Userscript basics** | What Tampermonkey does, the metadata block (`@match`, `@grant`, `@run-at`, `@connect`), and why userscripts run in a sandbox separate from the page's own code | A script that swaps fixed menu words using a small built-in Russian→English list (no internet needed) |
| 4 | **Talking to other sites** | `GM_xmlhttpRequest` and why it can skip CORS, the headers *your script* sends, API keys and keeping them safe, JSON | Translate the homepage and guide-thread text with the Google Cloud Translation API |
| 5 | **Dynamic pages & memory** | `MutationObserver` (noticing new content), caching with `GM_setValue`, avoiding repeat API calls | Fast, cached, fully automatic translation |
| 6 | **Extensions (Manifest V3)** | `manifest.json`, content scripts, the service worker, permissions, loading an unpacked extension in `chrome://extensions` | Rebuild the translator as a real extension |
| 7 | **How it all fits together** | Tampermonkey is itself an extension, the "Allow User Scripts" toggle in newer Chromium, and when to choose a userscript vs. an extension | A side-by-side comparison of the two versions |

## How to start a lesson

Just ask, for example *"cc, let's do lesson 1"*. Each lesson will include:

- a plain-language explanation
- small, well-commented code examples
- a hands-on exercise in Thorium
- a short "check yourself" section with a few questions
