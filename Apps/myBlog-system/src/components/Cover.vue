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
  background-color: #ddd;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 60px;
  height: 40px;
  overflow: hidden;
  img {
    width: 100%;
  }
}
</style>
