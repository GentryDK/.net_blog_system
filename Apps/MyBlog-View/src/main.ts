import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import router from './router'
import { createPinia } from "pinia";
import 'element-plus/dist/index.css'
import "highlight.js/styles/atom-one-light.css";
import '@/assets/scss/base.scss'
import '@/assets/scss/blog.detail.scss'
import '@/assets/scss/blog.list.scss'
import '@/assets/scss/category.scss'
import Blogltem from '@/components/BlogItem.vue'
import CategoryItem from '@/components/CategoryItem.vue'
import UserItem from '@/components/UserItem.vue';
import Pagination from '@/components/Pagination.vue'
import Avatar from '@/components/Avatar.vue'
import Cover from '@/components/Cover.vue'

const app = createApp(App)
const pinia = createPinia();

app.use(router)
app.use(ElementPlus)
app.use(pinia)
app.mount('#app')

// 这行代码用于全局注册 BlogItem 组件。
// 具体来说，它将 BlogItem 组件注册为一个名为 "BlogItem" 的全局组件，
// 这样你就可以在整个应用程序中使用 <BlogItem /> 标签来引用这个组件
app.component("Blogltem", Blogltem);
app.component("UserItem", UserItem);
app.component("CategoryItem", CategoryItem);
app.component("Pagination", Pagination);
app.component("Avatar", Avatar);
app.component("Cover", Cover);