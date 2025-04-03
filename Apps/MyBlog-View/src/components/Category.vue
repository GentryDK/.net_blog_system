<template>
    <div class="category-body">
      <div class="category-list">
        <div class="category-panel" v-for="item in categoryList" :key="item.id">
          <div class="category-panel-inner">
            <div class="category-image">
              <img :src="`${local}${getUpdatedCoverUrl(item.cover)}`" v-if="item.cover" />
              <img src="@/assets/logo.png" v-else />
            </div>
            <div class="category-info">
              <div class="category-title" :title="item.postTypeName">
                <router-link :to="{path:'/category/'+String(item.id)}" class="category-link">
                  {{ item.postTypeName }}
                </router-link>
              </div>
              <div class="category-desc" :title="item.typeBrief">{{ item.typeBrief }}</div>
              <div class="blog-count">Number of articles：{{ item.count }}</div>
            </div>
          </div>
        </div>
      </div>
      <div class="page-panel">
        <Pagination
          :total="postTypeDataInfo.totalCount"
          :pageSize="postTypeDataInfo.pageSize"
          :pageNo="postTypeDataInfo.pageNo"
          @pageChange="pageChange4BlogList"
        ></Pagination>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, reactive, onMounted } from 'vue';
  import { getPostTypesAsync, getPostTypeCountAsync } from "@/httpRequest/PostTypeRequest";
  import { getUpdatedCoverUrl } from '@/utility/common';
  import { superAxios } from "@/common/superAxios";
  
  interface CategoryItem {
  id: number;
  cover: string;
  postTypeName: string;
  typeBrief: string;
  count: number;
}

  const categoryList = ref<CategoryItem[]>([]);
  const local = ref<string>("");
  
  const postTypeDataInfo = reactive({
    pageNo: 1,
    pageSize: 8,
    totalCount: 0
  });
  
  // 获取当前所有的发帖类型的总和
  const loadPostTypeCount = async () => {
    let result = await getPostTypeCountAsync();
    postTypeDataInfo.totalCount = result;
  };
  
  // 获取全部分类
  const loadCategoryList = async () => {
    let result = await getPostTypesAsync({
      pageIndex: postTypeDataInfo.pageNo,
      pageSize: postTypeDataInfo.pageSize,
    });
    categoryList.value = result;
  };
  
  // 分页处理
  const pageChange4BlogList = async (pageNo: number) => {
    postTypeDataInfo.pageNo = pageNo;  // 更新当前页
    await loadCategoryList();          // 重新加载分类数据
  };
  
  const getDefaultImgUrl = async () => {
    const apiRoot = await superAxios.getValue("ImageUrl");
    if (apiRoot) {
      local.value = apiRoot;
    }
  };
  
  onMounted(async () => {
    await getDefaultImgUrl();
    await loadCategoryList();
    await loadPostTypeCount();
  });
  </script>
  
  <style lang="scss" scoped>
  .category-body {
    margin-top: 30px;
    background:#e7eaee;
    height: 400px;
    border-radius: 20px;
    overflow: hidden;
    position: relative;
    .category-list {
      background: #e7eaee;
      padding: 10px 10px;
      display: flex;
      flex-wrap: wrap;
      .category-panel {
        width: 25%;
        padding: 10px;
        transition: transition 0.3s ease-in-out;
        &:hover {
          transform: scale(1.1);  // 放大整体
          z-index: 1;  // 确保放大后的面板处于最上层
        }
        .category-panel-inner {
          background: #fff;
          padding: 10px;
          border: 4px solid #90acc4;
          border-radius: 15px;
          overflow: hidden;
          transition: background-color 0.3s, color 0.3s;  // 平滑过渡颜色
        &:hover {
          background-color: #90acc4;  // 背景色变深
          color: #63729b;  // 文字颜色变暗
          .category-info{
            color: #fff;
            .category-title {
            .category-link{
                color: #fff;
              }
            }
          }
          .category-info .blog-count{
            color: #fff;
          }

          img {
            transform: scale(1.1);  // 鼠标悬停时图片变大
          }

        }
          .category-image {
            width: 100px;
            height: 100px;
            background: #ddd;
            border-radius: 10px;
            display: flex;
            align-items: center;
            float: left;
            justify-content: center;  // 确保图片居中显示
            transition: transform 0.3s ease-in-out;  // 图片平滑过渡
            img {
              width: 100%;
              height: 100%;
              border-radius: 10px;
              object-fit: cover;
              transition: transform 0.6s ease-in-out;
            }
          }
          .category-info {
            margin-left: 110px;
            font-size: 14px;
            .category-title {
              max-width: 100%;
              font-size: 20px;
              font-weight: bold;
              overflow: hidden;
              white-space: nowrap;
              text-overflow: ellipsis;
              -o-text-overflow: ellipsis;
              .category-link{
                text-decoration: none;
                color: #4b73eb;
              }
            }
            .category-desc {
              max-width: 100%;
              margin-top: 10px;
              overflow: hidden;
              white-space: nowrap;
              text-overflow: ellipsis;
              -o-text-overflow: ellipsis;
            }
            .blog-count {
              font-size: 13px;
              margin-top: 10px;
              color: #919da9;
            }
          }
        }
      }
    }
    .page-panel {
      position: absolute;
      bottom: 10px;
      right: 20px;
      padding: 0px 0px 10px 20px;
    }
  }
  </style>
  
  
  