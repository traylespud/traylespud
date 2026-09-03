import { defineConfig } from 'vite';
import monkey from 'vite-plugin-monkey';

// One userscript = one Vite build. To add another script later, copy this
// config's `userscript` block or add a second build entry.
export default defineConfig({
  plugins: [
    monkey({
      entry: 'src/main.ts',
      userscript: {
        name: 'Traylespud Example',
        namespace: 'https://github.com/traylespud/traylespud',
        version: '0.1.0',
        description: 'Example scaffold userscript — replace with your own.',
        author: 'traylespud',
        match: ['https://example.com/*'],
        grant: [],
        // icon: 'https://www.google.com/s2/favicons?sz=64&domain=example.com',
      },
    }),
  ],
});
