// Import Tailwind CSS
import '../src/assets/main.css';

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import './styles.scss'
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'
import 'bootstrap-icons/font/bootstrap-icons.css'
import '@fortawesome/fontawesome-free/css/all.css'

// import 'bootstrap/dist/css/bootstrap.min.css'
// import 'bootstrap/dist/js/bootstrap.bundle.min.js'

import '@fortawesome/fontawesome-free/css/all.css'
import '@fortawesome/fontawesome-free/js/all.js'

const options = {
  position: 'top-right',
  timeout: 1000,
  closeOnClick: true,
  pauseOnHover: true,
}

const app = createApp(App)
app.use(router)
app.use(Toast, options)
app.mount('#app')
