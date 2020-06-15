import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import en from "vuetify/src/locale/en";

Vue.use(Vuetify);

export default new Vuetify({
	lang: {
		locales: { en },
		current: 'en',
	},
    icons: {
        iconfont: 'mdi',
    },
});
