import { createRouter, createWebHashHistory, RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'framework',
    component: () => import(/* webpackChunkName: "about" */ '@/views/Framework.vue'),
    children:[
      {
        path:'/',
        name:'index',
        meta:{title:"blogMain",activePath:"/"},
        component:()=>import(/* webpackChunkName: "about" */ '@/views/Index.vue')
      },{
        path:'/blog/:blogId',
        name:'blogDetail',
        meta:{title:"blogDetail",activePath:"/"},
        component:()=>import(/* webpackChunkName: "about" */ '@/views/BlogDetail.vue')
      },
      {
        path:'/category',
        name:'category',
        meta:{title:"category",activePath:"/category"},
        component:()=>import(/* webpackChunkName: "about" */ '@/views/Category.vue')
      },
      {
        path:'/category/:categoryId',
        name:'CategoryBlog',
        meta:{title:"categoryBlog",activePath:"/category"},
        component:()=>import(/* webpackChunkName: "about" */ '@/views/CategoryBlogList.vue')
      },
      {
        path:'/member',
        name:'member',
        meta:{title:"member",activePath:"/member"},
        component:()=>import(/* webpackChunkName: "about" */ '@/views/Member.vue')
      }
    ]
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

//设置页面的标签的名称
router.beforeEach((to, from, next) => {
  document.title = to.meta.title as string;
  next();
});

export default router
