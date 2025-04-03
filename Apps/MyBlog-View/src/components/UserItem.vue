<template>
    <div class = "category-item">
        <div class="cover">
            <img :src="`${local}${props.headUrl}`" v-if="headUrl" />
            <img v-else src="../assets/logo.png" />
        </div>
        <div class="text">
            <div class="name">{{ userName }}</div>
            <div class="introduction">{{introduction}}</div>
        </div>
    </div>
</template>

<script  setup lang="ts">
import { ref, onMounted } from "vue";
import { superAxios } from "@/common/superAxios";

const local = ref<string>("");

const props = defineProps({
    headUrl:{
        type:String,
    },
    userName:{
        type:String,
    },
    introduction:{
        type:String,
    }
});

onMounted(async () => {
  const apiRoot = await superAxios.getValue("ImageUrl");
  if (apiRoot) {
    local.value = apiRoot;
  }
});
</script>

<style lang="scss" scoped>
.category-item{
    display: flex;
    padding-bottom: 20px;
    align-items: center;
    .cover{
        background: #ebedf5;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 65px;
        width: 65px;
        border-radius: 50%;
        overflow: hidden;
        img{
            width: 100%;
            height: 100%;
            object-fit: cover; // 确保图片完整填充容器并保持比例
            border-radius: 50%; // 确保图片本身也是圆形
        }
    }
    .text{
        display: flex;
        flex-direction: column;
        margin-left: 10px;
        .name{
            font-size: 20px;
            font-weight: bold;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            color: #537ee4;
        }
        .introduction{
            width: 170px;
            font-size: 13px;
            text-align: left;
            word-break: keep-all;
            white-space: nowrap;
            overflow: hidden;
            color: #999aaa;
        }
    }
}
</style>