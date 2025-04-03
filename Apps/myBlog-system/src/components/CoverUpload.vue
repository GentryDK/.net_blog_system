<template>
  <div>
    <el-upload
      name="filed"
      :show-file-list="false"
      accept=".png,.jpg"
      :multiple="false"
      :before-upload="handlePreview"
      :on-remove="handleRemove"
      :http-request="uploadImage"
    >
      <img v-if="imageUrl" :src="`${imageUrl}`" class="avatar" />
      <el-icon v-else class="avatar-uploader-icon"><Plus /></el-icon>
    </el-upload>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { Plus } from "@element-plus/icons-vue";

const props = defineProps({
  modelValue: {
    type: String,
    default: undefined,
  },
  local: {
    type: String,
    default: "",
  },
});

const emit = defineEmits(["update:modelValue", "upload"]);
const fileData = ref<File | null>(null);
const imageUrl = ref<string | undefined>(props.modelValue);

// watch 可以直接传递 imageUrl，
// 因为 imageUrl 是一个 ref 对象，
// Vue 自动处理其内部 value 的变化。
// 而 props.modelValue 是一个普通值，需要通过 getter 函数传递给 watch

// 监听 modelValue 的变化
watch(
  () => props.modelValue,
  (newVal) => {
    imageUrl.value = newVal;
  }
);

// 监听父组件发送的清空事件
watch(imageUrl, (newVal) => {
  //这里newVal是watch函数自带的，表示当前新更改的值
  //这里是当我们把imageUrl修改为null的时候触发下面的if判断执行handleRemove
  if (newVal === null) {
    handleRemove();
  }
});

const handlePreview = (file: File) => {
  fileData.value = file;
  const reader = new FileReader();
  reader.onload = (e) => {
    // 图片加载完成后立即显示
    imageUrl.value = e.target?.result as string;

    //为什么是 update:modelValue 而不是直接 modelValue？
    // 在 Vue 3 中，v-model 的默认语法是 modelValue（或类似名字）
    //和 update:modelValue 事件。这是因为 v-model 实际上是语法糖，
    //它在背后做了两件事：
    // 创建一个 props 用于接收传递进来的值，默认名称是 modelValue。
    // 创建一个事件，用于在值改变时发射，默认名称是 update:modelValue。
    // 所以 update:modelValue 是一个约定俗成的命名方式，
    //它让 Vue 识别这是与 modelValue 相关的更新事件。

    //这里是执行时候会更改modelValue这个值，这个值在props中定义了，我们可以在父组件中
    //通过<CoverUpload :modelValue="imageSrc">来去获取，这里的imageSrc就会获得e.target?.result的值

    // 发射事件通知父组件更新
    emit("update:modelValue", e.target?.result as string);
  };
  reader.readAsDataURL(file);
};

//上传图片后自动执行
const uploadImage = (options: any) => {
  const { file, onSuccess, onError } = options;

  // 将文件数据传递给父组件
  emit("upload", file);
};

const handleRemove = () => {
  imageUrl.value = undefined;
  emit("update:modelValue", null);
  fileData.value = null;
};
</script>

<style scoped>
.avatar {
  width: 80px;
  height: 80px;
}

.avatar-uploader .avatar {
  width: 80px;
  height: 80px;
  display: block;
}
</style>

<style>
.avatar-uploader .el-upload {
  border: 1px dashed var(--el-border-color);
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: var(--el-transition-duration-fast);
}

.avatar-uploader .el-upload:hover {
  border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 80px;
  height: 80px;
  text-align: center;
}
</style>
