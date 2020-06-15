import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify';
import ServicesFactory from "@/services/ServicesFactory";

Vue.config.productionTip = false;
Vue.use(ServicesFactory);

new Vue({
    // @ts-ignore
    vuetify,
    render: h => h(App)
}).$mount('#app')
