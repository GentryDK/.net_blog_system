<template>
  <div class="category">
    <el-button type="danger" @click="showEdit('add', null)">新增分类</el-button>
  </div>
  <div>
    <Table
      :dataSource="tableData"
      :columns="columns"
      :showPagination="true"
      :fetch="loadDataList"
      :options="tableOptions"
    >
      <!-- row 名称在作用域插槽中是自定义的。关键是 row 代表了 Table 组件当前正在处理的每一行数据对象
                简单说这里的row就是typePost数据库中的数据 -->
      <!-- index是索引，既表示当前拿到的是哪个typePost，我们从后端请求的多个PostType,
                 存储到前端的table中，每个posttype则是row，他的索引是index -->

      <!-- #是作用域插槽 -->
      <template #cover="{ row }">
        <div class="cover">
          <Cover :cover="`${row.cover}?t=${new Date().getTime()}`"></Cover>
        </div>
      </template>
      <!-- 这里的#op绑定插槽，这里子组件的值是父组件中动态传递过去的
                 index 和 row 是使用的父组件中的值-->
      <template #op="{ row, index }">
        <div class="op">
          <!-- href="javascript:void(0)">修改</a> 表示点击链接时不会执行任何默认动作或导航到其他页面。主要用来创建纯粹的点击事件，比如在链接中结合 JavaScript 函数来触发一些交互效果，而不是跳转到一个 URL -->
          <a href="javascript:void(0)" @click="showEdit('update', row)">修改</a>
          <el-divider direction="vertical" />
          <a href="javascript:void(0)" @click="del(row)">删除</a>
          <el-divider direction="vertical" />
          <a
            href="javascript:void(0)"
            :class="[row.order == 0 ? 'not-allow' : 'a-link']"
            @click="changeSort(row.order, 'up')"
            >上移</a
          >
          <el-divider direction="vertical" />
          <a
            href="javascript:void(0)"
            :class="[
              index === tableData.list.length - 1 ||
              tableData.pageIndex * tableData.pageSize - 1 === index
                ? 'not-allow'
                : 'a-link',
            ]"
            @click="changeSort(row.order, 'down')"
            >下移</a
          >
        </div>
      </template>
      <!-- :class 是 Vue 中的一个指令，用于动态绑定 CSS 类名。具体来看：
:class 绑定了一个数组。
当 index == 0 时，应用 not-allow 类。
否则，应用 a-link 类。 -->
    </Table>
    <!-- 这里的 @close="dialgoConfig.show=false" -->
    <Dialog
      :title="dialogConfig.title"
      :buttons="dialogConfig.buttons"
      :show="dialogConfig.show"
      @close="handleDialogClose"
    >
      <el-form :model="formData" :rules="rules" ref="formDataRef">
        <el-form-item label="名称" prop="postTypeName">
          <el-input placeholder="请输入名称" v-model="formData.postTypeName">
          </el-input>
        </el-form-item>
        <el-form-item label="封面" prop="cover">
          <CoverUpload
            v-model:modelValue="imageSrc"
            :local="local"
            @upload="collectUploadData"
            ref="coverUpload"
          />
        </el-form-item>
        <el-form-item label="简介">
          <el-input
            placeholder="请输入简介"
            v-model="formData.typeBrief"
            type="textarea"
            :autosize="{ minRows: 4, maxRows: 20 }"
          ></el-input>
        </el-form-item>
      </el-form>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, nextTick } from "vue";
import {
  getPostTypesAsync,
  getPostTypeCountAsync,
  addPostTypeAsync,
  updatePostTpyeOrder,
  removePostType,
} from "@/httpRequest/PostTypeReuest";
import { superAxios } from "@/common/superAxios";
import Cover from "@/components/Cover.vue";
import CoverUpload from "@/components/CoverUpload.vue";
import Confirm from "@/utils/Confirm";
import { ElMessage } from "element-plus";

const local = ref<string>("");
const imageSrc = ref<string | undefined>(undefined);

const columns = [
  {
    label: "封面",
    prop: "cover",
    width: 80,
    scopedSlots: "cover",
  },
  {
    label: "名称",
    prop: "postTypeName",
    width: 150,
  },
  {
    label: "简介",
    prop: "typeBrief",
  },
  {
    label: "博客数量",
    prop: "count",
    width: 80,
  },
  {
    label: "操作",
    prop: "op",
    width: 200,
    scopedSlots: "op",
  },
];

const tableData = reactive({
  list: [],
  totalCount: 0,
  pageSize: 30,
  pageIndex: 1,
});

const tableOptions = {
  extHeight: 10,
};

const dialogConfig = reactive({
  title: "添加分类信息",
  show: false,
  buttons: [
    {
      type: "danger",
      text: "确定",
      click: async (e: any) => {
        await submitAllData();
      },
    },
  ],
});

const formDataRef = ref<any>();

const formData = reactive<any>({
  postTypeId: "",
  postTypeName: "",
  cover: "",
  typeBrief: "",
  coverFile: null,
});

const rules = {
  postTypeName: [{ required: true, message: "类型名称不能为空" }],
};

