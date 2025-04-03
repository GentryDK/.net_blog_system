<template>
  <div class="editor-html">
    <Toolbar
      style="border-bottom: 1px solid #ccc"
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      :mode="mode"
    />
    <Editor
      :style="{
        height: height + 'px',
        'overflow-y': 'hidden',
        'text-align': 'left',
      }"
      v-model="props.modelValue"
      :defaultConfig="editorConfig"
      :mode="mode"
      @onCreated="handleCreated"
      @onChange="onChange"
    />
  </div>
</template>

<script setup lang="ts">
import "@wangeditor/editor/dist/css/style.css"; // 引入 css
import { onBeforeUnmount, ref, shallowRef } from "vue";
import { Editor, Toolbar } from "@wangeditor/editor-for-vue";
import { IEditorConfig } from "@wangeditor/editor";
import { setPostPictureAsync } from "@/httpRequest/PostRequest";
import { superAxios } from "@/common/superAxios";

const editorRef = shallowRef();
const mode = ref("default");

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

//用于配置编辑器工具栏的选项。目前为空，可以根据需要添加工具栏配置。
const toolbarConfig = {};

const editorConfig: Partial<IEditorConfig> = {
  placeholder: "请输入内容...",
  //配置了自定义的图片上传逻辑。customUpload 函数会处理图片上传并插入图片。
  MENU_CONF: {
    uploadImage: {
      server: "", // 这个字段可以留空，因为我们使用自定义上传逻辑
      customUpload: async (files: File, insertFn: any) => {
        try {
          // 调用自定义的图片上传函数
          let result = await setPostPictureAsync(files);
          let local = await superAxios.getValue("ImageUrl");
          const url = local + result;
          // 调用插入图片方法
          insertFn(url, "图片");
        } catch (error) {
          console.error("图片上传失败:", error);
        }
      },
    },
  },
};

const emit = defineEmits();
//这个函数在编辑器内容发生变化时被调用
const onChange = (editor: any) => {
  emit("update:modelValue", editor.getHtml());
};

// 组件销毁时，也及时销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value;
  if (editor == null) return;
  editor.destroy();
});

//handleCreated：当编辑器实例创建时调用，记录编辑器实例到 editorRef 变量中。
//editorRef.value = editor：将创建的编辑器实例存储到 editorRef 中，这样你可以在其他地方访问和使用这个编辑器实例
const handleCreated = (editor: any) => {
  editorRef.value = editor;
};
</script>

<style lang="scss" scoped>
.editor-html {
  border: 1px solid #ccc;
}
</style>
