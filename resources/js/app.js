import './bootstrap.js';
import 'vuestic-ui/css';

import { createApp } from 'vue';
import { createVuestic } from 'vuestic-ui';

import loginPage from './pages/login.vue';
import dashboardPage from './pages/dashboard.vue';
import accountPage from './pages/account.vue';
import settingsPage from './pages/settings.vue';

const app = createApp({
    methods: {
        // TODO: For future use
        arrayFind(array, condition) {
            const item = array.find(condition);
            return array.indexOf(item);
        },
        prompt(msg = "Oops, something went wrong!") {
            this.$vaToast.init({ message: msg, position: 'bottom-right' });
        },
        redirectToPage(url) {
            window.location = url;
        }
    }
});

app.component('login', loginPage);
app.component('dashboard', dashboardPage);
app.component('account', accountPage);
app.component('settings', settingsPage);

app.use(createVuestic()).mount('#app');
