<template>
  <div class="carousel">
    <el-carousel :interval="5000" arrow="always" height="400px">
      <el-carousel-item v-for="(item, index) in images" :key="index">
        <img :src="item" alt="carousel image" class="carousel-image" />
      </el-carousel-item>
    </el-carousel>
  </div>
    <div class="container">
        <div class="left">
            <div class="blog-list">
                <div v-for="item in postData.list" key:item.id>
                    <BlogItem
                          :blogId = item.id
                          :cover="getUpdatedCoverUrl(item.postCover)"
                          :title="item.postTitle"
                          :summary="item.summary"
                          :postType="item.postTypeName"
                          :createUserName="item.createUserName"
                          :createDate="item.createDate"></BlogItem>
                </div>
                <div class="sentinel" ref="sentinel"></div>
            </div>
        </div>
        <div class="right">

        <div class="category-path">
            <div class="part-title">
                    <span style="color: #454c62;">CATEGORIZED</span>
                    <router-link to="/category" class="a-link">More&gt;&gt;</router-link>
            </div>
            <div class="category-list">
                <div v-for="item in categoryList" key:item.order>
                    <CategoryItem :postTypeId="item.id"
                             :cover="getUpdatedCoverUrl(item.cover)"
                             :name="item.postTypeName"
                             :count="item.count"></CategoryItem>
                </div>
            </div>
        </div>
            
        <div  class="category-path">
            <div class="part-title">
                <div>
                    <span style="color: #454c62;">MEMBER</span>
                    <span class="adminCount"> {{ adminUsersList.totalCount }}</span>
                </div>
                    <router-link to="/member" class="a-link">More&gt;&gt;</router-link>
            </div>
            <div class="user-list">
                <div v-for="item in adminUsersList.list">
                    <UserItem :headUrl="getUpdatedCoverUrl(item.headUrl)"
                              :userName="item.userName"
                              :introduction="item.introduction"></UserItem>
                </div>
                <div class="info" v-if="adminUsersList.list.length > 20"><span>Click 'More' to see additional members.</span></div>
            </div>
        </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import BlogItem from '@/components/BlogItem.vue';
import {getPostsAsync} from '@/httpRequest/PostRequest'
import { getPostTypesAsync } from "@/httpRequest/PostTypeRequest";
import {getActiveAdminUsersAsync} from "@/httpRequest/UserRequest";
import { onMounted,onUnmounted,reactive,ref} from 'vue';
import {getUpdatedCoverUrl} from '@/utility/common';

interface Post {
    id:string;
    postCover: string;
    postTitle: string;
    summary: string;
    postTypeId:string;
    postTypeName:string;
    createUserName: string;
    createDate: string;
    }

    interface UserInfo {
        headUrl: string;
        userName: string;
        introduction: string;
    }

    const postData = reactive<{
    list: Post[];
    totalCount: number;
    pageSize: number;
    pageIndex: number;
}>({
    list: [],
    totalCount: 0,
    pageSize: 15,
    pageIndex: 1,
});

const adminUsersList = reactive<{
    list: UserInfo[];
    totalCount: number;
}>({
    list:[],
    totalCount:0
});

// 图片数组，包含四张图片的路径
const images = ref([
  '/Image/1.jpg',
  '/Image/2.jpg',
  '/Image/3.jpg',
  '/Image/4.jpg'
]);

const categoryList = ref();
//IntersectionObserver 是一个浏览器 API，用于异步观察目标元素与其祖先元素（或顶级文档视口）交叉状态的变化。主要用于实现懒加载等功能
//“交叉状态的变化”指的是目标元素（如一个 div）进入或离开其祖先元素（或整个浏览器视口）的可视区域。这意味着当你滚动页面时，目标元素从不可见变为可见，或者从可见变为不可见。这种状态的变化会被 IntersectionObserver 监视到。
const observer = ref<IntersectionObserver | null>(null);
//HTMLElement 是所有 HTML 元素的基类
const sentinel = ref<HTMLElement | null>(null);

//获取全部分类
const loadCategoryList = async () => {
  let result = await getPostTypesAsync({
    pageIndex: 1,
    pageSize: 5,
  });
  categoryList.value = result;
};

