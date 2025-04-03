import { createRouter, createWebHashHistory, RouteRecordRaw } from "vue-router";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "framework",
    component: () =>
      import(/* webpackChunkName: "framework" */ "../views/Framework.vue"),
    //路由重定向
    redirect: "/blogManager",
    children: [
      {
        path: "/blogManager",
        name: "blogManager",
        component: () =>
          import(
            /* webpackChunkName: "blogManager" */ "@/views/blogs/BlogManager.vue"
          ),
      },
      {
        path: "/BlogTypeManager",
        name: "BlogTypeManager",
        component: () =>
          import(
            /* webpackChunkName: "BlogTypeManager" */ "@/views/blogs/BlogTypeManager.vue"
          ),
      },
      {
        path: "/myInfo",
        name: "myInfo",
        component: () =>
          import(
            /* webpackChunkName: "myInfo" */ "@/views/settings/MyInfo.vue"
          ),
      },
      {
        path: "/BlogUser",
        name: "BlogUser",
        component: () =>
          import(
            /* webpackChunkName: "myInfo" */ "@/views/settings/BlogUser.vue"
          ),
      },
      {
        path: "/blogRecycleBin",
        name: "blogRecycleBin",
        component: () =>
          import(
            /* webpackChunkName: "myInfo" */ "@/views/recycleBin/blogRecycleBin.vue"
          ),
      },
      {
        path: "/UploadSensitiveWords",
        name: "UploadSensitiveWords",
        component: () =>
          import(
            /* webpackChunkName: "myInfo" */ "@/views/settings/UploadSensitiveWords.vue"
          ),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
