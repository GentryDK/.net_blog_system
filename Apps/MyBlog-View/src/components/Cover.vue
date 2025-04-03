<template>
  <div class="cover">
    <img :src="`${local}${props.cover}`" v-if="cover" />
    <img v-else src="../assets/logo.png" />
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { superAxios } from "@/common/superAxios";

const props = defineProps({
  cover: {
    type: String,
  },
});

const local = ref<string>("");

onMounted(async () => {
  const apiRoot = await superAxios.getValue("ImageUrl");
  if (apiRoot) {
    local.value = apiRoot;
  }
});
</script>

<style lang="scss">
.cover {
  width: 100%; 
  height: 100%; 
  display: flex;
  justify-content: center;
  align-items: center;
  img {
    width: 100%; /* 让图片自适应宽度 */
    height: 100%; /* 让图片自适应高度 */
    object-fit: cover;  /* 确保图片不会超出父元素并且保持比例 */
    border-radius: 10px;
  }
}
</style>

