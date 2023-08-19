/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./resources/**/*.blade.php",
    "./resources/**/*.js",
    "./resources/**/*.vue",
  ],
  theme: {
    extend: {
        width: {
            '128': '32rem',
        },
        height: {
            '128': '32rem',
        }
    },
  },
  plugins: [],
}

