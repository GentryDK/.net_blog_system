<template>
  <div>
    <v-md-editor
      :model-value="modelValue"
      :height="height + 'px'"
      :disabled-menus="[]"
      :include-level="[1, 2, 3, 4, 5, 6]"
      @upload-image="handleUploadImage"
      @change="change"
    >
    </v-md-editor>
  </div>
</template>

<script setup lang="ts">
import { setPostPictureAsync } from "@/httpRequest/PostRequest";
import { superAxios } from "@/common/superAxios";

const props = defineProps({
  modelValue: {
    type: String,
    default: "",
  },
  height: {
    type: Number,
    default: 400,
  },
});

const emit = defineEmits();

const change = (markdownContent: any, htmlContent: any) => {
  emit("update:modelValue", markdownContent);
  emit("htmlContent", htmlContent);
};

const handleUploadImage = async (event: any, insertImage: any, files: any) => {
  let result = await setPostPictureAsync(files[0]);

  if (!result) {
    return;
  }

  let local = await superAxios.getValue("ImageUrl");
  const url = local + result;
  insertImage({
    url: url,
    desc: "图片",
  });
};
</script>

<style lang="scss" scoped></style>
