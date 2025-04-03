<template>
  <div>
    <el-table
      ref="dataTable"
      :data="dataSource.list || []"
      :height="tableHeight"
      :stripe="options.stripe"
      :border="options.border"
      header-row-class-name="table-header-row"
      highlight-current-row
      @row-click="handleRowClick"
      @selection-change="handleSelectionChange"
    >
      <!--selection选择框-->
      <el-table-column
        v-if="options.selectType && options.selectType == 'checkbox'"
        type="selection"
        width="50"
        align="center"
      ></el-table-column>
      <!--序号-->
      <el-table-column
        v-if="options.showIndex"
        label="序号"
        type="index"
        width="60"
        align="center"
      ></el-table-column>
      <!--数据列-->
      <template v-for="(column, index) in columns">
        <template v-if="column.scopedSlots">
          <el-table-column
            :key="index"
            :prop="column.prop"
            :label="column.label"
            :align="column.align || 'left'"
            :width="column.width"
          >
            <!-- #default="scope" 是 Vue 2 的插槽语法，用于将父组件传递的数据绑定到子组件的插槽内容。
                            scope 是默认传递的数据对象，可以通过 scope 访问传递的数据
                            scope.row 和 scope.$index 是从父组件传递到插槽的具体数据-->

            <!-- 为什么写在 <template> 中
使用 <template> 包裹插槽内容的原因是为了定义插槽的范围和作用域。
  当你希望在一个插槽中传递多行内容或者复杂结构时，
  使用 <template> 可以确保所有内容作为一个整体被传递。

如果你直接在 <slot> 中传递数据，通常适用于简单内容。
  复杂结构和需要定义作用域的插槽内容推荐使用 <template>。 -->
            <template v-slot:default="scope">
              <slot
                :name="column.scopedSlots"
                :index="scope.$index"
                :row="scope.row"
              >
              </slot>
            </template>
          </el-table-column>
        </template>
        <template v-else>
          <el-table-column
            :key="index"
            :prop="column.prop"
            :label="column.label"
            :align="column.align || 'left'"
            :width="column.width"
            :fixed="column.fixed"
          >
          </el-table-column>
        </template>
      </template>
    </el-table>
    <!-- 分页 -->
    <!-- 总页数 = total / page-size -->
    <!-- 如果 total 的值为 1000，而 page-size 的值为 50，那么总页数将是 1000 / 50 = 20 页。 -->
    <div class="pagination" v-if="showPagination">
      <el-pagination
        v-if="dataSource.totalCount"
        background="false"
        :total="dataSource.totalCount"
        :page-sizes="[15, 30]"
        :page-size="dataSource.pageSize"
        :current-page.sync="dataSource.pageIndex"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handlePageSizeChange"
        @current-change="handlePageNoChange"
        style="text-align: right"
      ></el-pagination>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, PropType } from "vue";

const emit = defineEmits(["rowSelected", "rowClick"]);

const props = defineProps({
  dataSource: {
    type: Object,
    default: () => ({
      list: [],
      totalCount: Number,
      pageSize: Number,
      pageIndex: Number,
    }),
  },
  //是否显示分页选项
  showPagination: {
    type: Boolean,
    default: true,
  },
  options: {
    type: Object,
    default: () => ({
      extHeight: 0,
      showIndex: false,
    }),
  },
  columns: {
    //PropType 是一个用于定义组件属性类型的工具,可以明确地指定组件属性的类型
    //这里则是用来定义数组中存储的类中每个属性值的作用
    type: Array as PropType<
      Array<{
        prop: string;
        label: string;
        align?: string;
        width?: string;
        scopedSlots?: string;
        fixed?: boolean;
      }>
    >,
    required: true,
  },
  fetch: Function, // 获取数据的函数
  initFetch: {
    type: Boolean,
    default: true,
  },
});

//顶部 60 , 内容区域距离顶部 20， 内容上下内间距 15*2  分页区域高度 46
const topHeight = 60 + 20 + 30 + 46;

const tableHeight = ref(
  props.options.tableHeight
    ? props.options.tableHeight
    : window.innerHeight - topHeight - props.options.extHeight
);

const dataTable = ref();

//行点击
const handleRowClick = (row: number) => {
  emit("rowClick", row);
};

//多选
const handleSelectionChange = (row: number) => {
  emit("rowSelected", row);
};

//当用户改变每页显示条数时触发的事件
//切换每页大小
const handlePageSizeChange = (size: number) => {
  props.dataSource.pageSize = size;
  props.dataSource.pageIndex = 1;
  if (props.fetch != null) {
    props.fetch();
  } else {
    console.log("fetch is null");
  }
};

//当用户改变当前页码时触发的事件
// 切换页码
const handlePageNoChange = (pageNo: number) => {
  props.dataSource.pageIndex = pageNo;
  if (props.fetch != null) {
    props.fetch();
  } else {
    console.log("fetch is null");
  }
};
//设置行选中
const setCurrentRow = (rowKey: string, rowValue: string) => {
  let row = props.dataSource.list.find((item: any) => {
    return item[rowKey] === rowValue;
  });
  dataTable.value.setCurrentRow(row);
};
//将子组件暴露出去，否则父组件无法调用
defineExpose({
  setCurrentRow,
});

//初始化
const init = () => {
  if (props.initFetch && props.fetch) {
    //props.fetch() 的作用是调用一个从父组件传递过来的函数
    props.fetch();
  }
};
init();
</script>

<style lang="scss">
.pagination {
  padding-top: 10px;
}
.el-pagination {
  justify-content: right;
}

.el-table__body tr.current-row > td.el-table__cell {
  background-color: #e6f0f9;
}

.el-table__body tr:hover > td.el-table__cell {
  background-color: #e6f0f9 !important;
}
</style>
