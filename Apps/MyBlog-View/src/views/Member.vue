<template>
    <div class="user-container">
      <div class="user-item"
           :id="item.id"
           v-for="item in adminUsersList.list">
        <div class="user-icon">
            <Avatar :size="185" :cover="getUpdatedCoverUrl(item.headUrl)"></Avatar>
        </div>
        <div class="user-info">
          <div class="nick-name">{{item.userName}}</div>
          <div class="profession">
            <span class="blog-count">Articles:{{item.blogCount}}</span>
          </div>
            <div class="introduction"
               v-html="item.introduction">
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts" setup>
  import {reactive,onMounted} from "vue"
  import {getActiveAdminUsersAsync} from "@/httpRequest/UserRequest"
  import {getUpdatedCoverUrl} from '@/utility/common'

  interface UserInfo {
        id:string;
        headUrl: string;
        userName: string;
        blogCount : string;
        introduction: string;
    }

  const adminUsersList = reactive<{
    list: UserInfo[];
    totalCount: number;
    }>({
        list:[],
        totalCount:0
    });

  //获取全部管理员用户
    const loadUsersList = async ()=>{
        var res  = await getActiveAdminUsersAsync({
        pageIndex:1,
        pageSize:20
    });
        adminUsersList.list = res.userInfoDtos;
        adminUsersList.totalCount = res.count;
    };

    onMounted(async ()=>{
        await loadUsersList();
    });
  </script>
  
  <style lang="scss" scoped>
  .user-container {
    margin-top: 30px;
    padding: 0px;
    .user-item {
      display: flex;
      align-items: flex-start;
      background: #e4ecf2;
      margin-bottom: 20px;
      margin-top: 0px;
      padding: 10px 20px;
      border-radius: 40px;
      box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.2);
      transition: background-color 0.3s, color 0.3s;
      &:hover {
          background-color: #90acc4;  // 背景色变深
          color: #f5f7fc;  // 文字颜色变暗
          .user-info{
            color: #fff;
          }
          .user-info .blog-count{
            color: #fff;
          }
          .user-info .nick-name{
            color: #fff;
          }
        }

        transition: transition 0.3s ease-in-out;
        &:hover {
          transform: scale(1.1);  // 放大整体
          z-index: 1;  // 确保放大后的面板处于最上层
        }

      .user-icon {
        width: 180px;
        height: 180px;
        background: #ddd;
        border-radius: 100px;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow: hidden;
        img {
          max-width: 100%;
        }
      }
  
      .user-info {
        flex: 1;
        display: block;
        padding-top: 20px;
        margin-left: 20px;
        .nick-name {
            padding-left: 20px;
            font-size: 30px;
            color: #38415a;
            font-weight: bold;
        }
        .profession {
          margin-top: 10px;
        }
        span {
          color: #0b1c2c;
          display: inline-block;
          font-size: 14px;
          word-wrap: break-word;
          word-break: break-all;
          white-space: normal;
        }
  
        .blog-count {
          font-size: 20px;
          color: #38415a;
          margin-left: 20px;
          width: 100px;
        }
    .introduction {
          margin-top: 10px;
          padding-left: 20px;
          font-size: 14px;
          word-wrap: break-word !important;
          word-break: break-all !important;
          white-space: normal !important;
          img   {
            max-width: 100%;
                } 
            }
      }
    }
}
  </style>
  