<template>
  <div class="block">
          <el-avatar :size="props.size" :src="`${local}${props.cover}`"  v-if="cover" />
          <el-avatar v-else src="../../public/Image/avatarIcon.png" />
        </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { superAxios } from "@/common/superAxios";

const props = defineProps({
  size:{
    type:Number
  },
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
</style>


