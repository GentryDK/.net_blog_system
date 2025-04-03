import { ElMessage, ElMessageBox } from 'element-plus'

const confirm = (message:string,okfun:any)=>{
    ElMessageBox.confirm(
        message,
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'info',
        }
      ).then(() => {
        okfun();
        })
        .catch(() => {})
    }

    export default confirm;

  