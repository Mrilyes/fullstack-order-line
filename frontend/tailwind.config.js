/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'navbar-bg': '#003f7f',
        // Add other custom colors here
      },
      spacing: {
        '18': '4.5rem', // Example of adding a custom spacing
      },
      // Add custom breakpoints if needed
      screens: {
        screens: {
          'xs': '475px',
          'sm': '640px',
          'md': '768px',
          'lg': '1024px',
          'xl': '1280px',
          '2xl': '1536px',
        },

      },
    },
  },
  plugins: [],
}
