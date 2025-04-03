<template>
    <div>
        <!-- custom-class 是 Element Plus 提供的一种属性，用于为组件的最外层元素添加自定义的 CSS 类 -->
        <el-dialog :show-close="false" 
                   :draggable="true" 
                   :model-value="show" 
                   :close-on-click-modal="false"
                   :title="title"
                  class="cust-dialog"
                   :top="top"
                   :width="width"
                   :showCancel="showCancel">
                   <div class="dialog-body">
                        <slot></slot>
                   </div>
                   <template v-if="buttons && buttons.length>0 || showCancel">
                    <div class="dialog-footer">
                        <el-button link 
                                   @click="close"
                                   size="small"> 取消 </el-button>
                        <el-button v-for="btn in buttons"
                                :type="btn.type"
                                size="small"
                                @click="btn.click">
                            {{ btn.text }}
                        </el-button>
                    </div>
                   </template>
        </el-dialog>
    </div>
</template>

<script lang="ts" setup>

interface Button {
    type: string;
    text:string;
    click: () => void;
}

const props = defineProps({
    title: {
        type: String
    },
    show: {
        type: Boolean,
        default: true
    },
    showClose: {
        type: Boolean,
        default: true
    },
    showCancel: {
        type: Boolean,
        default: true
    },
    top: {
        type: String,
        default: ""
    },
    width: {
        type: String,
        default: "30%"
    },
    buttons:{
        type:Array as()=> Button[],
        default:()=>[]
    }
})

const emit = defineEmits();
const close = ()=>{
    emit("close");
}

</script>

<style lang="scss">
.cust-dialog {
  .el-dialog__body {
    padding: 0px;
  }
  .dialog-body {
    border-top: 1px solid #ddd;
    border-bottom: 1px solid #ddd;
    padding: 15px;
    min-height: 80px;
  }
  .dialog-footer {
    text-align: right;
    padding: 5px 20px;
    .el-button{
        margin-bottom: -15px;
    }
  }
}
</style>