//获取全部的帖子
const loadPostsList = async ()=>{
    var res  = await getPostsAsync({
    pageIndex:postData.pageIndex,
    pageSize:postData.pageSize
  });
  // 保留之前的数据并追加新数据
  //将之前已经存在的帖子列表 (postData.list) 
  // 和新加载的帖子列表 (res.postListDto) 合并成一个新的列表，
  // 并将这个新列表赋值给 postData.list
  //... 可以展开一个数组,这里就是展开两个数组的每一个元素放到一个新的数组中
  postData.list = [...postData.list, ...res.postListDto]; 
  postData.totalCount = res.postCount;
};

//获取全部管理员用户
const loadUsersList = async ()=>{
    var res  = await getActiveAdminUsersAsync({
    pageIndex:1,
    pageSize:20
  });
   adminUsersList.list = res.userInfoDtos;
   adminUsersList.totalCount = res.count;
};

//这里的entries的值是下面的new IntersectionObserver(...)中回调时候传递进来的，包含所有的被观察元素的状态信息
const handleIntersect = async (entries: IntersectionObserverEntry[]) => {
    if (entries[0].isIntersecting && postData.list.length < postData.totalCount) {
        postData.pageIndex++;
        await loadPostsList();
    }
};

onMounted(async()=>{
 await loadCategoryList();
 await loadPostsList();
 await loadUsersList();

//下面就是定义了一个监听页面滚动到底部然后触发一个回调函数handleIntersect
//怎么判断是否滚动到底部的？
//我们在底部放了一个<div ref=sentinel>的dom元素，当翻页到最底部渲染这个元素时候则表示到底了然后触发回调函数

 //这行代码创建了一个新的 IntersectionObserver 实例。IntersectionObserver 的构造函数接收两个参数：
//回调函数 handleIntersect：这个回调函数会在观察的元素与视口（或祖先元素）的交叉状态发生变化时被调用。该函数的参数 entries 是一个数组，包含了所有被观察元素的状态信息
 observer.value = new IntersectionObserver(handleIntersect, {
        //root：定义用来检测交叉状态的祖先元素。如果设置为 null，则表示使用浏览器视口作为根元素
        root: null,
        //rootMargin：根元素的边距，用来扩展或缩小视口的范围。格式为 "10px 20px 30px 40px"（上、右、下、左）。这里设置为 "0px"，表示没有边距。
        rootMargin: '0px',
        //threshold：一个数组或单个数字，表示交叉比例的阈值。1.0 表示当目标元素完全可见时，才会触发回调。
        threshold: 1.0
    });
//如果 sentinel.value 存在（即哨兵元素已经被正确引用），那么就开始观察这个元素
//这里的sentinel是被渲染到页面上的
    if (sentinel.value) {
        //observer.value.observe(sentinel.value) 告诉 IntersectionObserver 开始观察 sentinel 元素的交叉状态
        observer.value.observe(sentinel.value);
    }
});

//在组件卸载（即销毁）时执行一些清理操作
onUnmounted(() => {
    //unobserve 停止观察目标元素
    if (observer.value && sentinel.value) {
        observer.value.unobserve(sentinel.value);
    }
});
</script>

<style lang="scss" scoped>

.demonstration {
  color: var(--el-text-color-secondary);
}

.carousel-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.carousel{
    width: 1350px;
    margin-top: 30px;
    margin-bottom: 40px;
    .el-carousel__item h3 {
        color: #FFFFFF;
        opacity: 1;
        line-height: 300px;
        margin: 0;
        text-align: center;
        font-size: 3em;
    }

    .el-carousel__item:nth-child(2n) {
         background-color: #99a9bf;
        }

    .el-carousel__item:nth-child(2n + 1) {
        background-color: #d3dce6;
        }
}



 .container{
    margin-top: 10px;
    display:flex;
    .left{
        flex: 1;
        background-color:#e7eaee;
        border-radius: 8px;
        padding: 10px;
        .pagination{
            margin-top: 20px;
            margin-bottom: 10px;
            display: flex;
            justify-content: flex-end;
        }
    }
    .right{
        margin-left: 10px;
        width: 300px;
        background-color: #e8eaf0;
        border-radius: 8px;
        .user-list {
            display: flex;
            flex-wrap: wrap;
            gap: 2px; // 适当调整间距
        }
        .info{
            display: flex; 
            align-items: center; 
            justify-content: center; 
            width: 100%; 
            font-size: 13px;
            margin-top: 20px;
            color: #999aaa;
        }
    }
 }
</style>