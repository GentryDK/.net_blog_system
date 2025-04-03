import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import ElementPlus, { dialogEmits } from "element-plus";
import * as Icons from "@element-plus/icons-vue";

import VueMarkdownEditor from "@kangc/v-md-editor";
import "@kangc/v-md-editor/lib/style/base-editor.css";
import vuepressTheme from "@kangc/v-md-editor/lib/theme/vuepress.js";
import "@kangc/v-md-editor/lib/theme/style/vuepress.css";

import Prism from "prismjs";

import "@/assets/icon/iconfont.css";
import "element-plus/dist/index.css";
import "./styles/tailwindcss.css";
import "@/styles/style.css";

import Table from "@/components/Table.vue";
import Cover from "@/components/Cover.vue";
import Dialog from "@/components/Dialog.vue";
import CoverUpload from "@/components/CoverUpload.vue";
import Windows from "@/components/Window.vue";
import EditorMarkdown from "@/components/EditorMarkdown.vue";
import EditorHtml from "@/components/EditorHtml.vue";

const app = createApp(App);

for (const [key, component] of Object.entries(Icons)) {
  app.component(key, component);
}

VueMarkdownEditor.use(vuepressTheme, {
  Prism,
});

app.use(router);
app.use(ElementPlus);

app.component("Table", Table);
app.component("Cover", Cover);
app.component("Dialog", Dialog);
app.component("CoverUpload", CoverUpload);
app.component("Windows", Windows);
app.component("EditorMarkdown", EditorMarkdown);
app.use(VueMarkdownEditor);
app.mount("#app");
