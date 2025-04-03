<template>
    <div class = "category-item">
        <div class="category-icon">
            <img :src="`${local}${props.cover}`" v-if="cover" />
            <img v-else src="../assets/logo.png" />
        </div>
        <div class="category-name">
            <router-link :to="{path:'/category/'+String(props.postTypeId)}" class="a-link">{{ name }}</router-link>
        </div>
        <div class="category-count">{{count}}articles</div>
    </div>
</template>

<script  setup lang="ts">
import { ref, onMounted } from "vue";
import { superAxios } from "@/common/superAxios";

const local = ref<string>("");

const props = defineProps({
    postTypeId:{
        type:String,
    },
    cover:{
        type:String,
    },
    name:{
        type:String,
    },
    count:{
        type:Number,
        default:0
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
</style>