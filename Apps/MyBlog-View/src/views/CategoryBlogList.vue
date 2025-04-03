<template>
  <div class="category-body">
    <div class="container">
      <div class="category-panel">
        <div class="category-image">
          <template v-if="categoryInfo && categoryInfo.cover">
             <Cover :cover="getUpdatedCoverUrl(categoryInfo.cover)"></Cover>
          </template>
        </div>
        <div class="category-info">
          <div class="category-title">{{categoryInfo?.postTypeName}}</div>
          <div class="category-desc">{{categoryInfo?.typeBrief}}</div>
          <div class="blog-count">articles：{{categoryInfo?.count}}</div>
        </div>
      </div>
      <div class="blog-list">
        <div class="blog-item"
             v-for="(item, index) in postData.list">
          <div class="blog-cover">
                 <Cover :cover=getUpdatedCoverUrl(item.postCover)></Cover>
          </div>
          <div class="blog-item-content">
            <div class="title">
              <router-link :to="'../blog/' + item.id" style="text-decoration: none; font-weight: bold; color: #4b73eb;">{{
              item.postTitle
            }}</router-link>
            </div>
            <div class="blog-summary">{{ item.summary }}</div>
            <div class="blog-info">
              <div class="create-time">Create time：{{ transDate(item.createDate) }}</div>
              <div class="nick-name">
                Author：{{ item.createUserName }}
              </div>
              <div class="nick-name">
                Categorized：{{item.postTypeName}}
              </div>
            </div>
          </div>
        </div>
        <Pagination :total="postData.totalCount"
                    :pageSize="postData.pageSize"
                    :pageNo="postData.pageIndex"
                    @pageChange="pageChangeBlogList"></Pagination>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import "@/assets/scss/blog.list.scss"
import "@/assets/scss/category.scss"
import { ref,reactive,onMounted} from "vue";
import {getPostTypeAsync} from "@/httpRequest/PostTypeRequest";
import {getPostsAsync} from '@/httpRequest/PostRequest';
import {transDate,getUpdatedCoverUrl} from '@/utility/common';
import { useRoute } from "vue-router";
const route = useRoute();
const categoryId = ref(route.params.categoryId);

//分页
const pageChangeBlogList = (pageNo:number) => {
  postData.pageIndex = pageNo;
  loadPostsList();
};

interface IpostType{
  cover:string,
  postTypeName:string,
  order:string,
  typeBrief :string,
  count:string,
}

interface Post {
    id:string;
    postCover: string;
    postTitle: string;
    summary: string;
    postTypeName:string;
    createUserName: string;
    createDate: string;
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

//获取当前分类的信息
const categoryInfo = ref<IpostType>();
const loadCategory = async () => {
  if (!categoryId.value) return;
  let result = await getPostTypeAsync(String(categoryId.value));
  categoryInfo.value = result;
  console.log(result);
};

//获取当前分类下的的帖子
const loadPostsList = async ()=>{
    var res  = await getPostsAsync({
    pageIndex:postData.pageIndex,
    pageSize:postData.pageSize,
    postTypeId:String(categoryId.value)
  });
  postData.list = res.postListDto; 
  postData.totalCount = res.postCount;
};

onMounted(async () => {
 await loadCategory();
 await loadPostsList();
});
</script>

<style lang="scss" scoped>
.category-body {
  display: flex;
  .container {
    flex: 1;
    .category-panel {
      height: 150px;
      background: #fff;
      border-radius: 15px;
      display: flex;
      padding: 10px 20px;
      .category-image {
        width: 130px;
        height: 130px;
        background: #ddd;
        border-radius: 4px;
        display: flex;
        align-items: center;
        img {
          border-radius: 2px;
          max-width: 100%;
        }
      }
      .category-info {
        flex: 1;
        margin-left: 20px;
        .category-title {
          font-size: 30px;
          font-weight: bold;
          text-decoration: none;
          color: #4b73eb;
        }
        .category-desc {
          margin-top: 10px;
        }
        .blog-count {
          margin-top: 10px;
          color: #919da9;
        }
      }
    }
    .blog-list {
      border-radius: 15px;
      margin-top: 10px;
      background: #fff;
      padding: 10px 20px;
    }
  }
}
</style>
