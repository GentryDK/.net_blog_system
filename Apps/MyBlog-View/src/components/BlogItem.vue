<template>
    <div class="blog-item">
        <div class="cover">
            <router-link :to="{ path: '/blog/' + String(blogId) }" class="a-link">
                <img :src="`${local}${props.cover}`" v-if="cover">
                <img v-else src="../../public/Image/Cover.png">
            </router-link>
        </div>
        <div class="content">
            <div class="title">
                <router-link :to="{ path: '/blog/' + String(blogId) }" class="a-link">{{ title }}</router-link>
            </div>
            <div class="summary">{{ summary }}</div>
            <div class="postInfo">
                <span>poster：{{ createUserName }}</span>
                <span>classify：{{ postType }}</span>
                <span>posting time：{{ transDate(props.createDate as string) }}</span>
            </div>
        </div>
       
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { superAxios } from "@/common/superAxios";
import {transDate} from '@/utility/common'
const local = ref<string>("");

const props = defineProps({
    blogId:{
        type: String,
    required: true
    },
    cover:String,
    title:String,
    summary:String,
    createUserName:String,
    postType:String,
    createDate:String
})

onMounted(async () => {
  const apiRoot = await superAxios.getValue("ImageUrl");
  if (apiRoot) {
    local.value = apiRoot;
  }
});
</script>

<style lang="scss" scoped>
.blog-item{
    display: flex;
    padding: 10px 0;
    border-bottom: 2px solid #ddd;
    padding-left: 10px;
    background-color:#f6f8f9 ;
    box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.2);
    border-radius: 10px;
    transition: background-color 0.3s ease;
    &:hover {
        background-color: #c8cfdb;
        border-radius: 10px;
        .cover {
            transform: scale(1.1);  // 图片放大
            transition: transform 0.3s ease;  // 平滑过渡效果
        }

        .content .title {
            color: #ececf5;  // 改变标题字体颜色
        }

        .content .summary, 
        .content .postInfo {
            color: #ececf5;  // 改变摘要和其他信息的字体颜色
        }
    }
    .cover{
        width: 110px;
        height: 110px;
        border-radius: 10px;
        background: #ddd;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow: hidden;
        img{
            width: 100%; 
        }
    }
    .content{
        flex: 1;
        margin-left: 15px;
        .title{
            color: #4b73eb;
            transition: color 0.3s ease;
        }
        .summary{
            word-break: break-all;
            margin-top: 15px;
            font-size: 14px;
            line-height: 22px;
            white-space: normal;
            color: #999aaa;
            display: block;
            overflow: hidden;
            text-overflow: ellipsis;
            display: -webkit-box;
            // -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            transition: color 0.3s ease;
        }
        .postInfo{
            word-break: break-all;
            margin-top: 15px;
            font-size: 14px;
            line-height: 22px;
            white-space: normal;
            color: #999aaa;
            display: block;
            overflow: hidden;
            text-overflow: ellipsis;
            display: -webkit-box;
            // -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            text-align: right;
            transition: color 0.3s ease;
            span{
                margin-right: 20px;
                padding-left: 20px;
            }
        }
    }
}
</style>