//修改，新增界面
const showEdit = (type: any, data: any) => {
  dialogConfig.show = true;
  // nextTick 确保在 Vue 完成对 DOM 进行更新之后执行回调
  //nextTick 确保 formDataRef.value.resetFields()
  //和对 dialogConfig.title 的修改在 dialogConfig.show = true
  //之后执行。这样可以确保弹窗在显示之后，表单字段和标题能够正确更
  //新和渲染

  //nextTick 确保了在 DOM 更新完成后再执行回调。如果不加 nextTick，resetFields 和其他操作可能会在 Vue 更新 DOM 之前触发
  //更新 DOM 元素意味着 Vue 或者其他前端框架在页面上改变
  //这个更新一般发生在:
  //                修改了 Vue 实例的某个数据属性
  //                调用组件的方法如 showEdit，修改数据属性
  //                某些 Vue 组件生命周期钩子中，如 mounted、updated 等，Vue 会更新 DOM 元素
  nextTick(() => {
    // resetFields 是 element-plus自带的参数，可以重置页面
    formDataRef.value.resetFields();

    if (type === "add") {
      dialogConfig.title = "新增分类";
    } else if (type === "update") {
      dialogConfig.title = "编辑分类";
      Object.assign(formData, data);
      formData.postTypeId = data.id;
    }

    if (data != null) {
      imageSrc.value = local.value + data.cover;
      formData.cover = data.cover;
      formData.operationType = type;
    } else {
      imageSrc.value = undefined; // 每次打开弹窗时初始化
    }
  });
};

//删除
const del = async (data: any) => {
  Confirm(`你确认要删除${data.postTypeName}`, async () => {
    console.log("id:" + data.id);
    await removePostType(data.id as string);
    // 删除完毕后重新请求数据
    await loadDataList();
  });
};

//修改排序
const changeSort = async (index: number, type: string) => {
  let categoryList = tableData.list;

  if (
    (type === "down" && index === categoryList.length - 1) ||
    (type === "up" && index === 0)
  ) {
    return;
  }
  //是down就数组+1
  let number = type == "down" ? 1 : -1;
  let temp = categoryList[index];

  //通过 splice 方法，可以灵活地删除、添加或替换数组中的元素
  //在 JavaScript 数组中，当你删除了 index=5 的元素，index=6 及其后的所有元素会自动向前移动一个位置，以填补被删除的位置
  //categoryList.splice(index, 1)：从 categoryList 数组中删除 index 位置开始的一个元素
  categoryList.splice(index, 1);
  //categoryList.splice(index + number, 0, temp)：在 index + number 位置插入 temp 元素，不删除任何元素
  categoryList.splice(index + number, 0, temp);

  // Array.prototype.map() 方法
  // 这个方法会创建一个新数组，数组中的每个元素都是调用函数处理后的结果。
  // 它的回调函数会接收三个参数，按顺序如下：
  // 当前元素 (item)：
  // 当前处理的数组元素。
  // 当前元素的索引 (index)：
  // 当前处理的数组元素的索引。
  const updateOrders = categoryList.map((item: any, index: any) => ({
    Id: item.id,
    Order: index,
  }));

  //TODO 更新后台的Order数据
  await updatePostTpyeOrder(updateOrders);

  //修改完毕重新请求
  await loadDataList();
};

//关闭弹窗时候清空
const handleDialogClose = () => {
  imageSrc.value = undefined;
  dialogConfig.show = false;
};

// 收集上传的图片数据
const collectUploadData = (data: File) => {
  formData.coverFile = data;
};

//统一将弹窗的数据提交到后端(新增和修改都是同一个接口,具体操作在后端区别执行)
const submitAllData = async () => {
  let response;
  formDataRef.value.validate(async (valid: any) => {
    if (!valid) {
      return;
    }
  });
  try {
    response = await addPostTypeAsync({
      PostTypeId: formData.postTypeId,
      PostTypeName: formData.postTypeName,
      Cover: formData.cover,
      TypeBrief: formData.typeBrief,
      CoverFile: formData.coverFile,
    });
    ElMessage({ message: "博客分类上传成功", type: "success" });
    dialogConfig.show = false;
    await loadDataList();
  } catch (error: any) {
    ElMessage({ message: "博客分类上传失败", type: "error" });
  }
};

//请求后端的table数据(新增和修改都是)
const loadDataList = async () => {
  let res = await getPostTypesAsync({
    pageIndex: tableData.pageIndex,
    pageSize: tableData.pageSize,
  });
  tableData.list = res;

  console.log(tableData.pageIndex);
  console.log(tableData.pageSize);
};

//获取当前所有的发帖类型的总和
const loadPostTypeCount = async () => {
  let result = await getPostTypeCountAsync();
  if (result) {
    tableData.totalCount = result;
  }
};

//分页
const pageChange4BlogList = async () => {
  await loadDataList();
};

onMounted(async () => {
  local.value = await superAxios.getValue("ImageUrl");
  await loadDataList();
  await loadPostTypeCount();
});
</script>

<style lang="scss" scoped>
.category {
  display: flex;
}
</style>
