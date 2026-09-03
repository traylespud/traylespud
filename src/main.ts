// vite-plugin-monkey injects GM_* / GM.* types automatically — you get
// full autocomplete for the Tampermonkey API in this file.

(function () {
  'use strict';

  console.log('[Traylespud] userscript loaded on', location.href);

  // Example: add a floating button to the page.
  const btn = document.createElement('button');
  btn.textContent = 'Hello from Traylespud';
  Object.assign(btn.style, {
    position: 'fixed',
    bottom: '16px',
    right: '16px',
    zIndex: '99999',
    padding: '8px 12px',
    borderRadius: '8px',
    border: 'none',
    background: '#4f46e5',
    color: '#fff',
    cursor: 'pointer',
    font: '14px system-ui, sans-serif',
  });
  btn.addEventListener('click', () => alert('It works! Edit src/main.ts to build your own.'));
  document.body.appendChild(btn);
})();
